using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
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

            [KSField] public string PartName => part.PartName;

            [KSField] public bool IsEngine => part.IsPartEngine(out var _);

            [KSField]
            public Option<EngineModuleAdapter> EngineModule {
                get {
                    if (part.IsPartEngine(out Data_Engine data)) {
                        return new Option<EngineModuleAdapter>(new EngineModuleAdapter(part, data));
                    }

                    return new Option<EngineModuleAdapter>();
                }
            }

            [KSField] public bool IsSolarPanel => part.IsPartSolarPanel(out var _);
        }
    }
}
