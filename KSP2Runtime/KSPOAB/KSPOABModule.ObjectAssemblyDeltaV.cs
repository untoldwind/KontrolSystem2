using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyDeltaV", Description = "Delta V information of an object assembly")]
    public class ObjectAssemblyDeltaVAdapter(VesselDeltaVComponent vesselDeltaVComponent) {
        [KSField]
        public ObjectAssemblyStageDeltaVAdapter[] Stages => vesselDeltaVComponent.StageInfo
            .Select(stage => new ObjectAssemblyStageDeltaVAdapter(stage)).ToArray();

        [KSMethod(Description = "Get delta-v information for a specific `stage` of the object assembly, if existent.")]
        public Option<ObjectAssemblyStageDeltaVAdapter> Stage(long stage) {
            var stageInfo = vesselDeltaVComponent.GetStage((int)stage);

            return stageInfo != null
                ? Option.Some(new ObjectAssemblyStageDeltaVAdapter(stageInfo))
                : Option.None<ObjectAssemblyStageDeltaVAdapter>();
        }
    }
}
