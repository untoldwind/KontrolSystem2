using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Targetable")]
        public interface IKSPTargetable {
            [KSField(Description = "Name of the vessel target.")] string Name { get; }

            [KSField(Description = "Orbit of the vessel target.")] KSPOrbitModule.IOrbit Orbit { get; }

            [KSField("body", Description = "Get the targeted celestial body, if target is a body.")] Option<KSPOrbitModule.IBody> AsBody { get; }

            [KSField("vessel", Description = "Get the targeted vessel, if target is a vessel.")] Option<VesselAdapter> AsVessel { get; }

            [KSField("docking_node", Description = "Get the targeted docking node, if target is a docking node.")] Option<ModuleDockingNodeAdapter> AsDockingPort { get; }

            IGGuid UnderlyingId { get; }
        }
    }
}
