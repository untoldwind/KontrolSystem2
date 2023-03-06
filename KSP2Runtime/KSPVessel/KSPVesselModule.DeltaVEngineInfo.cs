using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("EngineDeltaV")]
        public class DeltaVEngineInfoAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly DeltaVEngineInfo deltaVEngineInfo;

            internal DeltaVEngineInfoAdapter(VesselAdapter vesselAdapter, DeltaVEngineInfo deltaVEngineInfo) {
                this.vesselAdapter = vesselAdapter;
                this.deltaVEngineInfo = deltaVEngineInfo;
            }

            [KSMethod("get_ISP", Description = "Estimated ISP of the engine in a given `situation`")]
            public double GetIsp(string situation) => deltaVEngineInfo.GetSituationISP(SituationFromString(situation));

            [KSMethod(Description = "Estimated thrust of the engine in a given `situation`")]
            public double GetThrust(string situation) =>
                deltaVEngineInfo.GetSituationThrust(SituationFromString(situation));

            [KSMethod(Description = "Estimated thrust vector of the engine in a given `situation`")]
            public Vector3d GetThrustVector(string situation) =>
                deltaVEngineInfo.GetSituationThrustVector(SituationFromString(situation));

            [KSField(Description = "Number of the stage when engine is supposed to start")]
            public long StartBurnStage => deltaVEngineInfo.StartBurnStage;
        }
    }
}
