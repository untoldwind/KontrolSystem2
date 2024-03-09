using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleGenerator")]
    public class ModuleGeneratorAdapter(KSPVesselModule.PartAdapter part, Data_ModuleGenerator dataModuleGenerator) : BaseGeneratorAdatper<PartAdapter, PartComponent>(part, dataModuleGenerator) {
    }
}
