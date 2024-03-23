using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleFairing")]
    public class ModuleFairingAdapter(KSPVesselModule.PartAdapter part, Data_Fairing dataFairing) : BaseFairingAdapter<PartAdapter, PartComponent>(part, dataFairing) {
        [KSMethod(Description = "Jettison the fairing")]
        public bool Jettison() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Fairing>(out var moduleFairing)) return false;

            moduleFairing.PerformJettison();
            return dataFairing.IsDeployed.GetValue();
        }
    }
}
