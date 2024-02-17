using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDeployable")]
    public class ModuleDeployableAdapter {
        private readonly Data_Deployable dataDeployable;
        private readonly PartComponent part;

        public ModuleDeployableAdapter(PartComponent part, Data_Deployable dataDeployable) {
            this.part = part;
            this.dataDeployable = dataDeployable;
        }

        [KSField] public string PartName => part?.PartName ?? "Unknown";

        [KSField] public string DeployState => dataDeployable.CurrentDeployState.GetValue().ToString();

        [KSField] public bool Extendable => dataDeployable.extendable;

        [KSField] public bool Retractable => dataDeployable.retractable;

        [KSMethod]
        public void SetExtended(bool extend) {
            dataDeployable.toggleExtend.SetValue(extend);
        }
    }
}
