using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using KontrolSystem.Parsing;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using KSP.Messages;
using KSP.Sim.impl;
using UnityEngine;
using UnityEngine.Events;

namespace KontrolSystem.KSP.Runtime.Core;

public readonly struct MainframeError {
    public readonly Position position;
    public readonly string errorType;
    public readonly string message;

    public MainframeError(Position position, string errorType, string message) {
        this.position = position;
        this.errorType = errorType;
        this.message = message;
    }
}

public class Mainframe : KerbalMonoBehaviour {
    private static readonly char[] PathSeparator = { '\\', '/' };

    public UnityEvent availableProcessesChanged = new();

    private readonly Dictionary<Guid, Coroutine> coroutines = new();

    private KontrolSystemConfig? config;

    private List<KontrolSystemProcess>? processes;
    private volatile bool rebooting;

    private volatile State? state;

    public bool Initialized => state != null;

    public KSPConsoleBuffer ConsoleBuffer { get; } = new(40, 50);

    public TimeSeriesCollection TimeSeriesCollection { get; } = new();

    public bool Rebooting => rebooting;
    public TimeSpan LastRebootTime => state?.bootTime ?? TimeSpan.Zero;
    public IEnumerable<MainframeError> LastErrors => state?.errors ?? Enumerable.Empty<MainframeError>();
    public KontrolRegistry? LastRegistry => state?.registry;

    public static Mainframe? Instance { get; private set; }

    public ITO2Logger Logger => config!.Logger;

    public OptionalAddons OptionalAddons => config!.OptionalAddons;

    public string Version => config!.Version;

    public string LocalLibPath => config!.LocalLibPath;

    public KSPGameMode GameMode => GameModeAdapter.GameModeFromState(Game.GlobalGameState.GetState());

    public KontrolSystemProcess[] AvailableProcesses {
        get {
            var activeVessel = Game.ViewController.GetActiveSimVessel();

            return ListProcesses(GameMode, activeVessel).ToArray();
        }
    }

    public void Awake() {
        Instance = this;
        Game.Messages.Subscribe<GameStateChangedMessage>(OnStateChange);
    }

    public void Update() {
        if (processes == null) return;
        foreach (var process in processes)
            switch (process.State) {
            case KontrolSystemProcessState.Outdated:
            case KontrolSystemProcessState.Running:
                process.context?.TriggerMarkerUpdate();
                break;
            }
    }

    public void OnGUI() {
        if (processes == null) return;
        foreach (var process in processes)
            switch (process.State) {
            case KontrolSystemProcessState.Outdated:
            case KontrolSystemProcessState.Running:
                process.context?.TriggerMarkerRender();
                break;
            }
    }

    public void Initialize(KontrolSystemConfig config) {
        this.config = config;
    }

    public void Destroy() {
        Instance = null;
        Game.Messages.Unsubscribe<GameStateChangedMessage>(OnStateChange);
    }

    public void Reboot() {
        if (rebooting) return;
        DoReboot(config!);
    }

    private async void DoReboot(KontrolSystemConfig config) {
        rebooting = true;
        availableProcessesChanged.Invoke();
        var nextState = await Task.Run(() => {
            var stopwatch = new Stopwatch();
            try {
                stopwatch.Start();

                var nextRegistry = KontrolSystemKSPRegistry.CreateKSP();

                if (Directory.Exists(config.StdLibPath)) {
                    config.Logger.Debug($"Add Directory: {config.StdLibPath}");
                    nextRegistry.AddDirectory(config.StdLibPath);
                } else {
                    config.Logger.Warning($"StdLibPath does not exists: {config.LocalLibPath}");
                }

                if (Directory.Exists(config.LocalLibPath)) {
                    config.Logger.Debug($"Add Directory: {config.LocalLibPath}");
                    nextRegistry.AddDirectory(config.LocalLibPath);
                } else {
                    config.Logger.Warning($"LocalLibPath does not exists: {config.LocalLibPath}");
                }

                stopwatch.Stop();

                return new State(nextRegistry, stopwatch.Elapsed, new List<MainframeError>());
            } catch (CompilationErrorException e) {
                config.Logger.Debug(e.ToString());

                foreach (var error in e.errors) config.Logger.Info(error.ToString());

                return new State(state!.registry, stopwatch.Elapsed, e.errors.Select(error => new MainframeError(
                    error.start,
                    error.errorType.ToString(),
                    error.message
                )).ToList());
            } catch (ParseException e) {
                config.Logger.Debug(e.ToString());
                config.Logger.Info(e.Message);

                return new State(state?.registry, stopwatch.Elapsed, new List<MainframeError> {
                    new(e.position, "Parsing", e.Message)
                });
            } catch (Exception e) {
                config.Logger.Error("Mainframe initialization error: " + e);

                return new State(state?.registry, stopwatch.Elapsed, new List<MainframeError> {
                    new(new Position(), "Unknown error", e.Message)
                });
            }
        });
        if (nextState.errors.Count == 0 && nextState.registry != null)
            processes = (processes ?? Enumerable.Empty<KontrolSystemProcess>())
                .Where(p => p.State == KontrolSystemProcessState.Running ||
                            p.State == KontrolSystemProcessState.Outdated).Select(p => p.MarkOutdated()).Concat(
                    nextState.registry.modules.Values
                        .Where(module =>
                            module.HasKSCEntrypoint() || module.HasEditorEntrypoint() ||
                            module.HasTrackingEntrypoint() ||
                            module.HasFlightEntrypoint())
                        .Select(module => new KontrolSystemProcess(config.Logger, module))).ToList();

        state = nextState;
        rebooting = false;
        availableProcessesChanged.Invoke();
    }

