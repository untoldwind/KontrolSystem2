using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseLaunchClampAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_GroundLaunchClamp dataLaunchClamp;

    protected BaseLaunchClampAdapter(P part, Data_GroundLaunchClamp dataLaunchClamp) : base(part) {
        this.dataLaunchClamp = dataLaunchClamp;
    }

    [KSField] public bool IsReleased => dataLaunchClamp.isReleased;
}
