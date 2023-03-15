using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim.DeltaV;
using KSP.Sim.impl;
using KSP.Sim.ResourceSystem;

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

            [KSField] public string PartName => part.PartName;

            [KSField] public bool IsEngine => part.IsPartEngine(out var _);

            [KSField]
            public ResourceContainerAdapter Resources => new ResourceContainerAdapter(part.PartResourceContainer);
            
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
                        return new Option<ModuleDockingNodeAdapter>(new ModuleDockingNodeAdapter(part, data));
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
            public Option<ModuleSolarPanelAdapter> SolarPanel {
                get {
                    if (part.IsPartSolarPanel(out Data_SolarPanel data)) {
                        return new Option<ModuleSolarPanelAdapter>(new ModuleSolarPanelAdapter(part, data));
                    }

                    return new Option<ModuleSolarPanelAdapter>();
                }
            }

            [KSField] public bool IsSolarPanel => part.IsPartSolarPanel(out var _);
        }
    }
}
