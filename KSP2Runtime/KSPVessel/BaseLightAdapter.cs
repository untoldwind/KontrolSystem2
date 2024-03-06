using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseLightAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_Light dataLight;

    protected BaseLightAdapter(P part, Data_Light dataLight) : base(part) {
        this.dataLight = dataLight;
    }

    [KSField]
    public bool LightEnabled {
        get => dataLight.isLightEnabled.GetValue();
        set => dataLight.isLightEnabled.SetValue(value);
    }

    [KSField]
    public bool BlinkEnabled {
        get => dataLight.isBlinkEnabled.GetValue();
        set => dataLight.isBlinkEnabled.SetValue(value);
    }

    [KSField]
    public double BlinkRate {
        get => dataLight.blinkRate.GetValue();
        set => dataLight.blinkRate.SetValue((float)value);
    }

    [KSField]
    public Vector3d LightColor {
        get => new Vector3d(dataLight.lightColorR.GetValue(), dataLight.lightColorG.GetValue(),
            dataLight.lightColorB.GetValue());
        set {
            dataLight.lightColorR.SetValue((float)value.x);
            dataLight.lightColorG.SetValue((float)value.y);
            dataLight.lightColorB.SetValue((float)value.z);
        }
    }

    [KSField]
    public double Rotation {
        get => dataLight.rotationAngle.GetValue();
        set => dataLight.rotationAngle.SetValue((float)value);
    }

    [KSField]
    public double Pitch {
        get => dataLight.pitchAngle.GetValue();
        set => dataLight.pitchAngle.SetValue((float)value);
    }

    [KSField] public bool HasResourcesToOperate => dataLight.HasResourcesToOperate;

    [KSField] public KSPResourceModule.ResourceSettingAdapter RequiredResource => new(dataLight.requiredResource);

}
