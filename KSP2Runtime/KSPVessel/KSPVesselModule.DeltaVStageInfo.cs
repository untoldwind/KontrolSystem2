using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;
using System.Linq;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("StageDeltaV")]
        public class DeltaVStageInfoAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly DeltaVStageInfo deltaVStageInfo;

            internal DeltaVStageInfoAdapter(VesselAdapter vesselAdapter, DeltaVStageInfo deltaVStageInfo) {
                this.vesselAdapter = vesselAdapter;
                this.deltaVStageInfo = deltaVStageInfo;
            }

            [KSField(Description = "The stage number.")]
            public long Stage => deltaVStageInfo.Stage;

            [KSField(Description = "Estimated burn time of the stage.")]
            public double BurnTime => deltaVStageInfo.StageBurnTime;

            [KSMethod("get_deltav", Description = "Estimated delta-v of the stage in a given `situation`")]
            public double GetDeltaV(string situation) =>
                deltaVStageInfo.GetSituationDeltaV(SituationFromString(situation));

            [KSField(Description = "Start mass of the stage.")]
            public double StartMass => deltaVStageInfo.StartMass;

            [KSField(Description = "End mass of the stage.")]
            public double EndMass => deltaVStageInfo.EndMass;

            [KSField(Description = "Mass of the fuel in the stage.")]
            public double FuelMass => deltaVStageInfo.FuelMass;

            [KSField(Description = "Dry mass of the stage.")]
            public double DryMass => deltaVStageInfo.DryMass;

            [KSMethod("get_ISP", Description = "Estimated ISP of the stage in a given `situation`")]
            public double GetIsp(string situation) => deltaVStageInfo.GetSituationISP(SituationFromString(situation));

            [KSMethod("get_TWR", Description = "Estimated TWR of the stage in a given `situation`")]
            public double GetTwr(string situation) => deltaVStageInfo.GetSituationTWR(SituationFromString(situation));

            [KSMethod(Description = "Estimated thrust of the stage in a given `situation`")]
            public double GetThrust(string situation) =>
                deltaVStageInfo.GetSituationThrust(SituationFromString(situation));

            [KSField]
            public DeltaVEngineInfoAdapter[] Engines => deltaVStageInfo.EnginesInStage
                .Select(e => new DeltaVEngineInfoAdapter(vesselAdapter, e)).ToArray();

            [KSField]
            public DeltaVEngineInfoAdapter[] ActiveEngines => deltaVStageInfo.EnginesActiveInStage
                .Select(e => new DeltaVEngineInfoAdapter(vesselAdapter, e)).ToArray();


        }
    }
}
