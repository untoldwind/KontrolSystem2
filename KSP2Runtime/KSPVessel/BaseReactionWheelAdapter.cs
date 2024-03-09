using System.Linq;
using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseReactionWheelAdapter<P, T>(P part, Data_ReactionWheel reactionWheel)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_ReactionWheel reactionWheel = reactionWheel;

    [KSField]
    public double WheelAuthority {
        get => reactionWheel.WheelAuthority.GetValue();
        set => reactionWheel.WheelAuthority.SetValue((float)value);
    }

    [KSField]
    public bool ToggleTorque {
        get => reactionWheel.ToggleTorque.GetValue();
        set => reactionWheel.ToggleTorque.SetValue(value);
    }

    [KSField]
    public Data_ReactionWheel.ReactionWheelState WheelState => reactionWheel.WheelState;

    [KSField]
    public Data_ReactionWheel.ActuatorModes WheelActuatorMode {
        get => reactionWheel.WheelActuatorMode.GetValue();
        set => reactionWheel.WheelActuatorMode.SetValue(value);
    }

    [KSField]
    public bool HasResourcesToOperate => reactionWheel.HasResourcesToOperate;

    [KSField]
    public KSPResourceModule.ResourceSettingAdapter[] RequiredResources =>
        reactionWheel.RequiredResources.Select(settings => new KSPResourceModule.ResourceSettingAdapter(settings)).ToArray();

}
