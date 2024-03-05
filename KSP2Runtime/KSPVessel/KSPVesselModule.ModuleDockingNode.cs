using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("ModuleDockingNode")]
    public class ModuleDockingNodeAdapter : BaseDockingNodeAdapter<PartAdapter, PartComponent>, IKSPTargetable {
        public ModuleDockingNodeAdapter(PartAdapter part, Data_DockingNode dataDockingNode) : base(part, dataDockingNode) {
        }

        public string Name => part.PartName;

        public KSPOrbitModule.IOrbit Orbit => part.vesselAdapter.Orbit;

        public Option<KSPOrbitModule.IBody> AsBody => new();

        public Option<VesselAdapter> AsVessel => new();

        public Option<ModuleDockingNodeAdapter> AsDockingPort => new(this);

        public IGGuid UnderlyingId => part.part.GlobalId;

        [KSMethod]
        public void ControlFromHere() {
            part.vesselAdapter.vessel.SetControlOwner(part.part);
        }
    }
}
