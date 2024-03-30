using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyDecoupler")]
    public class ObjectAssemblyDecouplerAdapter(ObjectAssemblyPartAdapter part, Data_Decouple dataDecouple)
        : BaseDecouplerAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataDecouple);
}
