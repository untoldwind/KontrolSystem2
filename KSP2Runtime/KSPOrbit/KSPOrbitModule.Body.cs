using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Binding;
using KSP.Api;
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
            
            [KSField(Description = "Angular velocity vector of the body")] Vector3d AngularVelocity { get; }
            
            [KSField] ITransformFrame ReferenceFrame { get; }

            [KSField] public Position Position { get; }

            [KSField] Vector Up { get; }

            [KSField] Vector Right { get; }

            [KSMethod(Description = "Get the surface normal at a `latitude` and `longitude` (i.e. the vector pointing up at this geo coordinate")]
            Vector SurfaceNormal(double latitude, double longitude);

            [KSMethod]
            double TerrainHeight(double lat, double lon);

            [KSMethod]
            Position SurfacePosition(double latitude, double longitude, double altitude);

            [KSMethod] GeoCoordinates GeoCoordinates(double latitude, double longitude);

            [KSMethod(Description =
                "Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `ut`")]
            IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut);

            [KSMethod]
            IOrbit CreateOrbitFromParameters(double inclination, double eccentricity,
                double semiMajorAxis, double lan,
                double argumentOfPeriapsis, double meanAnomalyAtEpoch, double epoch);
        }
    }
}
