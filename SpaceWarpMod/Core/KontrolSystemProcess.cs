using System;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPGame;
using KSP.Sim.impl;

namespace KontrolSystem.SpaceWarpMod.Core {
    public enum KontrolSystemProcessState {
        Available,
        Running,
        Outdated,
        Error,
    }

    public class KontrolSystemProcess {
        private readonly IKontrolModule module;
        private KontrolSystemProcessState state;
        internal KSPContext context;
        public readonly Guid id;

        public KontrolSystemProcess(IKontrolModule module) {
            this.module = module;
            state = KontrolSystemProcessState.Available;
            id = Guid.NewGuid();
        }

        public string Name => module.Name;

        public KontrolSystemProcessState State => state;

        public void MarkRunning(KSPContext newContext) {
            state = KontrolSystemProcessState.Running;
            context?.Cleanup();
            context = newContext;
        }

        public KontrolSystemProcess MarkOutdated() {
            state = KontrolSystemProcessState.Outdated;
            return this;
        }

        public void MarkDone(string message) {
            if (!string.IsNullOrEmpty(message)) {
                LoggerAdapter.Instance.Info($"Process {id} for module {module.Name} terminated with: {message}");
                context.ConsoleBuffer.Print($"\n\n>>>>> ERROR <<<<<<<<<\n\nModule {module.Name} terminated with:\n{message}");
            }

            state = KontrolSystemProcessState.Available;
            context?.Cleanup();
            context = null;
        }

        public Entrypoint EntrypointFor(GameMode gameMode, IKSPContext newContext) {
            switch (gameMode) {
            case GameMode.KSC: return module.GetKSCEntrypoint(newContext);
            case GameMode.VAB:
                return module.GetEditorEntrypoint(newContext);
            case GameMode.Tracking: return module.GetTrackingEntrypoint(newContext);
            case GameMode.Flight:
                return module.GetFlightEntrypoint(newContext);
            default:
                return null;
            }
        }

        public bool AvailableFor(GameMode gameMode, VesselComponent vessel) {
            switch (gameMode) {
            case GameMode.KSC: return module.HasKSCEntrypoint();
            case GameMode.VAB: return module.HasEditorEntrypoint();
            case GameMode.Tracking: return module.HasTrackingEntrypoint();
            case GameMode.Flight:
                return !module.Name.StartsWith("boot::") && module.HasFlightEntrypoint() ||
                       module.IsBootFlightEntrypointFor(vessel);
            default:
                return false;
            }
        }

        public bool IsBootFor(GameMode gameMode, VesselComponent vessel) {
            switch (gameMode) {
            case GameMode.Flight: return module.IsBootFlightEntrypointFor(vessel);
            default: return false;
            }
        }
    }
}
