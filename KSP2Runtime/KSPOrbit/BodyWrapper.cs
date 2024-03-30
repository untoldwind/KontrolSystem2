using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public class BodyWrapper(IKSPContext context, CelestialBodyComponent body)
    : KSPOrbitModule.IBody, KSPVesselModule.IKSPTargetable {
    public string Name => body.Name;

    public Option<KSPOrbitModule.IBody> ParentBody => Option.OfNullable(body.referenceBody).Map<KSPOrbitModule.IBody>(
        parent => new BodyWrapper(context, parent)
    );

    public KSPOrbitModule.IBody[] OrbitingBodies =>
        body.orbitingBodies.Select(child => new BodyWrapper(context, child)).ToArray<KSPOrbitModule.IBody>();

    public double GravParameter => body.gravParameter;

    public double SoiRadius => body.sphereOfInfluence;

    public double RotationPeriod => body.rotationPeriod;

    public Vector3d Position => body.transform.celestialFrame.ToLocalPosition(body.Position);

    public Vector3d AngularVelocity => body.celestialMotionFrame.ToLocalAngularVelocity(body.AngularVelocity);

    public Position GlobalPosition => body.Position;

    public AngularVelocity GlobalAngularVelocity => body.AngularVelocity;

    public Vector3d Up => body.transform.up.vector;

    public Vector3d Right => body.transform.right.vector;

    public Vector GlobalUp => body.transform.celestialFrame.up;

    public Vector GlobalRight => body.transform.celestialFrame.right;

    [KSField]
    public KSPOrbitModule.IOrbit Orbit {
        get {
            var orbit = body.Orbit;

            if (orbit != null) return new OrbitWrapper(context, orbit);

            return new StaticOrbit(context, body);
        }
    }

    public bool HasAtmosphere => body.hasAtmosphere;

    public double AtmosphereDepth => body.atmosphereDepth;

    public double Radius => body.radius;

    public ITransformFrame CelestialFrame => body.transform.celestialFrame;

    public ITransformFrame BodyFrame => body.transform.bodyFrame;

    public Vector3d SurfaceNormal(double lat, double lon) {
        return body.GetSurfaceNVector(lat, lon);
    }

    public Vector GlobalSurfaceNormal(double latitude, double longitude) {
        return new Vector(body.transform.celestialFrame,
            body.GetSurfaceNVector(latitude, longitude));
    }

    public double TerrainHeight(double latitude, double longitude) {
        var fromCenter = body.SurfaceProvider.GetTerrainAltitudeFromCenter(latitude, longitude);
        var position = new Position(body.transform.celestialFrame,
            body.GetSurfaceNVector(latitude, longitude) * (fromCenter + 1000));
        body.GetAltitudeFromTerrain(position, out var _, out var sceneryOffset);

        return fromCenter - body.radius + sceneryOffset;
    }

    public Vector3d SurfacePosition(double latitude, double longitude, double altitude) =>
        body.GetSurfaceNVector(latitude, longitude) * (body.radius + altitude);

    public Position GlobalSurfacePosition(double latitude, double longitude, double altitude) =>
        new(body.transform.celestialFrame,
            body.GetSurfaceNVector(latitude, longitude) * (body.radius + altitude));

    public KSPOrbitModule.GeoCoordinates GeoCoordinates(double latitude, double longitude) => new(this, latitude, longitude);

    public KSPOrbitModule.IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut) {
        var orbit = new PatchedConicsOrbit(body.universeModel);

        orbit.UpdateFromStateVectors(new Position(body.SimulationObject.transform.celestialFrame, position),
            new Velocity(body.SimulationObject.transform.celestialFrame.motionFrame, velocity), body, ut);

        return new OrbitWrapper(context, orbit);
    }

    public KSPOrbitModule.IOrbit GlobalCreateOrbit(VelocityAtPosition velocity, double ut) {
        var orbit = new PatchedConicsOrbit(body.universeModel);

        orbit.UpdateFromStateVectors(velocity.position, velocity.velocity, body, ut);

        return new OrbitWrapper(context, orbit);
    }

    public double AtmospherePressureKpa(double altitude) => body.hasAtmosphere ? body.GetPressure(altitude) : 0.0;

    public double AtmosphereTemperature(double altitude) => body.hasAtmosphere ? body.GetTemperature(altitude) : 0.0;

    public double AtmosphereDensity(double altitude) => body.hasAtmosphere
        ? body.GetDensity(body.GetPressure(altitude), body.GetTemperature(altitude))
        : 0.0;

    public KSPOrbitModule.WaypointAdapter[] Waypoints => context.Game.UniverseModel.GetAllWaypoints()
        .Where(waypoint => waypoint.MainBody == body)
        .Select(waypoint => new KSPOrbitModule.WaypointAdapter(context, waypoint)).ToArray();

    public Option<KSPOrbitModule.IBody> AsBody => new(this);

    public Option<KSPVesselModule.VesselAdapter> AsVessel => new();

    public Option<KSPVesselModule.ModuleDockingNodeAdapter> AsDockingPort => new();


    public IGGuid UnderlyingId => body.SimulationObject.GlobalId;
}
