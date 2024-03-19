using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public class BaseLiftingSurfaceAdapter<P, T>(P part, Data_LiftingSurface dataLiftingSurface) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_LiftingSurface dataLiftingSurface = dataLiftingSurface;

    [KSField]
    public double LiftDragRatio => dataLiftingSurface.LiftDragRatio.GetValue();

    [KSField]
    public double LiftScalar => dataLiftingSurface.LiftScalar.GetValue();

    [KSField]
    public double DragScalar => dataLiftingSurface.DragScalar.GetValue();

    [KSField]
    public double AngleOfAttack => dataLiftingSurface.AoA.GetValue();
}
