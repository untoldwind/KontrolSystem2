using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleReactionWheel")]
    public class ModuleReactionWheelAdapter(PartAdapter part, Data_ReactionWheel reactionWheel) : BaseReactionWheelAdapter<PartAdapter, PartComponent>(part, reactionWheel) {
    }
}
