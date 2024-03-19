using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public class BaseDragAdapter<P, T>(P part, Data_Drag dataDrag) : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_Drag dataDrag = dataDrag;

    [KSField]
    public double ReferenceArea => dataDrag.ReferenceArea.GetValue();

    [KSField]
    public double ExposedArea => dataDrag.ExposedArea.GetValue();

    [KSField]
    public double TotalArea => dataDrag.TotalArea.GetValue();
}
