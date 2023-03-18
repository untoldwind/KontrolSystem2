using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleDockingNode")]
        public class ModuleDockingNodeAdapter {
            private readonly PartComponent part;
            private readonly Data_DockingNode dataDockingNode;

            public ModuleDockingNodeAdapter(PartComponent part, Data_DockingNode dataDockingNode) {
                this.part = part;
                this.dataDockingNode = dataDockingNode;
            }

            [KSField]
            public bool IsDeployableDockingPort => dataDockingNode.IsDeployableDockingPort;

            [KSField]
            public string DockingState => dataDockingNode.CurrentState.ToString();
        }
    }
}
