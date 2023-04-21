using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Body", Description = "Represents an in-game celestial body.")]
        public interface IBody : KSPVesselModule.IKSPTargetable {
            [KSField(Description = "Name of the celestial body.")]
            new string Name { get; }

            [KSField(Description = "Standard gravitation parameter of the body.")]
            double GravParameter { get; }

            [KSField("SOI_radius", Description = "Radius of the sphere of influence of the body")]
            double SoiRadius { get; }

            [KSField(Description = "The orbit of the celestial body itself (around the parent body)")]
            new IOrbit Orbit { get; }

            [KSField(Description = "`true` if the celestial body has an atmosphere to deal with.")]
            bool HasAtmosphere { get; }

            [KSField(Description = "Depth/height of the atmosphere if present.")]
            double AtmosphereDepth { get; }

            [KSField(Description = "Radius of the body at sea level")]
            double Radius { get; }

            [KSField(Description = "Rotation period of the planet.")]
            double RotationPeriod { get; }

            [KSField(Description = "The celestial/non-rotating reference frame of the body.")] ITransformFrame CelestialFrame { get; }

            [KSField(Description = "The body/rotating reference frame of the body.")] ITransformFrame BodyFrame { get; }

            [KSField(Description = "The current position of the body")] Vector3d Position { get; }

            [KSField(Description = "The current position of the body (coordinate system independent)")] Position GlobalPosition { get; }

            [KSField(Description = "Angular velocity vector of the body")] Vector3d AngularVelocity { get; }

            [KSField(Description = "Angular velocity vector of the body (coordinate system independent)")] AngularVelocity GlobalAngularVelocity { get; }

            [KSField(Description = "Up vector of the body in its celestial frame")] Vector3d Up { get; }

            [KSField(Description = "Right vector of the body in its celestial frame")] Vector3d Right { get; }

            [KSField(Description = "Up vector of the body (coordinate system independent)")] Vector GlobalUp { get; }

            [KSField(Description = "Right vector of the body (coordinate system independent)")] Vector GlobalRight { get; }

            [KSMethod(Description = "Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate)")]
            Vector3d SurfaceNormal(double latitude, double longitude);

            [KSMethod(Description = "Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate, coordinate system independent)")]
            Vector GlobalSurfaceNormal(double latitude, double longitude);

            [KSMethod(Description = "Height of the terrain relative to sea-level a a `latitude` and `longitude`")]
            double TerrainHeight(double latitude, double longitude);

            [KSMethod(Description = "Position of a `latitude` and `longitude` at an altitude relative to sea-level in the celestial frame of the body")]
            Vector3d SurfacePosition(double latitude, double longitude, double altitude);

            [KSMethod(Description = "Position of a `latitude` and `longitude` at an altitude relative to sea-level (coordinate system independent)")]
            Position GlobalSurfacePosition(double latitude, double longitude, double altitude);

            [KSMethod(Description = "Get `GeoCoordinates` struct representing a `latitude` and `longitude` of the body")] GeoCoordinates GeoCoordinates(double latitude, double longitude);

            [KSMethod(Description =
                "Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `ut`")]
            IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut);

            [KSMethod(Description =
                "Create a new orbit around this body starting at a given a coordinate independent `velocity` at universal time `ut`")]
            IOrbit GlobalCreateOrbit(VelocityAtPosition velocity, double ut);
        }
    }
}
