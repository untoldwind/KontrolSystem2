using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
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

            [KSField] public bool Ready => vesselAdapter.vessel.HasControlForEditingStagingStack();

            [KSMethod]
            public Future<bool> Next() {
                if (staging.StageCount > 0 && vesselAdapter.vessel.HasControlForEditingStagingStack()) return new Future.Success<bool>(false);
                staging.ActivateNextStage();
                return new Future.Success<bool>(true);
            }
        }
        
    }
}
