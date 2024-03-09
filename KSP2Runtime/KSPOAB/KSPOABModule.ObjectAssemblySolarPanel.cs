using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblySolarPanel")]
    public class ObjectAssemblySolarPanelAdapter(KSPOABModule.ObjectAssemblyPartAdapter part, Data_SolarPanel dataSolarPanel) : BaseSolarPanelAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, dataSolarPanel) {
    }
}
