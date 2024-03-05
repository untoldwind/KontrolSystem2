using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseDecouplerAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_Decouple dataDecouple;

    protected BaseDecouplerAdapter(P part, Data_Decouple dataDecouple) : base(part) {
        this.dataDecouple = dataDecouple;
    }

    [KSField]
    public double EjectionImpulse {
        get => dataDecouple.EjectionImpulse.GetValue();
        set => dataDecouple.EjectionImpulse.SetValue((float)value);
    }

    [KSField] public bool IsDecoupled => dataDecouple.isDecoupled.GetValue();

}
