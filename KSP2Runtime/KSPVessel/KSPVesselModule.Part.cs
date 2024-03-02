using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.KSP2.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Iteration.UI.Binding;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("Part")]
    public class PartAdapter : BasePartAdapter<PartComponent> {
        protected readonly VesselAdapter vesselAdapter;

        internal PartAdapter(VesselAdapter vesselAdapter, PartComponent part) : base(part) {
            this.vesselAdapter = vesselAdapter;
        }

        [KSField] public VesselAdapter Vessel => vesselAdapter;

        [KSField(Description = "Get position of the part in celestial frame of the main body.")]
        public Vector3d Position =>
            vesselAdapter.vessel.mainBody.transform.celestialFrame.ToLocalPosition(part.SimulationObject.Position);

        [KSField(Description = "Get coordinate independent position of the part.")]
        public Position GlobalPosition => part.SimulationObject.Position;

        [KSField]
        public RotationWrapper GlobalRotation =>
            new(new Rotation(part.SimulationObject.transform.bodyFrame, ControlFacingRotation));

        [KSField(Description = "Indicate if the part has splashed")]
        public bool Splashed => part.Splashed;

        [KSField(Description = "Resource mass of the part")]
        public double ResourceMass => part.ResourceMass;

        [KSField(Description = "Temperature of the part")]
        public double Temperature => part.Temperature;

        [KSField(Description = "Maximum temperature of the part")]
        public double MaxTemperature => part.MaxTemp;

        [KSField] public double ThermalMass => part.ThermalMass;

        [KSField] public double ResourceThermalMass => part.ResourceThermalMass;

        [KSField] public KSPResourceModule.ResourceContainerAdapter Resources => new(part);

        [KSField]
        public Option<ModuleAirIntakeAdapter> AirIntake =>
            part.IsPartAirIntake(out var data)
                ? Option.Some(new ModuleAirIntakeAdapter(part, data))
                : Option.None<ModuleAirIntakeAdapter>();

        [KSField]
        public Option<ModuleDockingNodeAdapter> DockingNode =>
            part.IsPartDockingPort(out var data)
                ? Option.Some(new ModuleDockingNodeAdapter(vesselAdapter, part, data))
                : Option.None<ModuleDockingNodeAdapter>();

        [KSField] public bool IsEngine => part.IsPartEngine(out var _);

        [KSField]
        public Option<ModuleEngineAdapter> EngineModule =>
            part.IsPartEngine(out var data)
                ? Option.Some(new ModuleEngineAdapter(part, data, vesselAdapter))
                : Option.None<ModuleEngineAdapter>();

        [KSField]
        public Option<ModuleControlSurfaceAdapter> ControlSurface =>
            part.TryGetModuleData<PartComponentModule_ControlSurface, Data_ControlSurface>(out var data)
                ? Option.Some(new ModuleControlSurfaceAdapter(part, data))
                : Option.None<ModuleControlSurfaceAdapter>();

        [KSField]
        public Option<ModuleCommandAdapter> CommandModule =>
            part.TryGetModuleData<PartComponentModule_Command, Data_Command>(out var data)
                ? Option.Some(new ModuleCommandAdapter(vesselAdapter, part, data))
                : Option.None<ModuleCommandAdapter>();

        [KSField]
        public bool IsScienceExperiment =>
            part.TryGetModuleData<PartComponentModule_ScienceExperiment, Data_ScienceExperiment>(out var _);

        [KSField]
        public Option<ModuleScienceExperimentAdapter> ScienceExperiment =>
            part.TryGetModuleData<PartComponentModule_ScienceExperiment, Data_ScienceExperiment>(out var data)
                ? Option.Some(new ModuleScienceExperimentAdapter(part, data))
                : Option.None<ModuleScienceExperimentAdapter>();

        [KSField] public bool IsSolarPanel => part.IsPartSolarPanel(out var _);

        [KSField]
        public Option<ModuleSolarPanelAdapter> SolarPanel =>
            part.IsPartSolarPanel(out var data)
                ? Option.Some(new ModuleSolarPanelAdapter(part, data))
                : Option.None<ModuleSolarPanelAdapter>();

        [KSField] public bool IsFairing => part.TryGetModuleData<PartComponentModule_Fairing, Data_Fairing>(out var _);

        [KSField]
        public Option<ModuleFairingAdapter> Fairing =>
            part.TryGetModuleData<PartComponentModule_Fairing, Data_Fairing>(out var data)
                ? Option.Some(new ModuleFairingAdapter(part, data))
                : Option.None<ModuleFairingAdapter>();

        [KSField] public bool IsDeployable => part.IsPartDeployable(out var _);

        [KSField]
        public Option<ModuleDeployableAdapter> Deployable =>
            part.IsPartDeployable(out var data)
                ? Option.Some(new ModuleDeployableAdapter(part, data))
                : Option.None<ModuleDeployableAdapter>();

        [KSField] public bool IsDecoupler => part.IsPartDecoupler(out var _);

        [KSField]
        public Option<ModuleDecouplerAdapter> Decoupler =>
            part.IsPartDecoupler(out var data)
                ? Option.Some(new ModuleDecouplerAdapter(part, data))
                : Option.None<ModuleDecouplerAdapter>();

        [KSField] public bool IsLaunchClamp => part.IsPartLaunchClamp(out var _);

        [KSField]
        public Option<ModuleLaunchClampAdapter> LaunchClamp =>
            part.IsPartLaunchClamp(out var data)
                ? Option.Some(new ModuleLaunchClampAdapter(part, data))
                : Option.None<ModuleLaunchClampAdapter>();

        [KSField] public bool IsParachute => part.IsParachute(out var _);

        [KSField]
        public Option<ModuleParachuteAdapter> Parachute =>
            part.IsParachute(out var data)
                ? Option.Some(new ModuleParachuteAdapter(part, data))
                : Option.None<ModuleParachuteAdapter>();

        [KSField]
        public bool IsHeatshield => part.TryGetModuleData<PartComponentModule_Heatshield, Data_Heatshield>(out var _);

        [KSField]
        public Option<ModuleHeatshieldAdapter> Heatshield =>
            part.TryGetModuleData<PartComponentModule_Heatshield, Data_Heatshield>(out var data)
                ? Option.Some(new ModuleHeatshieldAdapter(part, data))
                : Option.None<ModuleHeatshieldAdapter>();

        [KSField] public bool IsCargoBay => part.TryGetModuleData<PartComponentModule_CargoBay, Data_CargoBay>(out var _);
    }
}
