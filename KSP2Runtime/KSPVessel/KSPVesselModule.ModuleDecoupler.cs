using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDecoupler")]
    public class ModuleDecouplerAdapter : BaseDecouplerAdapter<PartAdapter, PartComponent> {
        public ModuleDecouplerAdapter(PartAdapter part, Data_Decouple dataDecouple) : base(part, dataDecouple) {
        }

        [KSMethod]
        public bool Decouple() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(part.part.SimulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_Decouple>(out var moduleDecouple)) return false;

            moduleDecouple.OnDecouple();
            return dataDecouple.isDecoupled.GetValue();
        }
    }
}
