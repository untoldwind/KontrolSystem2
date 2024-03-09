using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseGeneratorAdatper<P, T>(P part, Data_ModuleGenerator dataModuleGenerator)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected Data_ModuleGenerator dataModuleGenerator = dataModuleGenerator;

    [KSField]
    public double GeneratorOutput => dataModuleGenerator.GeneratorOutput.GetValue();

    [KSField]
    public bool Enabled {
        get => dataModuleGenerator.GeneratorEnabled.GetValue();
        set => dataModuleGenerator.GeneratorEnabled.SetValue(value);
    }

    [KSField] public bool IsAlwaysActive => dataModuleGenerator.IsAlwaysActive;

    [KSField]
    public KSPResourceModule.ResourceSettingAdapter ResourceSetting => new(dataModuleGenerator.ResourceSetting);
}
