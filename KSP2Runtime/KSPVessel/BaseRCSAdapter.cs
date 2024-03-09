using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseRCSAdapter<P, T>(P part, Data_RCS dataRcs) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_RCS dataRcs = dataRcs;

    [KSField]
    public bool Enabled {
        get => dataRcs.isEnabled.GetValue();
        set => dataRcs.isEnabled.SetValue(value);
    }

    [KSField]
    public bool EnablePitch {
        get => dataRcs.enablePitch.GetValue();
        set => dataRcs.enablePitch.SetValue(value);
    }

    [KSField]
    public bool EnableYaw {
        get => dataRcs.enableYaw.GetValue();
        set => dataRcs.enableYaw.SetValue(value);
    }

    [KSField]
    public bool EnableRoll {
        get => dataRcs.enableRoll.GetValue();
        set => dataRcs.enableRoll.SetValue(value);
    }

    [KSField]
    public bool EnableX {
        get => dataRcs.enableX.GetValue();
        set => dataRcs.enableX.SetValue(value);
    }

    [KSField]
    public bool EnableY {
        get => dataRcs.enableY.GetValue();
        set => dataRcs.enableY.SetValue(value);
    }

    [KSField]
    public bool EnableZ {
        get => dataRcs.enableZ.GetValue();
        set => dataRcs.enableZ.SetValue(value);
    }

    [KSField]
    public double ThrustLimiter {
        get => dataRcs.thrustPercentage.GetValue() / 100.0;
        set => dataRcs.thrustPercentage.SetValue((float)(100.0 * value));
    }

    [KSField]
    public KSPResourceModule.ResourceDefinitionAdapter Propellant =>
        new(dataRcs.PropellantState.resourceDef);

}
