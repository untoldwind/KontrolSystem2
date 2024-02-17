using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("Staging")]
    public class StagingAdapter {
        private readonly StagingComponent staging;
        private readonly VesselAdapter vesselAdapter;

        public StagingAdapter(VesselAdapter vesselAdapter) {
            this.vesselAdapter = vesselAdapter;
            staging = this.vesselAdapter.vessel.SimulationObject.Staging;
        }

        [KSField] public long Current => staging.StageCount > 0 ? staging.StageCount - 1 : 0;

        [KSField] public long Count => staging.StageCount;

        [KSField] public long TotalCount => staging.TotalStageCount;

        [KSField] public bool Ready => vesselAdapter.vessel.HasControlForEditingStagingStack();

        [KSMethod]
        public Future<bool> Next() {
            if (staging.StageCount > 0 && vesselAdapter.vessel.HasControlForEditingStagingStack()) {
                staging.ActivateNextStage();
                return new Future.Success<bool>(true);
            }

            return new Future.Success<bool>(false);
        }

        [KSMethod]
        public PartAdapter[] PartsInStage(long stage) {
            if (stage >= 0 && stage < staging.StageCount)
                return staging.GetPartsInStage((int)stage).Select(part => new PartAdapter(vesselAdapter, part))
                    .ToArray();

            return Array.Empty<PartAdapter>();
        }
    }
}
