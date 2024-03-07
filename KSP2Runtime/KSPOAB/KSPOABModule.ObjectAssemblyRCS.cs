using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyRCS")]
    public class ObjectAssemblyRCS : BaseRCSAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart> {
        public ObjectAssemblyRCS(ObjectAssemblyPartAdapter part, Data_RCS dataRcs) : base(part, dataRcs) {
        }

    }
}
