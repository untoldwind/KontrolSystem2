using KontrolSystem.TO2.Binding;
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
        }
    }
}
