using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyDockingNode")]
    public class ObjectAssemblyDockingNodeAdapter(ObjectAssemblyPartAdapter part, Data_DockingNode dataDockingNode)
        : BaseDockingNodeAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataDockingNode);
}
