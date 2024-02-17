using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("VesselDeltaV")]
    public class VesselDeltaVAdapter {
        private readonly VesselDeltaVComponent deltaV;
        private readonly VesselAdapter vesselAdapter;

        public VesselDeltaVAdapter(VesselAdapter vesselAdapter) {
            this.vesselAdapter = vesselAdapter;
            deltaV = vesselAdapter.vessel.VesselDeltaV;
        }

        [KSField]
        public DeltaVStageInfoAdapter[] Stages => deltaV.StageInfo
            .Select(stage => new DeltaVStageInfoAdapter(vesselAdapter, stage)).ToArray();

        [KSMethod(Description = "Get delta-v information for a specific `stage` of the vessel, if existent.")]
        public Option<DeltaVStageInfoAdapter> Stage(long stage) {
            var stageInfo = deltaV.GetStage((int)stage);

            return stageInfo != null
                ? new Option<DeltaVStageInfoAdapter>(new DeltaVStageInfoAdapter(vesselAdapter, stageInfo))
                : new Option<DeltaVStageInfoAdapter>();
        }
    }
}
