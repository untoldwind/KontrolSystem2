using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleRCS")]
    public class ModuleRCSAdapter : BaseRCSAdapter<PartAdapter, PartComponent> {
        public ModuleRCSAdapter(PartAdapter part, Data_RCS dataRcs) : base(part, dataRcs) {
        }
    }

}
