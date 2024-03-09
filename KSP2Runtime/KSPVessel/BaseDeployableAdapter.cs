using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseDeployableAdapter<P, T>(P part, Data_Deployable dataDeployable)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Deployable dataDeployable = dataDeployable;

    [KSField] public Data_Deployable.DeployState DeployState => dataDeployable.CurrentDeployState.GetValue();

    [KSField] public bool Extendable => dataDeployable.extendable;

    [KSField] public bool Retractable => dataDeployable.retractable;

    [KSField]
    public double DeployLimit {
        get => dataDeployable.DeployLimit.GetValue();
        set => dataDeployable.DeployLimit.SetValue((float)value);
    }

    [KSField]
    public bool Extended {
        get => dataDeployable.toggleExtend.GetValue();
        set => dataDeployable.toggleExtend.SetValue(value);
    }

    [KSMethod]
    public void SetExtended(bool extend) {
        dataDeployable.toggleExtend.SetValue(extend);
    }

}
