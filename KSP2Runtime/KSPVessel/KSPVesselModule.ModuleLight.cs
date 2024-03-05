using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleLight")]
    public class ModuleLightAdapter : BaseLightAdapter<PartAdapter, PartComponent> {
        public ModuleLightAdapter(PartAdapter part, Data_Light dataLight) : base(part, dataLight) {
        }

    }
}
