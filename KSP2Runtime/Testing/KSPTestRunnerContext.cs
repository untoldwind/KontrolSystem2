using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.Testing {
    public class KSPTestRunnerContext : TestRunnerContext, IKSPContext {
        private KSPConsoleBuffer consoleBuffer = new KSPConsoleBuffer(50, 80);
        public GameMode GameMode => GameMode.Unknown;
        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;
        public double UniversalTime => 0;
        public KSPOrbitModule.IBody FindBody(string name) {
            return null;
        }

        public object NextYield { get; set; }
        
        public Action OnNextYieldOnce { get; set; }
        
        public void AddMarker(IMarker marker) {
        }

        public void RemoveMarker(IMarker marker) {
        }

        public void ClearMarkers() {
        }
        
        public void HookAutopilot(VesselComponent vessel, FlightInputCallback autopilot) {
        }

        public void UnhookAutopilot(VesselComponent vessel, FlightInputCallback autopilot) {
        }

        public void UnhookAllAutopilots(VesselComponent vessel) {
        }
    }
}
