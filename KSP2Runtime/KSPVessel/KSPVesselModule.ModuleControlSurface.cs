using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleControlSurface")]
    public class ModuleControlSurfaceAdapter : BaseControlSurfaceAdapter {
        private readonly PartComponent part;

        public ModuleControlSurfaceAdapter(PartComponent part, Data_ControlSurface dataControlSurface) : base(dataControlSurface) {
            this.part = part;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";
    }
}
