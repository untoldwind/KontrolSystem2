using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Iteration.UI.Binding;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleDockingNode")]
        public class ModuleDockingNodeAdapter : PartAdapter, IKSPTargetable {
            private readonly Data_DockingNode dataDockingNode;

            public ModuleDockingNodeAdapter(VesselAdapter vesselAdapter, PartComponent part, Data_DockingNode dataDockingNode) : base(vesselAdapter, part) {
                this.dataDockingNode = dataDockingNode;
            }

            [KSField]
            public bool IsDeployableDockingPort => dataDockingNode.IsDeployableDockingPort;

            [KSField]
            public Data_DockingNode.DockingState DockingState => dataDockingNode.CurrentState;

            [KSField]
            public string[] NodeTypes => dataDockingNode.NodeTypes;

            [KSMethod]
            public void ControlFromHere() {
                vesselAdapter.vessel.SetControlOwner(part);
            }

            public string Name => part.Name;

            public KSPOrbitModule.IOrbit Orbit => vesselAdapter.Orbit;

            public Option<KSPOrbitModule.IBody> AsBody => new Option<KSPOrbitModule.IBody>();

            public Option<VesselAdapter> AsVessel => new Option<VesselAdapter>();

            public Option<ModuleDockingNodeAdapter> AsDockingPort => new Option<ModuleDockingNodeAdapter>(this);

            public IGGuid UnderlyingId => part.GlobalId;
        }
    }
}
