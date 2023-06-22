using System;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.Core {
    public enum KontrolSystemProcessState {
        Available,
        Running,
        Outdated,
        Error,
    }

    public class KontrolSystemProcess {
        internal readonly ITO2Logger logger;
        private readonly IKontrolModule module;
        private KontrolSystemProcessState state;
        internal KSPCoreContext context;
        public readonly Guid id;

        public KontrolSystemProcess(ITO2Logger logger, IKontrolModule module) {
            this.logger = logger;
            this.module = module;
            state = KontrolSystemProcessState.Available;
            id = Guid.NewGuid();
        }

        public string Name => module.Name;

        public KontrolSystemProcessState State => state;

        public void MarkRunning(KSPCoreContext newContext) {
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
                logger.Info($"Process {id} for module {module.Name} terminated with: {message}");
                context.ConsoleBuffer.Print($"\n\n>>>>> ERROR <<<<<<<<<\n\nModule {module.Name} terminated with:\n{message}");
            }

            state = KontrolSystemProcessState.Available;
            context?.Cleanup();
            context = null;
        }

        public Entrypoint EntrypointFor(KSPGameMode gameMode, IKSPContext newContext) {
            switch (gameMode) {
            case KSPGameMode.KSC: return module.GetKSCEntrypoint(newContext);
            case KSPGameMode.VAB:
                return module.GetEditorEntrypoint(newContext);
            case KSPGameMode.Tracking: return module.GetTrackingEntrypoint(newContext);
            case KSPGameMode.Flight:
                return module.GetFlightEntrypoint(newContext);
            default:
                return null;
            }
        }

        public int EntrypointArgumentCount(KSPGameMode gameMode) => module.GetEntrypointArgumentCount(gameMode);

        public EntrypointArgumentDescriptor[] EntrypointArgumentDescriptors(KSPGameMode gameMode) => module.GetEntrypointParameterDescriptors(gameMode);

        public bool AvailableFor(KSPGameMode gameMode, VesselComponent vessel) {
            switch (gameMode) {
            case KSPGameMode.KSC: return module.HasKSCEntrypoint();
            case KSPGameMode.VAB: return module.HasEditorEntrypoint();
            case KSPGameMode.Tracking: return module.HasTrackingEntrypoint();
            case KSPGameMode.Flight:
                return !module.Name.StartsWith("boot::") && module.HasFlightEntrypoint() ||
                       module.IsBootFlightEntrypointFor(vessel);
            default:
                return false;
            }
        }

        public bool IsBootFor(KSPGameMode gameMode, VesselComponent vessel) {
            switch (gameMode) {
            case KSPGameMode.Flight: return module.IsBootFlightEntrypointFor(vessel);
            default: return false;
            }
        }
    }
}
