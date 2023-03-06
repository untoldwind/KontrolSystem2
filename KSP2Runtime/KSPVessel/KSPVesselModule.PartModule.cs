using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("PartModule")]
        public class PartModuleAdapter {
            protected readonly VesselAdapter vesselAdapter;
            protected readonly PartComponentModule partModule;

            internal PartModuleAdapter(VesselAdapter vesselAdapter, PartComponentModule partModule) {
                this.vesselAdapter = vesselAdapter;
                this.partModule = partModule;
            }
        }
    }
}
