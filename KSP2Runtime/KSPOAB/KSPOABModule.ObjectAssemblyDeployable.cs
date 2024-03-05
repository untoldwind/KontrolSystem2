using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyDeployable")]
    public class ObjectAssemblyDeployableAdapter : BaseDeployableAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart> {
        public ObjectAssemblyDeployableAdapter(ObjectAssemblyPartAdapter part, Data_Deployable dataDeployable) : base(part, dataDeployable) {
        }
    }
}
