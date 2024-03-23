using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseAirIntakeAdapter<P, T>(P part, Data_ResourceIntake dataResourceIntake)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_ResourceIntake dataResourceIntake = dataResourceIntake;

    [KSField]
    public KSPResourceModule.ResourceDefinitionAdapter Resource => new(dataResourceIntake.ResourceDefinitionData);

    [KSField] public double ResourceUnits => dataResourceIntake.ResourceUnits;

    [KSField(Description = "Enable/disable module")]
    public bool Enabled => dataResourceIntake.ModuleEnabled;

    [KSField(Description = "Toggle air intake.")]
    public bool ToogleIntake {
        get => dataResourceIntake.toggleResourceIntake.GetValue();
        set => dataResourceIntake.toggleResourceIntake.SetValue(value);
    }

    [KSField(Description = "Resource flow rate")]
    public double FlowRate => dataResourceIntake.flowRate.GetValue();
}
