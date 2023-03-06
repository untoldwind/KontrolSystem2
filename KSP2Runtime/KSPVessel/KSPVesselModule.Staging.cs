using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Staging")]
        public class StagingAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly StagingComponent staging;

            public StagingAdapter(VesselAdapter vesselAdapter) {
                this.vesselAdapter = vesselAdapter;
                staging = this.vesselAdapter.vessel.SimulationObject.Staging;
            }

            [KSField] public long Count => staging.StageCount;
            
            [KSField] public long TotalCount => staging.TotalStageCount;
        }
        
    }
}
