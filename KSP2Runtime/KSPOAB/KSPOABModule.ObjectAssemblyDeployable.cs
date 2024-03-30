using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyDeployable")]
    public class ObjectAssemblyDeployableAdapter(ObjectAssemblyPartAdapter part, Data_Deployable dataDeployable)
        : BaseDeployableAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataDeployable);
}
