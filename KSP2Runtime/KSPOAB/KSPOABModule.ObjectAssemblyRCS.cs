using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyRCS")]
    public class ObjectAssemblyRCS(ObjectAssemblyPartAdapter part, Data_RCS dataRcs)
        : BaseRCSAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataRcs);
}
