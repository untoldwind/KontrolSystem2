using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public class BodyWrapper : KSPOrbitModule.IBody, KSPVesselModule.IKSPTargetable {
        private readonly IKSPContext context;
        private readonly CelestialBodyComponent body;

        public BodyWrapper(IKSPContext context, CelestialBodyComponent body) {
            this.context = context;
            this.body = body;
        }

        public string Name => body.Name;

        public double GravParameter => body.gravParameter;
        
        public double SoiRadius => body.sphereOfInfluence;

        public double RotationPeriod => body.rotationPeriod;

        public Vector3d Up => body.transform.up.vector;

        public Vector3d Right => body.transform.right.vector;

        [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(context, body.Orbit);

        public bool HasAtmosphere => body.hasAtmosphere;

        public double AtmosphereDepth => body.atmosphereDepth;

        public double Radius => body.radius;
        
        public Vector3d SurfaceNormal(double lat, double lon) => body.GetSurfaceNVector(lat, lon);

        public double TerrainHeight(double lat, double lon) => body.SurfaceProvider.GetTerrainAltitudeFromCenter(lat, lon);

        public Vector3d SurfacePosition(double latitude, double longitude, double altitude) =>
            body.GetWorldSurfacePosition(latitude, longitude, altitude, body.coordinateSystem);
        
        public KSPOrbitModule.IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut) {
            PatchedConicsOrbit orbit = new PatchedConicsOrbit(body.universeModel);
            
            orbit.UpdateFromStateVectors(new Position(body.coordinateSystem, position), new Velocity(body.bodyMotionFrame, velocity), body, ut);

            return new OrbitWrapper(context, orbit);
        }
    }
}
