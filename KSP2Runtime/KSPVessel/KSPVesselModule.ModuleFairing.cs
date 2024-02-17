using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleFairing")]
    public class ModuleFairingAdapter {
        private readonly Data_Fairing dataFairing;
        private readonly PartComponent part;

        public ModuleFairingAdapter(PartComponent part, Data_Fairing dataFairing) {
            this.part = part;
            this.dataFairing = dataFairing;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";

        [KSField]
        public double EjectionForce {
            get => dataFairing.EjectionForce.GetValue();
            set => dataFairing.EjectionForce.SetValue((float)value);
        }

        [KSField] public bool IsJettisoned => dataFairing.IsDeployed.GetValue();

        [KSMethod]
        public bool Jettison() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Fairing>(out var moduleFairing)) return false;

            moduleFairing.PerformJettison();
            return dataFairing.IsDeployed.GetValue();
        }
    }
}
