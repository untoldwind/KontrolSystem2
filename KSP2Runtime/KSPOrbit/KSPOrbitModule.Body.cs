using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Body", Description = "Represents an in-game celestial body.")]
        public interface IBody : KSPVesselModule.IKSPTargetable {
            [KSField(Description = "Name of the celestial body.")]
            string Name { get; }

            [KSField(Description = "Standard gravitation parameter of the body.")]
            double GravParameter { get; }

            [KSField("SOI_radius", Description = "Radius of the sphere of influence of the body")]
            double SoiRadius { get; }

            [KSField(Description = "The orbit of the celestial body itself (around the parent body)")]
            IOrbit Orbit { get; }

            [KSField(Description = "`true` if the celestial body has an atmosphere to deal with.")]
            bool HasAtmosphere { get; }

            [KSField(Description = "Depth/height of the atmosphere if present.")]
            double AtmosphereDepth { get; }

            [KSField(Description = "Radius of the body at sea level")]
            double Radius { get; }

            [KSField(Description = "Rotation period of the planet.")]
            double RotationPeriod { get; }

            [KSField] ITransformFrame CelestialFrame { get; }

            [KSField] ITransformFrame BodyFrame { get; }

            [KSField(Description = "The current position of the body")] Vector3d Position { get; }

            [KSField(Description = "The current position of the body (coordinate system independent)")] Position GlobalPosition { get; }

            [KSField(Description = "Angular velocity vector of the body")] Vector3d AngularVelocity { get; }

            [KSField(Description = "Angular velocity vector of the body (coordinate system independent)")] AngularVelocity GlobalAngularVelocity { get; }

            [KSField] Vector3d Up { get; }

            [KSField] Vector3d Right { get; }

            [KSField] Vector GlobalUp { get; }

            [KSField] Vector GlobalRight { get; }

            [KSMethod(Description = "Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate)")]
            Vector3d SurfaceNormal(double latitude, double longitude);

            [KSMethod(Description = "Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate, coordinate system independent)")]
            Vector GlobalSurfaceNormal(double latitude, double longitude);

            [KSMethod]
            double TerrainHeight(double lat, double lon);

            [KSMethod]
            Vector3d SurfacePosition(double latitude, double longitude, double altitude);

            [KSMethod]
            Position GlobalSurfacePosition(double latitude, double longitude, double altitude);

            [KSMethod] GeoCoordinates GeoCoordinates(double latitude, double longitude);

            [KSMethod(Description =
                "Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `ut`")]
            IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut);
        }
    }
}
