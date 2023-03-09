using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Api;
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


        public Vector3d AngularVelocity => body.celestialMotionFrame.ToLocalAngularVelocity(body.AngularVelocity);

        public Vector Up => body.transform.up;

        public Vector Right => body.transform.right;

        [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(context, body.Orbit);

        public bool HasAtmosphere => body.hasAtmosphere;

        public double AtmosphereDepth => body.atmosphereDepth;

        public double Radius => body.radius;

        public ITransformFrame ReferenceFrame => body.transform.celestialFrame;

        public Position Position => body.Position;

        public Vector SurfaceNormal(double lat, double lon) => new Vector(body.transform.celestialFrame, body.GetSurfaceNVector(lat, lon));

        public double TerrainHeight(double lat, double lon) => body.SurfaceProvider.GetTerrainAltitudeFromCenter(lat, lon) - body.radius;

        public Position SurfacePosition(double latitude, double longitude, double altitude) =>
            new Position(body.transform.celestialFrame, body.GetRelSurfacePosition(latitude, longitude, altitude));

        public KSPOrbitModule.GeoCoordinates GeoCoordinates(double latitude, double longitude) => new KSPOrbitModule.GeoCoordinates(this, latitude, longitude);
        
        public KSPOrbitModule.IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut) {
            PatchedConicsOrbit orbit = new PatchedConicsOrbit(body.universeModel);

            orbit.UpdateFromStateVectors(new Position(body.SimulationObject.transform.celestialFrame, position), new Velocity(body.SimulationObject.transform.celestialFrame.motionFrame, velocity), body, ut);

            return new OrbitWrapper(context, orbit);
        }
        
        public IGGuid UnderlyingId => body.SimulationObject.GlobalId;        

        public KSPOrbitModule.IOrbit CreateOrbitFromParameters(double inclination, double eccentricity,
            double semiMajorAxis, double lan,
            double argumentOfPeriapsis, double meanAnomalyAtEpoch, double epoch) {
            PatchedConicsOrbit orbit = new PatchedConicsOrbit(body.universeModel);

            orbit.inclination = inclination;
            orbit.eccentricity = eccentricity;
            orbit.semiMajorAxis = semiMajorAxis;
            orbit.longitudeOfAscendingNode = lan;
            orbit.argumentOfPeriapsis = argumentOfPeriapsis;
            orbit.meanAnomalyAtEpoch = meanAnomalyAtEpoch;
            orbit.epoch = epoch;

            return new OrbitWrapper(context, orbit);
        }
    }
}
