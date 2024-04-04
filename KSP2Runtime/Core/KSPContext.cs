﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPDebug;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using KontrolSystem.KSP.Runtime.KSPUI;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Core;

internal class AutopilotHooks {
    internal readonly List<IKSPAutopilot> autopilots = [];
    private readonly IKSPContext context;

    internal AutopilotHooks(IKSPContext context) {
        this.context = context;
    }

    internal bool IsEmpty => autopilots.Count == 0;

    internal void Add(IKSPAutopilot autopilot) {
        if (!autopilots.Contains(autopilot)) {
            autopilots.Add(autopilot);
            autopilots.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }

    internal bool Remove(IKSPAutopilot autopilot) {
        return autopilots.Remove(autopilot);
    }

    internal bool TryFindAutopilot<T>([MaybeNullWhen(false)] out T autopilot) where T : IKSPAutopilot {
        foreach (var item in autopilots)
            if (item is T t) {
                autopilot = t;
                return true;
            }

        autopilot = default;
        return false;
    }

    internal void RunAutopilots(ref FlightCtrlState state, float deltaTime) {
        try {
            ContextHolder.CurrentContext.Value = context;
            foreach (var autopilot in autopilots)
                autopilot.UpdateAutopilot(ref state, deltaTime);
        } finally {
            ContextHolder.CurrentContext.Value = null;
        }
    }
}

public class KSPCoreContext(string processName, ITO2Logger logger, GameInstance gameInstance, KSPConsoleBuffer consoleBuffer,
    TimeSeriesCollection timeSeriesCollection, OptionalAddons optionalAddons) : IKSPContext {
    internal const int MaxCallStack = 100;
    private readonly Dictionary<VesselComponent, AutopilotHooks> autopilotHooks = [];
    private readonly List<BackgroundKSPContext> childContexts = [];

    private readonly List<IMarker> markers = [];
    private readonly List<KSPResourceModule.ResourceTransfer> resourceTransfers = [];
    private readonly Stopwatch timeStopwatch = Stopwatch.StartNew();
    private readonly long timeoutMillis = 100;
    private readonly List<KSPUIModule.Window> windows = [];
    private object? nextYield = new WaitForFixedUpdate();
    private int stackCallCount;
    private readonly List<Action> nextUpdateOnce = [];
    private readonly Stack<CoreError.StackEntry> callStack = new();
    private readonly Dictionary<string, DirectLogFile> logFiles = [];
    private readonly List<MessageBus.Subscription> subscriptions = [];

    public bool IsBackground => false;
    public ITO2Logger Logger { get; } = logger;

    public void CheckTimeout() {
        var elapsed = timeStopwatch.ElapsedMilliseconds;
        if (elapsed >= timeoutMillis)
            throw new YieldTimeoutException(elapsed);
    }

    public void ResetTimeout() {
        timeStopwatch.Reset();
        timeStopwatch.Start();
    }


    public void FunctionEnter(string name, object[] arguments, string sourceName, int line) {
        if (Interlocked.Increment(ref stackCallCount) > MaxCallStack)
            throw new StackOverflowException($"Exceed stack count: {MaxCallStack}");
        callStack.Push(new CoreError.StackEntry(name, arguments, sourceName, line));
    }

    public void FunctionLeave() {
        Interlocked.Decrement(ref stackCallCount);
        callStack.TryPop(out _);
    }

    public CoreError.StackEntry? CurrentCallSite { get; set; }

    public IEnumerable<CoreError.StackEntry> CurrentStack() => callStack.Reverse();

    public IContext CloneBackground(CancellationTokenSource token) {
        var childContext = new BackgroundKSPContext(Logger, ConsoleBuffer, token);

        childContexts.Add(childContext);

        return childContext;
    }

    public GameInstance Game { get; } = gameInstance;

    public KSPGameMode GameMode => GameModeAdapter.GameModeFromState(Game.GlobalGameState.GetState());

    public string ProcessName { get; } = processName;

    public double UniversalTime => Game.SpaceSimulation.UniverseModel.UniverseTime;

    public VesselComponent ActiveVessel => Game.ViewController.GetActiveSimVessel();

    public KSPConsoleBuffer ConsoleBuffer { get; } = consoleBuffer;

    public TimeSeriesCollection TimeSeriesCollection { get; } = timeSeriesCollection;

    public KSPOrbitModule.IBody? FindBody(string name) {
        var body = Game.ViewController.GetBodyByName(name);

        return body != null ? new BodyWrapper(this, body) : null;
    }

    public object? NextYield {
        get {
            var result = nextYield;
            nextYield = new WaitForFixedUpdate();
            return result;
        }
        set => nextYield = value;
    }

    public void AddMarker(IMarker marker) {
        markers.Add(marker);
    }

    public void RemoveMarker(IMarker marker) {
        marker.Visible = false;
        markers.Remove(marker);
    }

    public void ClearMarkers() {
        foreach (var marker in markers) marker.Visible = false;
        markers.Clear();
    }

    public void AddResourceTransfer(KSPResourceModule.ResourceTransfer resourceTransfer) {
        resourceTransfers.Add(resourceTransfer);
    }

    public void AddWindow(KSPUIModule.Window window) {
        windows.Add(window);
    }

    public KSPDebugModule.ILogFile AddLogFile(string fileName) {
        var sanitizedName = Regex.Replace(fileName, "[^0-9a-zA-Z_\\-]+", "_") + ".log";
        if (logFiles.TryGetValue(sanitizedName, out var logFile)) {
            return logFile;
        }

        var newLogFile = new DirectLogFile(Path.Combine(Mainframe.Instance!.LocalLibPath, "logs"), sanitizedName);
        logFiles.Add(sanitizedName, newLogFile);
        return newLogFile;
    }

    public MessageBus.Subscription<T> AddSubscription<T>() {
        var subscription = Mainframe.Instance!.MessageBus.Subscribe<T>();
        subscriptions.Add(subscription);
        return subscription;
    }
    
    public bool TryFindAutopilot<T>(VesselComponent vessel, [MaybeNullWhen(false)] out T autopilot) where T : IKSPAutopilot {
        if (autopilotHooks.TryGetValue(vessel, out var hook)) return hook.TryFindAutopilot(out autopilot);

        autopilot = default;
        return false;
    }

    public void HookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot) {
        Logger.Debug($"Hook autopilot {autopilot} to {vessel.Name}");
        if (autopilotHooks.TryGetValue(vessel, out var hook)) {
            hook.Add(autopilot);
        } else {
            var autopilots = new AutopilotHooks(this);
            autopilots.Add(autopilot);
            autopilotHooks.Add(vessel, autopilots);

            Logger.Debug($"Hooking up for vessel: {vessel.Name}");
            // Ensure that duplicates do no trigger an exception
            vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
            vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate += autopilots.RunAutopilots;
        }
    }

