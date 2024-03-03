using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyEngine")]
    public class ObjectAssemblyEngineAdapter : BaseEngineAdapter {
        public ObjectAssemblyEngineAdapter(Data_Engine dataSolarPanel) : base(dataSolarPanel) {
        }

        [KSField(Description = "Direction of thrust")]
        public Vector3d ThrustDirection => dataEngine.ThrustDirRelativePartWorldSpace;
    }
}
