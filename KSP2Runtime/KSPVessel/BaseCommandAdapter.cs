using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.Definitions;
using KSP.Sim.DeltaV;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseCommandAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    private readonly Data_Command dataCommand;

    protected BaseCommandAdapter(P part, Data_Command dataCommand) : base(part) {
        this.dataCommand = dataCommand;
    }

    [KSField] public CommandControlState ControlState => dataCommand.controlStatus.GetValue();

    [KSField] public bool HasHibernation => dataCommand.hasHibernation;

    [KSField] public bool IsHibernating => dataCommand.IsHibernating;

    [KSField] public double HibernationMultiplier => dataCommand.hibernationMultiplier;

    [KSField]
    public KSPVesselModule.ResourceSettingAdapter[] RequiredResources =>
        dataCommand.requiredResources.Select(settings => new KSPVesselModule.ResourceSettingAdapter(settings)).ToArray();
}
