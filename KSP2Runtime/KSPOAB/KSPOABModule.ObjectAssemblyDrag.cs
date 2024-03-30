using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyDrag")]
    public class ObjectAssemblyDragAdapter(ObjectAssemblyPartAdapter part, Data_Drag dataDrag)
        : BaseDragAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataDrag);
}
