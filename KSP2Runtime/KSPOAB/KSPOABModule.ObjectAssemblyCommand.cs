using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyCommand")]
    public class ObjectAssemblyCommandAdapter(ObjectAssemblyPartAdapter part, Data_Command dataCommand)
        : BaseCommandAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataCommand);
}
