﻿using System;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KSP.Game;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime {
    public interface IMarker {
        bool Visible { get; set; }
        void Update();
        void OnRender();
    }

    public interface IKSPContext : IContext {
        GameInstance Game { get; }
        
        GameMode GameMode { get; }

        VesselComponent ActiveVessel { get; }

        KSPConsoleBuffer ConsoleBuffer { get; }

        double UniversalTime { get; }

        KSPOrbitModule.IBody FindBody(string name);

        object NextYield { get; set; }

        Action OnNextYieldOnce { get; set; }

        void AddMarker(IMarker marker);

        void RemoveMarker(IMarker marker);

        void ClearMarkers();

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
