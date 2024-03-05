using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyLight")]
    public class ObjectAssemblyLightAdapter : BaseLightAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart> {
        public ObjectAssemblyLightAdapter(ObjectAssemblyPartAdapter part, Data_Light dataLight) : base(part, dataLight) {
        }
    }
}
