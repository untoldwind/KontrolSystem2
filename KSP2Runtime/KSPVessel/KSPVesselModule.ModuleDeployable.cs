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

        [KSField] public Data_Deployable.DeployState DeployState => dataDeployable.CurrentDeployState.GetValue();

        [KSField] public bool Extendable => dataDeployable.extendable;

        [KSField] public bool Retractable => dataDeployable.retractable;

        [KSField]
        public double DeployLimit {
            get => dataDeployable.DeployLimit.GetValue();
            set => dataDeployable.DeployLimit.SetValue((float)value);
        }
        
        [KSField]
        public bool Extended {
            get => dataDeployable.toggleExtend.GetValue();
            set => dataDeployable.toggleExtend.SetValue(value);
        }

        [KSMethod]
        public void SetExtended(bool extend) {
            dataDeployable.toggleExtend.SetValue(extend);
        }
    }
}
