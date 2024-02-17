using System;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Runtime;
using KSP.Sim;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.Testing;

public class MockBody : KSPOrbitModule.IBody {
    public static readonly MockBody Kerbol = new("Kerbol", 261600000, 1.17233279483249E+18);

    public static readonly MockBody Kerbin = new("Kerbin", 600000, Kerbol, 0, 0, 13599840256, 0, 0, 0,
        3.14000010490417, 3531600000000, 84159286.4796305, true, 70000, 21549.425);

    public static readonly MockBody Mun = new("Mun", 200000, Kerbin, 0, 0, 12000000, 0, 0, 0,
        1.70000004768372,
        65138397520.7807, 2429559.11656475, false, 0, 138984.38);

    public static readonly MockBody Minmus = new("Minmus", 60000, Kerbin, 6, 0, 47000000, 78, 38, 0,
        0.899999976158142, 1765800026.31247, 2247428.3879023, false, 0, 40400.000);

    public static readonly MockBody Duna = new("Duna", 320000, Kerbol, 0.06, 0.051, 20726155264, 135.5, 0,
        0,
        3.14000010490417, 301363211975.098, 47921949.369738, true, 50000, 65517.859);

    public static readonly MockBody Ike = new("Ike", 130000, Duna, 0.2, 0.03, 3200000, 0, 0, 0, 1.7,
        18568368573.144, 1049598.93931162, false, 0, 65517.862);

    public static readonly MockBody Eve = new("Eve", 700000, Kerbol, 2.09999990463257, 0.00999999977648258,
        9832684544, 15, 0, 0, 3.14000010490417, 8171730229210.87, 85109364.7382441, true, 90000, 80500.000);

    public static readonly MockBody Gilly = new("Gilly", 13000, Eve, 12, 0.550000011920929, 31500000, 80,
        10, 0,
        0.899999976158142, 8289449.81471635, 126123.271704568, false, 0, 80500.000);

    public static readonly MockBody Jool = new("Jool", 6000000, Kerbol, 1.30400002002716,
        0.0500000007450581,
        68773560320, 52, 0, 0, 0.100000001490116, 282528004209995, 2455985185.42347, true, 200000, 0);

    public static readonly MockBody Tylo = new("Tylo", 600000, Jool, 0.025000000372529, 0, 68500000, 0, 0,
        0,
        3.14000010490417, 2825280042099.95, 10856518.3683586, false, 0, 0);

    public static readonly MockBody Laythe = new("Laythe", 500000, Jool, 0, 0, 27184000, 0, 0, 0,
        3.14000010490417,
        1962000029236.08, 3723645.81113302, true, 50000, 0);

    public static readonly MockBody Bop = new("Bop", 65000, Jool, 15, 0.234999999403954, 128500000, 10, 25,
        0, 0.899999976158142, 2486834944.41491, 1221060.86284253, false, 0, 0);

    public static readonly MockBody Pol = new("Pol", 44000, Jool, 4.25, 0.17085, 179890000, 2, 15, 0,
        0.899999976158142, 721702080, 1042138.89230178, false, 0, 0);

    public static readonly MockBody Vall = new("Vall", 300000, Jool, 0, 0, 43152000, 0, 0, 0,
        0.899999976158142, 207481499473.751, 2406401.44479404, false, 0, 0);

    public readonly Vector3d angularVelocity;
    public readonly double atmosphereDepth;
    public readonly bool hasAtmosphere;
    public readonly double mu;
    public readonly string name;
    public readonly MockOrbit? orbit;

    public readonly MockBody? parent;
    public readonly double radius;
    public readonly double rotationPeriod;
    public readonly double soiRadius;

    public MockBody(string name, double radius, double mu) {
        this.name = name;
        this.mu = mu;
        parent = null;
        orbit = null;
        soiRadius = double.PositiveInfinity;
        this.radius = radius;
        hasAtmosphere = false;
        atmosphereDepth = 0;
    }

    public MockBody(string name,
        double radius,
        MockBody parent,
        double inclination,
        double eccentricity,
        double semiMajorAxis,
        double lan,
        double argumentOfPeriapsis,
        double epoch,
        double meanAnomalyAtEpoch,
        double mu,
        double soiRadius,
        bool hasAtmosphere,
        double atmosphereDepth,
        double rotationPeriod) {
        this.name = name;
        this.parent = parent;
        this.mu = mu;
        this.soiRadius = soiRadius;
        orbit = new MockOrbit(parent, inclination, eccentricity, semiMajorAxis, lan, argumentOfPeriapsis,
            epoch, meanAnomalyAtEpoch);
        this.radius = radius;
        this.hasAtmosphere = hasAtmosphere;
        this.atmosphereDepth = atmosphereDepth;
        this.rotationPeriod = rotationPeriod;
        angularVelocity = Vector3d.down * (2 * Math.PI / rotationPeriod);
    }

