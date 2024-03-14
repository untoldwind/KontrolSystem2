using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.OAB;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyEngineDeltaV")]
    public class ObjectAssemblyEngineDeltaV(DeltaVEngineInfo deltaVEngineInfo) : BaseDeltaVEngineInfoAdapter(deltaVEngineInfo) {
        [KSField]
        public ObjectAssemblyPartAdapter Part => new((IObjectAssemblyPart)deltaVEngineInfo.Part);

        [KSField]
        public ObjectAssemblyEngineAdapter Engine => new(new ObjectAssemblyPartAdapter((IObjectAssemblyPart)deltaVEngineInfo.Part), deltaVEngineInfo.Engine);
    }
}
