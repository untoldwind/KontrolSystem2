using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.SpaceWarpMod.UI;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.Core {
    internal class AutopilotHooks {
        private readonly IKSPContext context;
        internal readonly List<IKSPAutopilot> autopilots = new List<IKSPAutopilot>();

        internal AutopilotHooks(IKSPContext context) => this.context = context;

        internal void Add(IKSPAutopilot autopilot) {
            if (!autopilots.Contains(autopilot)) autopilots.Add(autopilot);
        }

        internal bool Remove(IKSPAutopilot autopilot) => autopilots.Remove(autopilot);

        internal bool IsEmpty => autopilots.Count == 0;

        internal bool TryFindAutopilot<T>(out T autopilot) where T : IKSPAutopilot {
            foreach (var item in autopilots) {
                if (item is T t) {
                    autopilot = t;
                    return true;
                }
            }

            autopilot = default;
            return false;
        }
        
        internal void RunAutopilots(ref FlightCtrlState state, float deltaTime) {
            try {
                ContextHolder.CurrentContext.Value = context;
                foreach (IKSPAutopilot autopilot in autopilots)
                    autopilot.UpdateAutopilot(ref state, deltaTime);
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }
    }
    public class KSPContext : IKSPContext {
        private readonly GameInstance gameInstance;
        private readonly KSPConsoleBuffer consoleBuffer;
        private object nextYield;
        private Action onNextYieldOnce;
        private readonly Stopwatch timeStopwatch;
        private readonly long timeoutMillis;
        internal readonly List<IMarker> markers;
        private readonly Dictionary<VesselComponent, AutopilotHooks> autopilotHooks;
        private readonly List<BackgroundKSPContext> childContexts;

        public KSPContext(GameInstance gameInstance, KSPConsoleBuffer consoleBuffer) {
            this.gameInstance = gameInstance;
            this.consoleBuffer = consoleBuffer;
            markers = new List<IMarker>();
            autopilotHooks = new Dictionary<VesselComponent, AutopilotHooks>();
            nextYield = new WaitForFixedUpdate();
            childContexts = new List<BackgroundKSPContext>();
            timeStopwatch = Stopwatch.StartNew();
            timeoutMillis = 100;
        }

        public bool IsBackground => false;
        public ITO2Logger Logger => LoggerAdapter.Instance;

        public void CheckTimeout() {
            long elapsed = timeStopwatch.ElapsedMilliseconds;
            if (elapsed >= timeoutMillis)
                throw new YieldTimeoutException(elapsed);
        }

        public void ResetTimeout() {
            if (onNextYieldOnce != null) {
                onNextYieldOnce();
                onNextYieldOnce = null;
            }
            timeStopwatch.Reset();
            timeStopwatch.Start();
        }

        public IContext CloneBackground(CancellationTokenSource token) {
            var childContext = new BackgroundKSPContext(consoleBuffer, token);

            childContexts.Add(childContext);

            return childContext;
        }

        public GameInstance Game => gameInstance;

        public GameMode GameMode => GameModeAdapter.GameModeFromState(Game.GlobalGameState.GetState());

        public Font ConsoleFont(int fontSize) => FontManager.Instance.GetSystemFontByNameAndSize(FontManager.DefaultConsoleFonts,
            fontSize, true);
        
        public double UniversalTime => Game.SpaceSimulation.UniverseModel.UniversalTime;

        public VesselComponent ActiveVessel => gameInstance.ViewController.GetActiveSimVessel(true);
        
        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;

        public KSPOrbitModule.IBody FindBody(string name) {
            var body = Game.ViewController.GetBodyByName(name);

            return body != null ? new BodyWrapper(this, body) : null;
        }

        public object NextYield {
            get {
                object result = nextYield;
                nextYield = new WaitForFixedUpdate();
                return result;
            }
            set => nextYield = value;
        }

        public Action OnNextYieldOnce {
            get => onNextYieldOnce;
            set => onNextYieldOnce = value;
        }

        public void AddMarker(IMarker marker) => markers.Add(marker);

        public void RemoveMarker(IMarker marker) {
            marker.Visible = false;
            markers.Remove(marker);
        }

        public void ClearMarkers() {
            foreach (IMarker marker in markers) marker.Visible = false;
            markers.Clear();
        }

        public void TriggerMarkerUpdate() {
            try {
                ContextHolder.CurrentContext.Value = this;
                foreach (IMarker marker in markers)
                    marker.OnUpdate();
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }

        public void TriggerMarkerRender() {
            try {
                ContextHolder.CurrentContext.Value = this;
                foreach (IMarker marker in markers)
                    marker.OnRender();
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }

        public bool TryFindAutopilot<T>(VesselComponent vessel, out T autopilot) where T : IKSPAutopilot {
            if (autopilotHooks.ContainsKey(vessel)) {
                return autopilotHooks[vessel].TryFindAutopilot(out autopilot);
            }

            autopilot = default;
            return false;
        }
        
        public void HookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot) {
            LoggerAdapter.Instance.Debug($"Hook autopilot {autopilot} to {vessel.Name}");
            if (autopilotHooks.ContainsKey(vessel)) {
                autopilotHooks[vessel].Add(autopilot);
            } else {
                AutopilotHooks autopilots = new AutopilotHooks(this);
                autopilots.Add(autopilot);
                autopilotHooks.Add(vessel, autopilots);

                LoggerAdapter.Instance.Debug($"Hooking up for vessel: {vessel.Name}");
                // Ensure that duplicates do no trigger an exception
                vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
                vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate += autopilots.RunAutopilots;
            }
        }

        public void UnhookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot) {
            if (!autopilotHooks.ContainsKey(vessel)) return;

            LoggerAdapter.Instance.Debug($"Unhook autopilot {autopilot} to {vessel.Name}");

            AutopilotHooks autopilots = autopilotHooks[vessel];

            autopilots.Remove(autopilot);
            if (autopilots.IsEmpty) {
                LoggerAdapter.Instance.Debug($"Unhooking from vessel: {vessel.Name}");
                autopilotHooks.Remove(vessel);
                vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
            }
        }

        public void UnhookAllAutopilots(VesselComponent vessel) {
            if (!autopilotHooks.ContainsKey(vessel)) return;

            AutopilotHooks autopilots = autopilotHooks[vessel];

            autopilotHooks.Remove(vessel);
            LoggerAdapter.Instance.Debug($"Unhooking from vessel: {vessel.Name}");
            vessel.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
        }

        public void Cleanup() {
            ClearMarkers();
            foreach (var kv in autopilotHooks) {
                LoggerAdapter.Instance.Debug($"Unhooking from vessel: {kv.Key.Name}");
                kv.Key.SimulationObject.objVesselBehavior.OnPreAutopilotUpdate -= kv.Value.RunAutopilots;
            }

            foreach (var childContext in childContexts) {
                childContext.Cleanup();
            }

            autopilotHooks.Clear();
            childContexts.Clear();
        }
    }

    public class BackgroundKSPContext : IContext {
        private readonly KSPConsoleBuffer consoleBuffer;
        private readonly CancellationTokenSource token;
        private readonly List<BackgroundKSPContext> childContexts;

        public BackgroundKSPContext(KSPConsoleBuffer consoleBuffer, CancellationTokenSource token) {
            this.consoleBuffer = consoleBuffer;
            this.token = token;
            childContexts = new List<BackgroundKSPContext>();
        }

        public ITO2Logger Logger => LoggerAdapter.Instance;

        public bool IsBackground => true;

        public void CheckTimeout() => token.Token.ThrowIfCancellationRequested();

        public void ResetTimeout() {
        }

        public IContext CloneBackground(CancellationTokenSource token) {
            var childContext = new BackgroundKSPContext(consoleBuffer, token);

            childContexts.Add(childContext);

            return childContext;
        }

        public void Cleanup() {
            if (token.Token.CanBeCanceled) {
                token.Cancel();
            }

            foreach (var childContext in childContexts) {
                childContext.Cleanup();
            }
        }
    }
}
