using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyReactionWheel")]
    public class ObjectAssemblyReactionWheelAdapter : BaseReactionWheelAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart> {
        public ObjectAssemblyReactionWheelAdapter(ObjectAssemblyPartAdapter part, Data_ReactionWheel reactionWheel) : base(part, reactionWheel) {
        }
    }
}
