using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseModuleAdapter<P, T>(P part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly P part = part;

    [KSField] public P Part => part;

    [KSField] public string PartName => part.PartName;

}
