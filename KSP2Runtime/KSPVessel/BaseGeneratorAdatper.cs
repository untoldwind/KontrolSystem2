using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseGeneratorAdatper<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected Data_ModuleGenerator dataModuleGenerator;

    public BaseGeneratorAdatper(P part, Data_ModuleGenerator dataModuleGenerator) : base(part) {
        this.dataModuleGenerator = dataModuleGenerator;
    }

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
