using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleEngine")]
    public class ModuleEngineAdapter : BaseEngineAdapter {
        private readonly PartComponent part;
        private readonly VesselAdapter vesselAdapter;

        public ModuleEngineAdapter(PartComponent part, Data_Engine dataEngine, VesselAdapter vesselAdapter) : base(dataEngine) {
            this.part = part;
            this.vesselAdapter = vesselAdapter;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";

        [KSField(Description = "Direction of thrust in the celestial frame of the main body")]
        public Vector3d ThrustDirection =>
            vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalVector(
                KSPContext.CurrentContext.Game.UniverseView.PhysicsSpace.PhysicsToVector(
                    dataEngine.ThrustDirRelativePartWorldSpace));

        [KSField(Description = "Coordinate independent direction of thrust.")]
        public Vector GlobalThrustDirection =>
            KSPContext.CurrentContext.Game.UniverseView.PhysicsSpace.PhysicsToVector(
                dataEngine.ThrustDirRelativePartWorldSpace);

        [KSField]
        public EngineModeAdapter[] EngineModes => dataEngine.engineModes
            .Select(engineMode => new EngineModeAdapter(engineMode)).ToArray();

        [KSField] public bool IsGimbal => part.TryGetModuleData<PartComponentModule_Gimbal, Data_Gimbal>(out var _);

        [KSField]
        public Option<ModuleGimbalAdapter> Gimbal =>
            part.TryGetModuleData<PartComponentModule_Gimbal, Data_Gimbal>(out var data)
                ? Option.Some(new ModuleGimbalAdapter(part, data))
                : Option.None<ModuleGimbalAdapter>();

        [KSField]
        public EngineModeAdapter CurrentEngineMode =>
            new(dataEngine.engineModes[dataEngine.currentEngineModeIndex]);

        [KSField]
        public KSPResourceModule.ResourceDefinitionAdapter CurrentPropellant =>
            new(dataEngine.CurrentPropellantState.resourceDef);

        [KSField]
        public KSPResourceModule.ResourceDefinitionAdapter[] Propellants =>
            dataEngine.PropellantStates
                .Select(state => new KSPResourceModule.ResourceDefinitionAdapter(state.resourceDef)).ToArray();

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