    public void UnhookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot) {
        if (!autopilotHooks.ContainsKey(vessel)) return;

        Logger.Debug($"Unhook autopilot {autopilot} to {vessel.Name}");

        var autopilots = autopilotHooks[vessel];

        autopilots.Remove(autopilot);
        if (autopilots.IsEmpty) {
            Logger.Debug($"Unhooking from vessel: {vessel.Name}");
            autopilotHooks.Remove(vessel);
            vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
        }
    }

    public void UnhookAllAutopilots(VesselComponent vessel) {
        if (!autopilotHooks.ContainsKey(vessel)) return;

        var autopilots = autopilotHooks[vessel];

        autopilotHooks.Remove(vessel);
        Logger.Debug($"Unhooking from vessel: {vessel.Name}");
        vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
    }

    public OptionalAddons OptionalAddons { get; } = optionalAddons;

    public void TriggerMarkerUpdate() {
        try {
            ContextHolder.CurrentContext.Value = this;
            foreach (var marker in markers)
                marker.OnUpdate();
            foreach (var action in nextUpdateOnce) {
                action();
            }
            nextUpdateOnce.Clear();
        } finally {
            ContextHolder.CurrentContext.Value = null;
        }
    }

    public void TriggerMarkerRender() {
        try {
            ContextHolder.CurrentContext.Value = this;
            foreach (var marker in markers)
                marker.OnRender();
        } finally {
            ContextHolder.CurrentContext.Value = null;
        }
    }

    public void Cleanup() {
        ClearMarkers();
        foreach (var kv in autopilotHooks) {
            Logger.Debug($"Unhooking from vessel: {kv.Key.Name}");
            if (kv.Key.SimulationObject != null && kv.Key.SimulationObject.objVesselBehavior != null)
                kv.Key.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= kv.Value.RunAutopilots;
        }

        foreach (var resourceTransfer in resourceTransfers.ToArray()) resourceTransfer.Clear();

        foreach (var window in windows.ToArray()) window.Close();

        foreach (var childContext in childContexts.ToArray()) childContext.Cleanup();

        foreach (var logFile in logFiles.Values) logFile.Close();

        foreach (var subscription in subscriptions) subscription.Unsubscribe();
        
        subscriptions.Clear();
        logFiles.Clear();
        windows.Clear();
        resourceTransfers.Clear();
        autopilotHooks.Clear();
        childContexts.Clear();
    }

    public void AddNextUpdateOnce(Action action) {
        nextUpdateOnce.Add(action);
    }
}

public class BackgroundKSPContext(ITO2Logger logger, KSPConsoleBuffer consoleBuffer, CancellationTokenSource token) : IContext {
    private readonly List<BackgroundKSPContext> childContexts = [];
    private int stackCallCount;
    private readonly Stack<CoreError.StackEntry> callStack = new();

    public ITO2Logger Logger { get; } = logger;

    public bool IsBackground => true;

    public void CheckTimeout() {
        token.Token.ThrowIfCancellationRequested();
    }

    public void ResetTimeout() {
    }

    public IContext CloneBackground(CancellationTokenSource token) {
        var childContext = new BackgroundKSPContext(Logger, consoleBuffer, token);

        childContexts.Add(childContext);

        return childContext;
    }

    public void FunctionEnter(string name, object[] arguments, string sourceName, int line) {
        if (Interlocked.Increment(ref stackCallCount) > KSPCoreContext.MaxCallStack)
            throw new StackOverflowException($"Exceed stack count: {KSPCoreContext.MaxCallStack}");
        callStack.Push(new CoreError.StackEntry(name, arguments, sourceName, line));
    }

    public void FunctionLeave() {
        Interlocked.Decrement(ref stackCallCount);
        callStack.TryPop(out _);
    }

    public CoreError.StackEntry? CurrentCallSite { get; set; }

    public IEnumerable<CoreError.StackEntry> CurrentStack() => callStack.Reverse();

    public void Cleanup() {
        if (token.Token.CanBeCanceled) token.Cancel();

        foreach (var childContext in childContexts) childContext.Cleanup();
    }
}
