using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblySolarPanel")]
    public class ObjectAssemblySolarPanelAdapter : BaseSolarPanelAdapter {
        public ObjectAssemblySolarPanelAdapter(Data_SolarPanel dataSolarPanel) : base(dataSolarPanel) {
        }
    }
}
