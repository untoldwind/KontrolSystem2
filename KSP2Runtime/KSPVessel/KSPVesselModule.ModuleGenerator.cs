using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleGenerator")]
    public class ModuleGeneratorAdapter : BaseGeneratorAdatper<PartAdapter, PartComponent> {
        public ModuleGeneratorAdapter(PartAdapter part, Data_ModuleGenerator dataModuleGenerator) : base(part, dataModuleGenerator) {
        }
    }
}
