using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyLight")]
    public class ObjectAssemblyLightAdapter(ObjectAssemblyPartAdapter part, Data_Light dataLight)
        : BaseLightAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataLight);
}
