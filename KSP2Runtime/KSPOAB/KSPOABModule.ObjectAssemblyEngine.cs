using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyEngine")]
    public class ObjectAssemblyEngineAdapter(ObjectAssemblyPartAdapter part, Data_Engine dataSolarPanel)
        : BaseEngineAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataSolarPanel) {
        [KSField(Description = "Direction of thrust")]
        public Vector3d ThrustDirection => dataEngine.ThrustDirRelativePartWorldSpace;
    }
}
