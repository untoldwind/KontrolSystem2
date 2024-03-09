using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseGimbalAdapter<P, T>(P part, Data_Gimbal dataGimbal) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Gimbal dataGimbal = dataGimbal;

    [KSField]
    public bool Enabled {
        get => dataGimbal.isEnabled.GetValue();
        set => dataGimbal.isEnabled.SetValue(value);
    }

    [KSField]
    public bool EnablePitch {
        get => dataGimbal.enablePitch.GetValue();
        set => dataGimbal.enablePitch.SetValue(value);
    }

    [KSField]
    public bool EnableYaw {
        get => dataGimbal.enableYaw.GetValue();
        set => dataGimbal.enableYaw.SetValue(value);
    }

    [KSField]
    public bool EnableRoll {
        get => dataGimbal.enableRoll.GetValue();
        set => dataGimbal.enableRoll.SetValue(value);
    }

    [KSField]
    public double Limiter {
        get => dataGimbal.gimbalLimiter.GetValue();
        set => dataGimbal.gimbalLimiter.SetValue((float)value);
    }

}
