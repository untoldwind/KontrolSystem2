using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.OAB;

namespace KontrolSystem.KSP.Runtime.KSPOAB;

public partial class KSPOABModule {
    [KSClass("ObjectAssemblyReactionWheel")]
    public class ObjectAssemblyReactionWheelAdapter(ObjectAssemblyPartAdapter part, Data_ReactionWheel reactionWheel)
        : BaseReactionWheelAdapter<ObjectAssemblyPartAdapter, IObjectAssemblyPart>(part, reactionWheel);
}
