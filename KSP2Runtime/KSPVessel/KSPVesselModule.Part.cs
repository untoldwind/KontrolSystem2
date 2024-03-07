using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.KSP2.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("Part")]
    public class PartAdapter : BasePartAdapter<PartComponent> {
        internal readonly VesselAdapter vesselAdapter;

        internal PartAdapter(VesselAdapter vesselAdapter, PartComponent part) : base(part) {
            this.vesselAdapter = vesselAdapter;
        }

        [KSField] public PartCategories PartCategory => part.PartData.category;

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
                ? Option.Some(new ModuleAirIntakeAdapter(this, data))
                : Option.None<ModuleAirIntakeAdapter>();

        [KSField]
        public Option<ModuleDockingNodeAdapter> DockingNode =>
            part.IsPartDockingPort(out var data)
                ? Option.Some(new ModuleDockingNodeAdapter(this, data))
                : Option.None<ModuleDockingNodeAdapter>();

        [KSField] public bool IsEngine => part.IsPartEngine(out _);

        [KSField]
        public Option<ModuleEngineAdapter> EngineModule =>
            part.IsPartEngine(out var data)
                ? Option.Some(new ModuleEngineAdapter(this, data))
                : Option.None<ModuleEngineAdapter>();

        [KSField]
        public Option<ModuleControlSurfaceAdapter> ControlSurface =>
            part.TryGetModuleData<PartComponentModule_ControlSurface, Data_ControlSurface>(out var data)
                ? Option.Some(new ModuleControlSurfaceAdapter(this, data))
                : Option.None<ModuleControlSurfaceAdapter>();

        [KSField]
        public Option<ModuleCommandAdapter> CommandModule =>
            part.TryGetModuleData<PartComponentModule_Command, Data_Command>(out var data)
                ? Option.Some(new ModuleCommandAdapter(this, data))
                : Option.None<ModuleCommandAdapter>();

        [KSField]
        public bool IsScienceExperiment =>
            part.TryGetModuleData<PartComponentModule_ScienceExperiment, Data_ScienceExperiment>(out _);

        [KSField]
        public Option<ModuleScienceExperimentAdapter> ScienceExperiment =>
            part.TryGetModuleData<PartComponentModule_ScienceExperiment, Data_ScienceExperiment>(out var data)
                ? Option.Some(new ModuleScienceExperimentAdapter(this, data))
                : Option.None<ModuleScienceExperimentAdapter>();

        [KSField] public bool IsSolarPanel => part.IsPartSolarPanel(out _);

        [KSField]
        public Option<ModuleSolarPanelAdapter> SolarPanel =>
            part.IsPartSolarPanel(out var data)
                ? Option.Some(new ModuleSolarPanelAdapter(this, data))
                : Option.None<ModuleSolarPanelAdapter>();

        [KSField] public bool IsFairing => part.PartData.category == PartCategories.Payload && part.TryGetModuleData<PartComponentModule_Fairing, Data_Fairing>(out _);

        [KSField]
        public Option<ModuleFairingAdapter> Fairing =>
            part.PartData.category == PartCategories.Payload && part.TryGetModuleData<PartComponentModule_Fairing, Data_Fairing>(out var data)
                ? Option.Some(new ModuleFairingAdapter(this, data))
                : Option.None<ModuleFairingAdapter>();

        [KSField] public bool IsDeployable => part.IsPartDeployable(out _);

        [KSField]
        public Option<ModuleDeployableAdapter> Deployable =>
            part.IsPartDeployable(out var data)
                ? Option.Some(new ModuleDeployableAdapter(this, data))
                : Option.None<ModuleDeployableAdapter>();

        [KSField] public bool IsDecoupler => part.IsPartDecoupler(out _);

        [KSField]
        public Option<ModuleDecouplerAdapter> Decoupler =>
            part.IsPartDecoupler(out var data)
                ? Option.Some(new ModuleDecouplerAdapter(this, data))
                : Option.None<ModuleDecouplerAdapter>();

        [KSField] public bool IsLaunchClamp => part.IsPartLaunchClamp(out _);

        [KSField]
        public Option<ModuleLaunchClampAdapter> LaunchClamp =>
            part.IsPartLaunchClamp(out var data)
                ? Option.Some(new ModuleLaunchClampAdapter(this, data))
                : Option.None<ModuleLaunchClampAdapter>();

        [KSField] public bool IsParachute => part.IsParachute(out _);

        [KSField]
        public Option<ModuleParachuteAdapter> Parachute =>
            part.IsParachute(out var data)
                ? Option.Some(new ModuleParachuteAdapter(this, data))
                : Option.None<ModuleParachuteAdapter>();

        [KSField]
        public bool IsHeatshield => part.TryGetModuleData<PartComponentModule_Heatshield, Data_Heatshield>(out _);

        [KSField]
        public Option<ModuleHeatshieldAdapter> Heatshield =>
            part.TryGetModuleData<PartComponentModule_Heatshield, Data_Heatshield>(out var data)
                ? Option.Some(new ModuleHeatshieldAdapter(this, data))
                : Option.None<ModuleHeatshieldAdapter>();

        [KSField] public bool IsCargoBay => part.TryGetModuleData<PartComponentModule_CargoBay, Data_CargoBay>(out _);

        [KSField]
        public bool IsTransmitter =>
            part.TryGetModuleData<PartComponentModule_DataTransmitter, Data_Transmitter>(out _);

        [KSField]
        public Option<ModuleTransmitterAdapter> Transmitter =>
            part.TryGetModuleData<PartComponentModule_DataTransmitter, Data_Transmitter>(out var data)
                ? Option.Some(new ModuleTransmitterAdapter(this, data))
                : Option.None<ModuleTransmitterAdapter>();

        [KSField]
        public bool IsLight =>
            part.TryGetModuleData<PartComponentModule_Light, Data_Light>(out _);

        [KSField]
        public Option<ModuleLightAdapter> Light =>
            part.TryGetModuleData<PartComponentModule_Light, Data_Light>(out var data)
                ? Option.Some(new ModuleLightAdapter(this, data))
                : Option.None<ModuleLightAdapter>();

        [KSField]
        public bool IsGenerator =>
            part.TryGetModuleData<PartComponentModule_Generator, Data_ModuleGenerator>(out _);

        [KSField]
        public Option<ModuleGeneratorAdapter> Generator =>
            part.TryGetModuleData<PartComponentModule_Generator, Data_ModuleGenerator>(out var data)
                ? Option.Some(new ModuleGeneratorAdapter(this, data))
                : Option.None<ModuleGeneratorAdapter>();

        [KSField("is_rcs")]
        public bool IsRCS =>
            part.TryGetModuleData<PartComponentModule_RCS, Data_RCS>(out _);

        [KSField("rcs")]
        public Option<ModuleRCSAdapter> RCS =>
            part.TryGetModuleData<PartComponentModule_RCS, Data_RCS>(out var data)
                ? Option.Some(new ModuleRCSAdapter(this, data))
                : Option.None<ModuleRCSAdapter>();

    }
}
