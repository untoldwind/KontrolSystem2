using System.Linq;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.OAB;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyStageDeltaV")]
    public class ObjectAssemblyStageDeltaVAdapter(DeltaVStageInfo deltaVStageInfo) : BaseDeltaVStageInfoAdapter(deltaVStageInfo) {
        [KSField]
        public ObjectAssemblyPartAdapter[] Parts => deltaVStageInfo.Parts.Select(partInfo =>
            new ObjectAssemblyPartAdapter((IObjectAssemblyPart)partInfo.PartRef)).ToArray();

        [KSField]
        public ObjectAssemblyEngineDeltaV[] Engines => deltaVStageInfo.EnginesInStage
            .Select(e => new ObjectAssemblyEngineDeltaV(e)).ToArray();

        [KSField]
        public ObjectAssemblyEngineDeltaV[] ActiveEngines => deltaVStageInfo.EnginesActiveInStage
            .Select(e => new ObjectAssemblyEngineDeltaV(e)).ToArray();
    }
}