    public double RealMaxAtmosphereAltitude => atmosphereDepth;

    public string Name => name;

    public Option<KSPOrbitModule.IBody> ParentBody => Option.OfNullable<KSPOrbitModule.IBody>(parent);

    public KSPOrbitModule.IBody[] OrbitingBodies => Array.Empty<KSPOrbitModule.IBody>();

    public double GravParameter => mu;

    public double SoiRadius => soiRadius;

    public bool HasAtmosphere => hasAtmosphere;

    public double AtmosphereDepth => atmosphereDepth;

    public KSPOrbitModule.IOrbit Orbit => orbit!;

    public Option<KSPOrbitModule.IBody> AsBody => new(this);

    public Option<KSPVesselModule.VesselAdapter> AsVessel => new();

    public Option<KSPVesselModule.ModuleDockingNodeAdapter> AsDockingPort => new();

    public IGGuid UnderlyingId { get; }

    public double Radius => radius;

    public double RotationPeriod => rotationPeriod;

    public Vector3d Position => Vector3d.zero;

    public Position GlobalPosition => new(CelestialFrame, Vector3d.zero);

    public ITransformFrame CelestialFrame => KSPTesting.IDENTITY_COORDINATE_SYSTEM;

    public ITransformFrame BodyFrame => KSPTesting.IDENTITY_COORDINATE_SYSTEM;

    public AngularVelocity GlobalAngularVelocity => new(CelestialFrame.motionFrame, AngularVelocity);

    public Vector3d Up => Vector3d.up;

    public Vector3d Right => Vector3d.right;

    public Vector GlobalUp => new(CelestialFrame, Up);

    public Vector GlobalRight => new(CelestialFrame, Right);

    public Vector3d AngularVelocity => angularVelocity;

    public Vector3d SurfaceNormal(double lat, double lon) {
        lat *= Math.PI / 180.0;
        lon *= Math.PI / 180.0;
        var phi = Math.Cos(lat);
        var z = Math.Sin(lat);

        return new Vector3d(phi * Math.Cos(lon), z, phi * Math.Sin(lon));
    }

    public Vector GlobalSurfaceNormal(double latitude, double longitude) {
        return new Vector(CelestialFrame, SurfaceNormal(latitude, longitude));
    }

    public double TerrainHeight(double lat, double lon) {
        return 0.0;
    }

    public Position GlobalSurfacePosition(double latitude, double longitude, double altitude) {
        return new Position(CelestialFrame, SurfacePosition(latitude, longitude, altitude));
    }

    public KSPOrbitModule.GeoCoordinates GeoCoordinates(double latitude, double longitude) {
        return new KSPOrbitModule.GeoCoordinates(this, latitude, longitude);
    }

    public Vector3d SurfacePosition(double latitude, double longitude, double altitude) {
        return SurfaceNormal(latitude, longitude) * (radius + altitude);
    }

    public KSPOrbitModule.IOrbit CreateOrbit(Vector3d relPos, Vector3d vel, double ut) {
        return new MockOrbit(this, relPos.SwapYAndZ, vel.SwapYAndZ, ut);
    }

    public KSPOrbitModule.IOrbit GlobalCreateOrbit(VelocityAtPosition velocity, double ut) {
        return new MockOrbit(this, CelestialFrame.ToLocalPosition(velocity.position),
            CelestialFrame.motionFrame.ToLocalVelocity(velocity.velocity, velocity.position), ut);
    }

    public Vector3d GetPositionAtUT(double ut) {
        return orbit?.GetRelativePositionAtUT(ut) ?? Vector3d.zero;
    }

    public Vector3d GetOrbitalVelocityAtUT(double ut) {
        return orbit?.GetOrbitalVelocityAtUT(ut) ?? Vector3d.zero;
    }

    public double Latitude(Vector3d position) {
        var normalized = position.normalized.SwapYAndZ;
        return Math.Asin(normalized.z) * 180.0 / Math.PI;
    }

    public double Longitude(Vector3d position) {
        var normalized = position.normalized.SwapYAndZ;
        return Math.Atan2(normalized.y, normalized.x) * 180.0 / Math.PI;
    }

    public Vector3d RelativeVelocity(Vector3d position) {
        return Vector3d.Cross(angularVelocity, position);
    }

    public double AltitudeOf(Vector3d position) {
        return position.magnitude - radius;
    }

    public KSPOrbitModule.IOrbit CreateOrbitFromParameters(double inclination, double eccentricity,
        double semiMajorAxis, double lan,
        double argumentOfPeriapsis, double meanAnomalyAtEpoch, double epoch) {
        return new MockOrbit(this, inclination, eccentricity, semiMajorAxis, lan, argumentOfPeriapsis, epoch,
            meanAnomalyAtEpoch);
    }
}
