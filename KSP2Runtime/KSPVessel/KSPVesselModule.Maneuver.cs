using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using KSP.Sim.Maneuver;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Maneuver")]
        public class ManeuverAdapter {
            private readonly VesselComponent vessel;

            public ManeuverAdapter(VesselComponent vessel) => this.vessel = vessel;

            [KSField]
            public ManeuverNodeAdapter[] Nodes => vessel.Game.SpaceSimulation.Maneuvers
                .GetNodesForVessel(vessel.GlobalId)?.Select(node => new ManeuverNodeAdapter(vessel, node)).ToArray() ?? Array.Empty<ManeuverNodeAdapter>();
            
            [KSMethod]
            public Result<ManeuverNodeAdapter, string> NextNode() {
                ManeuverNodeData node = vessel.Game.SpaceSimulation.Maneuvers.GetNodesForVessel(vessel.GlobalId)?.FirstOrDefault();

                if (node == null) return Result.Err<ManeuverNodeAdapter, string>("No maneuver node present");
                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vessel, node));
            }
        }
    }
}
