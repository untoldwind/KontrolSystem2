using System;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using KontrolSystem.TO2.Binding;
using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.State;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime {
    public interface IMarker {
        bool Visible { get; set; }
        void OnUpdate();

        void OnRender();
    }

    public interface IKSPAutopilot {
        void UpdateAutopilot(ref FlightCtrlState st, float deltaTime);
    }

    public interface IKSPContext : IContext {
        GameInstance Game { get; }

        GameMode GameMode { get; }

        VesselComponent ActiveVessel { get; }

        KSPConsoleBuffer ConsoleBuffer { get; }
        
        TimeSeriesCollection TimeSeriesCollection { get; }
        
        double UniversalTime { get; }

        KSPOrbitModule.IBody FindBody(string name);

        object NextYield { get; set; }

        Action OnNextYieldOnce { get; set; }

        void AddMarker(IMarker marker);

        void RemoveMarker(IMarker marker);

        void ClearMarkers();

        void TriggerMarkerUpdate();

        void TriggerMarkerRender();

        bool TryFindAutopilot<T>(VesselComponent vessel, out T autopilot) where T : IKSPAutopilot;

        void HookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot);

        void UnhookAutopilot(VesselComponent vessel, IKSPAutopilot autopilot);

        void UnhookAllAutopilots(VesselComponent vessel);
    }

    public class KSPContext {
        public static IKSPContext CurrentContext {
            get {
                IKSPContext context = ContextHolder.CurrentContext.Value as IKSPContext;
                if (context == null) throw new ArgumentException($"No current IKSPContext");
                return context;
            }
        }
    }
}
