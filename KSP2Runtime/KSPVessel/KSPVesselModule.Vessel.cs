using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Vessel",
            Description =
                "Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite.")]
        public class VesselAdapter : IKSPTargetable {
            private readonly IKSPContext context;
            private readonly VesselComponent vessel;

            internal VesselAdapter(IKSPContext context, VesselComponent vessel) {
                this.context = context;
                this.vessel = vessel;
            }
            
            [KSField(Description = "The name of the vessel.")]
            public string Name => vessel.Name;
            
            [KSField] 
            public string ControlStatus => vessel.ControlStatus.ToString();
            
            [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(vessel.Orbit);

            [KSField] public Vector3d OrbitalVelocity => vessel.OrbitalVelocity.vector;

            [KSField] public Vector3d SurfaceVelocity => vessel.SurfaceVelocity.vector;
            
        } 
    }
}
