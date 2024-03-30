using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyControlSurface")]
    public class ObjectAssemblyControlSurfaceAdapter(
        ObjectAssemblyPartAdapter part,
        Data_ControlSurface dataControlSurface)
        : BaseControlSurfaceAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataControlSurface);
}
