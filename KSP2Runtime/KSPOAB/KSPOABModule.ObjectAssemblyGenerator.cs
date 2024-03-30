using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyGenerator")]
    public class ObjectAssemblyGeneratorAdapter(
        ObjectAssemblyPartAdapter part,
        Data_ModuleGenerator dataModuleGenerator)
        : BaseGeneratorAdatper<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataModuleGenerator);
}