    public IEnumerable<KontrolSystemProcess> ListProcesses(KSPGameMode gameMode, VesselComponent vessel) {
        return processes != null
            ? processes.Where(p => p.AvailableFor(gameMode, vessel))
            : Enumerable.Empty<KontrolSystemProcess>();
    }

    public bool StartProcess(KontrolSystemProcess process, VesselComponent? vessel = null, object[]? arguments = null) {
        switch (process.State) {
        case KontrolSystemProcessState.Available:
            var context = new KSPCoreContext(process.logger, Game, ConsoleBuffer, TimeSeriesCollection,
                config!.OptionalAddons);
            var entrypoint = process.EntrypointFor(context.GameMode, context);
            if (entrypoint == null) return false;
            arguments ??= process.EntrypointArgumentDescriptors(context.GameMode).Select(arg => arg.DefaultValue)
                .ToArray();
            var activeVessel = vessel ?? Game.ViewController.GetActiveSimVessel();
            var adapter = new CorouttineAdapter(entrypoint(activeVessel, arguments), context,
                message => OnProcessDone(process, message));
            process.MarkRunning(context);

            var coroutine = StartCoroutine(adapter);

            if (coroutines.ContainsKey(process.id)) {
                StopCoroutine(coroutines[process.id]);
                coroutines[process.id] = coroutine;
            } else {
                coroutines.Add(process.id, coroutine);
            }

            availableProcessesChanged.Invoke();

            return true;
        default:
            return false;
        }
    }

    public void TriggerBoot(KSPGameMode gameMode, VesselComponent vessel) {
        var bootProcess = processes?.FirstOrDefault(p => p.IsBootFor(gameMode, vessel));

        if (bootProcess?.State != KontrolSystemProcessState.Available) return;

        StartProcess(bootProcess, vessel);
    }

    public bool StopProcess(KontrolSystemProcess process) {
        switch (process.State) {
        case KontrolSystemProcessState.Running:
        case KontrolSystemProcessState.Outdated:
            if (coroutines.TryGetValue(process.id, out var coroutine)) {
                StopCoroutine(coroutine);
                OnProcessDone(process, "Aborted by pilot");
            }

            return true;
        default:
            return false;
        }
    }

    public void StopAll() {
        if (processes == null) return;
        foreach (var process in processes)
            if (coroutines.TryGetValue(process.id, out var coroutine)) {
                StopCoroutine(coroutine);
                OnProcessDone(process, "Aborted by pilot", false);
            }

        availableProcessesChanged.Invoke();
    }

    private void OnProcessDone(KontrolSystemProcess process, string? message, bool triggerEvent = true) {
        if (process.State == KontrolSystemProcessState.Outdated) processes?.Remove(process);

        process.MarkDone(message);
        coroutines.Remove(process.id);

        if (triggerEvent) availableProcessesChanged.Invoke();
    }

    private void OnStateChange(MessageCenterMessage message) {
        if (message is GameStateChangedMessage g) {
            var prevMode = GameModeAdapter.GameModeFromState(g.PreviousState);
            var currentMode = GameModeAdapter.GameModeFromState(g.CurrentState);

            if (prevMode != currentMode) {
                config?.Logger?.Info($"Stop all processes due to mode change {prevMode} -> {currentMode}");
                StopAll();
            }
        }
    }
}
