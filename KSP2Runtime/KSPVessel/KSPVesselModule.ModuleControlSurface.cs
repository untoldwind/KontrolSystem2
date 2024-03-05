using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleControlSurface")]
    public class ModuleControlSurfaceAdapter : BaseControlSurfaceAdapter<PartAdapter, PartComponent> {

        public ModuleControlSurfaceAdapter(PartAdapter part, Data_ControlSurface dataControlSurface) : base(part, dataControlSurface) {
        }
    }
}
