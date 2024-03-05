using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleTransmitter")]
    public class ModuleTransmitterAdapter : BaseTransmitterAdapter<PartAdapter, PartComponent> {
        public ModuleTransmitterAdapter(PartAdapter part, Data_Transmitter dataTransmitter) : base(part, dataTransmitter) {
        }
    }
}
