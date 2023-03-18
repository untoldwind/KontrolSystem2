using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.Parsing;
using KontrolSystem.SpaceWarpMod.Utils;
using KontrolSystem.TO2;
using KSP.Api;
using KSP.Game;
using KSP.Messages;
using KSP.Sim.impl;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.Core {
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

    public class Mainframe : Singleton<Mainframe> {
        static readonly char[] PathSeparator = { '\\', '/' };

        volatile State state;
        volatile bool rebooting;

        List<KontrolSystemProcess> processes;

        readonly KSPConsoleBuffer consoleBuffer = new KSPConsoleBuffer(40, 50);

        public bool Initialized => state != null;

        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;

        public bool Rebooting => rebooting;
        public TimeSpan LastRebootTime => state?.bootTime ?? TimeSpan.Zero;
        public IEnumerable<MainframeError> LastErrors => state?.errors ?? Enumerable.Empty<MainframeError>();
        public KontrolRegistry LastRegistry => state?.registry;

        private readonly Dictionary<Guid, Coroutine> coroutines = new Dictionary<Guid, Coroutine>();

        public void Awake() {
            Game.Messages.Subscribe<GameStateChangedMessage>(OnStateChange);
        }

        public void Destroy() {
            Game.Messages.Unsubscribe<GameStateChangedMessage>(OnStateChange);
        }

        public void Update() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                switch (process.State) {
                case KontrolSystemProcessState.Outdated:
                case KontrolSystemProcessState.Running:
                    process.context?.TriggerMarkerUpdate();
                    break;
                }
            }
        }

        public void OnGUI() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                switch (process.State) {
                case KontrolSystemProcessState.Outdated:
                case KontrolSystemProcessState.Running:
                    process.context?.TriggerMarkerRender();
                    break;
                }
            }
        }

        public void Reboot(KontrolSystemConfig config) {
            if (rebooting) return;
            DoReboot(config);
        }

        private async void DoReboot(KontrolSystemConfig config) {
            rebooting = true;
            State nextState = await Task.Run(() => {
                Stopwatch stopwatch = new Stopwatch();
                try {
                    stopwatch.Start();

                    KontrolRegistry nextRegistry = KontrolSystemKSPRegistry.CreateKSP();

                    if (Directory.Exists(config.StdLibPath)) {
                        LoggerAdapter.Instance.Debug($"Add Directory: {config.StdLibPath}");
                        nextRegistry.AddDirectory(config.StdLibPath);
                    } else {
                        LoggerAdapter.Instance.Warning($"StdLibPath does not exists: {config.LocalLibPath}");
                    }

                    if (Directory.Exists(config.LocalLibPath)) {
                        LoggerAdapter.Instance.Debug($"Add Directory: {config.LocalLibPath}");
                        nextRegistry.AddDirectory(config.LocalLibPath);
                    } else {
                        LoggerAdapter.Instance.Warning($"LocalLibPath does not exists: {config.LocalLibPath}");
                    }
                    stopwatch.Stop();

                    return new State(nextRegistry, stopwatch.Elapsed, new List<MainframeError>());
                } catch (CompilationErrorException e) {
                    LoggerAdapter.Instance.Debug(e.ToString());

                    foreach (StructuralError error in e.errors) {
                        LoggerAdapter.Instance.Info(error.ToString());
                    }

                    return new State(state?.registry, stopwatch.Elapsed, e.errors.Select(error => new MainframeError(
                        error.start,
                        error.errorType.ToString(),
                        error.message
                    )).ToList());
                } catch (ParseException e) {
                    LoggerAdapter.Instance.Debug(e.ToString());
                    LoggerAdapter.Instance.Info(e.Message);

                    return new State(state?.registry, stopwatch.Elapsed, new List<MainframeError> {
                        new MainframeError(e.position, "Parsing", e.Message)
                    });
                } catch (Exception e) {
                    LoggerAdapter.Instance.Error("Mainframe initialization error: " + e);

                    return new State(state?.registry, stopwatch.Elapsed, new List<MainframeError> {
                        new MainframeError(new Position(), "Unknown error", e.Message)
                    });
                }
            });
            if (nextState.errors.Count == 0) {
                processes = (processes ?? Enumerable.Empty<KontrolSystemProcess>())
                    .Where(p => p.State == KontrolSystemProcessState.Running ||
                                p.State == KontrolSystemProcessState.Outdated).Select(p => p.MarkOutdated()).Concat(
                        nextState.registry.modules.Values
                            .Where(module =>
                                module.HasKSCEntrypoint() || module.HasEditorEntrypoint() ||
                                module.HasTrackingEntrypoint() ||
                                module.HasFlightEntrypoint())
                            .Select(module => new KontrolSystemProcess(module))).ToList();
            }

            state = nextState;
            rebooting = false;
        }

        public IEnumerable<KontrolSystemProcess> ListProcesses(GameMode gameMode, VesselComponent vessel) {
            return processes != null
                ? processes.Where(p => p.AvailableFor(gameMode, vessel))
                : Enumerable.Empty<KontrolSystemProcess>();
        }

        public bool StartProcess(KontrolSystemProcess process, VesselComponent vessel, object[] arguments = null) {
            switch (process.State) {
            case KontrolSystemProcessState.Available:
                KSPContext context = new KSPContext(Game, consoleBuffer);
                Entrypoint entrypoint = process.EntrypointFor(context.GameMode, context);
                if (entrypoint == null) return false;
                arguments ??= process.EntrypointArgumentDescriptors(context.GameMode).Select(arg => arg.DefaultValue).ToArray();
                CorouttineAdapter adapter = new CorouttineAdapter(entrypoint(vessel, arguments), context,
                    message => OnProcessDone(process, message));
                process.MarkRunning(context);

                Coroutine coroutine = StartCoroutine(adapter);

                if (coroutines.ContainsKey(process.id)) {
                    StopCoroutine(coroutines[process.id]);
                    coroutines[process.id] = coroutine;
                } else {
                    coroutines.Add(process.id, coroutine);
                }

                return true;
            default:
                return false;
            }
        }

        public void TriggerBoot(GameMode gameMode, VesselComponent vessel) {
            KontrolSystemProcess bootProcess = processes?.FirstOrDefault(p => p.IsBootFor(gameMode, vessel));

            if (bootProcess?.State != KontrolSystemProcessState.Available) return;

            StartProcess(bootProcess, vessel);
        }

        public bool StopProcess(KontrolSystemProcess process) {
            switch (process.State) {
            case KontrolSystemProcessState.Running:
            case KontrolSystemProcessState.Outdated:
                if (coroutines.ContainsKey(process.id)) {
                    StopCoroutine(coroutines[process.id]);
                    OnProcessDone(process, "Aborted by pilot");
                }

                return true;
            default:
                return false;
            }
        }

        public void StopAll() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                if (coroutines.ContainsKey(process.id)) {
                    StopCoroutine(coroutines[process.id]);
                    OnProcessDone(process, "Aborted by pilot");
                }
            }
        }

        private void OnProcessDone(KontrolSystemProcess process, string message) {
            if (process.State == KontrolSystemProcessState.Outdated) {
                processes.Remove(process);
            }

            process.MarkDone(message);
            coroutines.Remove(process.id);
        }

        private void OnStateChange(MessageCenterMessage message) {
            if (message is GameStateChangedMessage g) {
                GameMode prevMode = GameModeAdapter.GameModeFromState(g.PreviousState);
                GameMode currentMode = GameModeAdapter.GameModeFromState(g.CurrentState);

                if (prevMode != currentMode) {
                    LoggerAdapter.Instance.Info($"Stop all processes due to mode change {prevMode} -> {currentMode}");
                    StopAll();
                }
            }
        }
    }
}
