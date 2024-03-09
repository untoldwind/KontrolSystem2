using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleTransmitter")]
    public class ModuleTransmitterAdapter(KSPVesselModule.PartAdapter part, Data_Transmitter dataTransmitter) : BaseTransmitterAdapter<PartAdapter, PartComponent>(part, dataTransmitter) {
    }
}
