using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyEngineDeltaV")]
    public class ObjectAssemblyEngineDeltaV : BaseDeltaVEngineInfoAdapter {
        public ObjectAssemblyEngineDeltaV(DeltaVEngineInfo deltaVEngineInfo) : base(deltaVEngineInfo) {
        }
    }
}
