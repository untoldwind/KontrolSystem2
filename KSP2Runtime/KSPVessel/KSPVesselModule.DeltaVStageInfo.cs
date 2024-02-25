using System.Linq;
using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("StageDeltaV")]
    public class DeltaVStageInfoAdapter : BaseDeltaVStageInfoAdapter {
        private readonly VesselAdapter vesselAdapter;

        internal DeltaVStageInfoAdapter(VesselAdapter vesselAdapter, DeltaVStageInfo deltaVStageInfo) : base(deltaVStageInfo) {
            this.vesselAdapter = vesselAdapter;
        }

        [KSField]
        public DeltaVEngineInfoAdapter[] Engines => deltaVStageInfo.EnginesInStage
            .Select(e => new DeltaVEngineInfoAdapter(vesselAdapter, e)).ToArray();

        [KSField]
        public DeltaVEngineInfoAdapter[] ActiveEngines => deltaVStageInfo.EnginesActiveInStage
            .Select(e => new DeltaVEngineInfoAdapter(vesselAdapter, e)).ToArray();

    }
}
