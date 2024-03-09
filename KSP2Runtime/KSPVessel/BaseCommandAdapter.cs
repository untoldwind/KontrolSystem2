using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.Definitions;
using KSP.Sim.DeltaV;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseCommandAdapter<P, T>(P part, Data_Command dataCommand) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    [KSField] public CommandControlState ControlState => dataCommand.controlStatus.GetValue();

    [KSField] public bool HasHibernation => dataCommand.hasHibernation;

    [KSField] public bool IsHibernating => dataCommand.IsHibernating;

    [KSField] public double HibernationMultiplier => dataCommand.hibernationMultiplier;

    [KSField]
    public KSPResourceModule.ResourceSettingAdapter[] RequiredResources =>
        dataCommand.requiredResources.Select(settings => new KSPResourceModule.ResourceSettingAdapter(settings)).ToArray();
}
