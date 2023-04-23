using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
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

        public Vector3d Position => body.coordinateSystem.ToLocalPosition(body.Position);

        public Vector3d AngularVelocity => body.celestialMotionFrame.ToLocalAngularVelocity(body.AngularVelocity);

        public Position GlobalPosition => body.Position;

        public AngularVelocity GlobalAngularVelocity => body.AngularVelocity;

        public Vector3d Up => body.transform.up.vector;

        public Vector3d Right => body.transform.right.vector;

        public Vector GlobalUp => body.transform.celestialFrame.up;

        public Vector GlobalRight => body.transform.celestialFrame.right;

        [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(context, body.Orbit);

        public bool HasAtmosphere => body.hasAtmosphere;

        public double AtmosphereDepth => body.atmosphereDepth;

        public double Radius => body.radius;

        public ITransformFrame CelestialFrame => body.transform.celestialFrame;

        public ITransformFrame BodyFrame => body.transform.bodyFrame;

        public Vector3d SurfaceNormal(double lat, double lon) => body.GetSurfaceNVector(lat, lon);

        public Vector GlobalSurfaceNormal(double latitude, double longitude) => new Vector(body.transform.celestialFrame,
           body.GetSurfaceNVector(latitude, longitude));

        public double TerrainHeight(double lat, double lon) => body.SurfaceProvider.GetTerrainAltitudeFromCenter(lat, lon) - body.radius;

        public Vector3d SurfacePosition(double latitude, double longitude, double altitude) =>
            body.GetWorldSurfacePosition(latitude, longitude, altitude, body.coordinateSystem);

        public Position GlobalSurfacePosition(double latitude, double longitude, double altitude) => new Position(body.transform.celestialFrame, body.GetSurfaceNVector(latitude, longitude) * (body.radius + altitude));

        public KSPOrbitModule.GeoCoordinates GeoCoordinates(double latitude, double longitude) => new KSPOrbitModule.GeoCoordinates(this, latitude, longitude);

        public KSPOrbitModule.IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut) {
            PatchedConicsOrbit orbit = new PatchedConicsOrbit(body.universeModel);

            orbit.UpdateFromStateVectors(new Position(body.SimulationObject.transform.celestialFrame, position), new Velocity(body.SimulationObject.transform.celestialFrame.motionFrame, velocity), body, ut);

            return new OrbitWrapper(context, orbit);
        }

        public KSPOrbitModule.IOrbit GlobalCreateOrbit(VelocityAtPosition velocity, double ut) {
            PatchedConicsOrbit orbit = new PatchedConicsOrbit(body.universeModel);

            orbit.UpdateFromStateVectors(velocity.position, velocity.velocity, body, ut);

            return new OrbitWrapper(context, orbit);
        }

        public Option<KSPOrbitModule.IBody> AsBody => new Option<KSPOrbitModule.IBody>(this);

        public Option<KSPVesselModule.VesselAdapter> AsVessel => new Option<KSPVesselModule.VesselAdapter>();

        public Option<KSPVesselModule.ModuleDockingNodeAdapter> AsDockingPort => new Option<KSPVesselModule.ModuleDockingNodeAdapter>();


        public IGGuid UnderlyingId => body.SimulationObject.GlobalId;
    }
}
