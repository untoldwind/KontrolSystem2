using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleLaunchClamp")]
    public class ModuleLaunchClampAdapter {
        private readonly Data_GroundLaunchClamp dataLaunchClamp;
        private readonly PartComponent part;

        public ModuleLaunchClampAdapter(PartComponent part, Data_GroundLaunchClamp dataLaunchClamp) {
            this.part = part;
            this.dataLaunchClamp = dataLaunchClamp;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";

        [KSField] public bool IsReleased => dataLaunchClamp.isReleased;

        [KSMethod]
        public bool Release() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_GroundLaunchClamp>(out var moduleLaunchClamp)) return false;

            moduleLaunchClamp.Release();
            return dataLaunchClamp.isReleased;
        }
    }
}
