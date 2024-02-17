using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDecoupler")]
    public class ModuleDecouplerAdapter {
        private readonly Data_Decouple dataDecouple;
        private readonly PartComponent part;

        public ModuleDecouplerAdapter(PartComponent part, Data_Decouple dataDecouple) {
            this.part = part;
            this.dataDecouple = dataDecouple;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";

        [KSField]
        public double EjectionImpulse {
            get => dataDecouple.EjectionImpulse.GetValue();
            set => dataDecouple.EjectionImpulse.SetValue((float)value);
        }

        [KSField] public bool IsDecoupled => dataDecouple.isDecoupled.GetValue();

        [KSMethod]
        public bool Decouple() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Decouple>(out var moduleDecouple)) return false;

            moduleDecouple.OnDecouple();
            return dataDecouple.isDecoupled.GetValue();
        }
    }
}
