using System;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleEngine")]
    public class ModuleEngineAdapter {
        private readonly Data_Engine dataEngine;
        private readonly PartComponent part;

        public ModuleEngineAdapter(PartComponent part, Data_Engine dataEngine) {
            this.part = part;
            this.dataEngine = dataEngine;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";

        [KSField] public bool IsShutdown => dataEngine.EngineShutdown;

        [KSField] public bool HasIgnited => dataEngine.EngineIgnited;

        [KSField] public bool IsFlameout => dataEngine.Flameout;

        [KSField] public bool IsStaged => dataEngine.staged;

        [KSField] public bool IsOperational => dataEngine.IsOperational;

        [KSField] public bool IsPropellantStarved => dataEngine.IsPropellantStarved;

        [KSField] public double CurrentThrottle => dataEngine.currentThrottle;

        [KSField] public double CurrentThrust => dataEngine.FinalThrustValue;

        [KSField] public double RealIsp => dataEngine.RealISPValue;

        [KSField] public double ThrottleMin => dataEngine.ThrottleMin;

        [KSField] public double MinFuelFlow => dataEngine.MinFuelFlow;

        [KSField] public double MaxFuelFlow => dataEngine.MaxFuelFlow;

        [KSField] public double MaxThrustOutputVac => dataEngine.MaxThrustOutputVac();

        [KSField] public double MaxThrustOutputAtm => dataEngine.MaxThrustOutputAtm();

        [KSField]
        public EngineModeAdapter[] EngineModes => dataEngine.engineModes
            .Select(engineMode => new EngineModeAdapter(engineMode)).ToArray();

        [KSField]
        public EngineModeAdapter CurrentEngineMode =>
            new(dataEngine.engineModes[dataEngine.currentEngineModeIndex]);

        [KSMethod]
        public bool ChangeMode(string name) {
            if (part == null) return false;

            var idx = Array.FindIndex(dataEngine.engineModes,
                engineMode => engineMode.engineID.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (idx < 0 || idx == dataEngine.currentEngineModeIndex) return false;

            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Engine>(out var moduleEngine)) return false;

            moduleEngine.ChangeEngineMode(idx);

            return true;
        }
    }
}
