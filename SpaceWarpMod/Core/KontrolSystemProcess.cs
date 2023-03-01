using System;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;
using KSP.Game;
using KSP.Sim.impl;
using SpaceWarp.API.Logging;
using UnityEngine;

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
            }

            state = KontrolSystemProcessState.Available;
            context?.Cleanup();
            context = null;
        }

        public Entrypoint EntrypointFor(GameState gameState, IKSPContext newContext) {
            switch (gameState) {
            case GameState.KerbalSpaceCenter: return module.GetKSCEntrypoint(newContext);
            case GameState.VehicleAssemblyBuilder:
                return module.GetEditorEntrypoint(newContext);
            case GameState.TrackingStation: return module.GetTrackingEntrypoint(newContext);
            case GameState.FlightView:
                return module.GetFlightEntrypoint(newContext);
            default:
                return null;
            }
        }

        public bool AvailableFor(GameState gameState, VesselComponent vessel) {
            switch (gameState) {
            case GameState.KerbalSpaceCenter: return module.HasKSCEntrypoint();
            case GameState.VehicleAssemblyBuilder: return module.HasEditorEntrypoint();
            case GameState.TrackingStation: return module.HasTrackingEntrypoint();
            case GameState.FlightView:
                return !module.Name.StartsWith("boot::") && module.HasFlightEntrypoint() ||
                       module.IsBootFlightEntrypointFor(vessel);
            default:
                return false;
            }
        }

        public bool IsBootFor(GameScenes gameScene, VesselComponent vessel) {
            switch (gameScene) {
            case GameScenes.FLIGHT: return module.IsBootFlightEntrypointFor(vessel);
            default: return false;
            }
        }
    }
}
