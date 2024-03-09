using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseLaunchClampAdapter<P, T>(P part, Data_GroundLaunchClamp dataLaunchClamp)
    : BaseModuleAdapter<P, T>(part)
    where P : BasePartAdapter<T>
    where T : IDeltaVPart {
    protected readonly Data_GroundLaunchClamp dataLaunchClamp = dataLaunchClamp;

    [KSField] public bool IsReleased => dataLaunchClamp.isReleased;
}
