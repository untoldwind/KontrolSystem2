using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyLiftingSurface")]
    public class ObjectAssemblyLiftingSurfaceAdapter(
        ObjectAssemblyPartAdapter part,
        Data_LiftingSurface dataLiftingSurface)
        : BaseLiftingSurfaceAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataLiftingSurface);
}
