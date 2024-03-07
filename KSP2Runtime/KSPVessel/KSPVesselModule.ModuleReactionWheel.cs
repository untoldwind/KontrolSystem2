using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleReactionWheel")]
    public class ModuleReactionWheelAdapter : BaseReactionWheelAdapter<PartAdapter, PartComponent> {
        public ModuleReactionWheelAdapter(PartAdapter part, Data_ReactionWheel reactionWheel) : base(part, reactionWheel) {
        }
    }
}
