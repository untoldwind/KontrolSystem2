using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.KSP2.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Part")]
        public class PartAdapter {
            protected readonly VesselAdapter vesselAdapter;
            protected readonly PartComponent part;

            internal PartAdapter(VesselAdapter vesselAdapter, PartComponent part) {
                this.vesselAdapter = vesselAdapter;
                this.part = part;
            }

            [KSField] public VesselAdapter Vessel => vesselAdapter;

            [KSField] public string PartName => part.PartName;

            [KSField(Description = "Get position of the part in celestial frame of the main body.")]
            public Vector3d Position =>
                vesselAdapter.vessel.mainBody.coordinateSystem.ToLocalPosition(part.SimulationObject.Position);

            [KSField(Description = "Get coordinate independent position of the part.")] 
            public Position GlobalPosition => part.SimulationObject.Position;

            [KSField] public RotationWrapper GlobalRotation => new RotationWrapper(new Rotation(part.SimulationObject.transform.bodyFrame, ControlFacingRotation));

            [KSField] public bool IsEngine => part.IsPartEngine(out var _);

            [KSField] public long ActivationStage => part.ActivationStage;

            [KSField] public long DecoupleStage => part.DecoupleStage;

            [KSField]
            public KSPResourceModule.ResourceContainerAdapter Resources => new KSPResourceModule.ResourceContainerAdapter(part);

            [KSField]
            public Option<ModuleAirIntakeAdapter> AirIntake {
                get {
                    if (part.IsPartAirIntake(out Data_ResourceIntake data)) {
                        return new Option<ModuleAirIntakeAdapter>(new ModuleAirIntakeAdapter(part, data));
                    }

                    return new Option<ModuleAirIntakeAdapter>();
                }
            }

            [KSField]
            public Option<ModuleDockingNodeAdapter> DockingNode {
                get {
                    if (part.IsPartDockingPort(out Data_DockingNode data)) {
                        return new Option<ModuleDockingNodeAdapter>(new ModuleDockingNodeAdapter(vesselAdapter, part, data));
                    }

                    return new Option<ModuleDockingNodeAdapter>();
                }
            }

            [KSField]
            public Option<ModuleEngineAdapter> EngineModule {
                get {
                    if (part.IsPartEngine(out Data_Engine data)) {
                        return new Option<ModuleEngineAdapter>(new ModuleEngineAdapter(part, data));
                    }

                    return new Option<ModuleEngineAdapter>();
                }
            }

            [KSField]
            public Option<ModuleControlSurfaceAdapter> ControlSurface {
                get {
                    if (part.TryGetModuleData<PartComponentModule_ControlSurface, Data_ControlSurface>(out var data)) {
                        return new Option<ModuleControlSurfaceAdapter>(new ModuleControlSurfaceAdapter(part, data));
                    }

                    return new Option<ModuleControlSurfaceAdapter>();
                }
            }

            [KSField]
            public Option<ModuleCommandAdapter> CommandModule {
                get {
                    if (part.TryGetModuleData<PartComponentModule_Command, Data_Command>(out var data)) {
                        return new Option<ModuleCommandAdapter>(new ModuleCommandAdapter(vesselAdapter, part, data));
                    }

                    return new Option<ModuleCommandAdapter>();
                }
            }

            [KSField] public bool IsSolarPanel => part.IsPartSolarPanel(out var _);

            [KSField]
            public Option<ModuleSolarPanelAdapter> SolarPanel {
                get {
                    if (part.IsPartSolarPanel(out Data_SolarPanel data)) {
                        return new Option<ModuleSolarPanelAdapter>(new ModuleSolarPanelAdapter(part, data));
                    }

                    return new Option<ModuleSolarPanelAdapter>();
                }
            }

            [KSField] public bool IsFairing => part.TryGetModuleData<PartComponentModule_Fairing, Data_Fairing>(out var _);

            [KSField]
            public Option<ModuleFairingAdapter> Fairing {
                get {
                    if (part.TryGetModuleData<PartComponentModule_Fairing, Data_Fairing>(out Data_Fairing data)) {
                        return new Option<ModuleFairingAdapter>(new ModuleFairingAdapter(part, data));
                    }

                    return new Option<ModuleFairingAdapter>();
                }
            }

            [KSField] public bool IsDeployable => part.IsPartDeployable(out var _);

            [KSField]
            public Option<ModuleDeployableAdapter> Deployable {
                get {
                    if (part.IsPartDeployable(out Data_Deployable data)) {
                        return new Option<ModuleDeployableAdapter>(new ModuleDeployableAdapter(part, data));
                    }

                    return new Option<ModuleDeployableAdapter>();
                }
            }

            [KSField] public bool IsDecoupler => part.IsPartDecoupler(out var _);

            [KSField]
            public Option<ModuleDecouplerAdapter> Decoupler {
                get {
                    if (part.IsPartDecoupler(out Data_Decouple data)) {
                        return new Option<ModuleDecouplerAdapter>(new ModuleDecouplerAdapter(part, data));
                    }

                    return new Option<ModuleDecouplerAdapter>();
                }
            }

            [KSField] public bool IsLaunchClamp => part.IsPartLaunchClamp(out var _);

            [KSField]
            public Option<ModuleLaunchClampAdapter> LaunchClamp {
                get {
                    if (part.IsPartLaunchClamp(out Data_GroundLaunchClamp data)) {
                        return new Option<ModuleLaunchClampAdapter>(new ModuleLaunchClampAdapter(part, data));
                    }

                    return new Option<ModuleLaunchClampAdapter>();
                }
            }

            [KSField] public bool IsParachute => part.IsParachute(out var _);

            [KSField]
            public Option<ModuleParachuteAdapter> Parachute {
                get {
                    if (part.IsParachute(out Data_Parachute data)) {
                        return new Option<ModuleParachuteAdapter>(new ModuleParachuteAdapter(part, data));
                    }

                    return new Option<ModuleParachuteAdapter>();
                }
            }
        }
    }
}
