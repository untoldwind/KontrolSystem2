using KontrolSystem.TO2.Binding;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly P part;

    protected BaseModuleAdapter(P part) {
        this.part = part;
    }

    [KSField] public P Part => part;

    [KSField] public string PartName => part.PartName;

}
