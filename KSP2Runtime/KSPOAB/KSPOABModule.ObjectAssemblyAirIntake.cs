using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyAirIntake")]
    public class ObjectAssemblyAirIntakeAdapter(ObjectAssemblyPartAdapter part, Data_ResourceIntake dataResourceIntake)
        : BaseAirIntakeAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataResourceIntake);
}
