using System;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime {
    public interface IMarker {
        bool Visible { get; set; }
        void Update();
        void OnRender();
    }

    public interface IFixedUpdateObserver {
        void OnFixedUpdate(double deltaTime);
    }

    public interface IKSPContext : IContext {
        GameScenes CurrentScene { get; }

        KSPConsoleBuffer ConsoleBuffer { get; }

        object NextYield { get; set; }

        void AddMarker(IMarker marker);

        void RemoveMarker(IMarker marker);

        void ClearMarkers();

        void AddFixedUpdateObserver(IFixedUpdateObserver observer);

        void HookAutopilot(VesselComponent vessel, FlightInputCallback autopilot);

        void UnhookAutopilot(VesselComponent vessel, FlightInputCallback autopilot);

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
