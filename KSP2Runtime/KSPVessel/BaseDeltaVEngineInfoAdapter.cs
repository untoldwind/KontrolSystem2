using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseDeltaVEngineInfoAdapter {
    protected readonly DeltaVEngineInfo deltaVEngineInfo;

    protected BaseDeltaVEngineInfoAdapter(DeltaVEngineInfo deltaVEngineInfo) {
        this.deltaVEngineInfo = deltaVEngineInfo;
    }

    [KSField(Description = "Number of the stage when engine is supposed to start")]
    public long StartBurnStage => deltaVEngineInfo.StartBurnStage;


    [KSMethod("get_ISP", Description = "Estimated ISP of the engine in a given `situation`")]
    public double GetIsp(DeltaVSituationOptions situation) {
        return deltaVEngineInfo.GetSituationISP(situation);
    }

    [KSMethod(Description = "Estimated thrust of the engine in a given `situation`")]
    public double GetThrust(DeltaVSituationOptions situation) {
        return deltaVEngineInfo.GetSituationThrust(situation);
    }
}
