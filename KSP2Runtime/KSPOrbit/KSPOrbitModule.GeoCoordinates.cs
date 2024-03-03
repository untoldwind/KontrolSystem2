using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public partial class KSPOrbitModule {
    [KSClass(Description = "Represents a geo coordinate (longitude, latitude) of a specific celestial body.")]
    public class GeoCoordinates {
        public GeoCoordinates(IBody body, double latitude, double longitude) {
            this.Body = body;
            Latitude = latitude;
            Longitude = longitude;
        }

        [KSField(Description = "The celestial body the geo coordinate is based on.")]
        public IBody Body { get; }

        [KSField(Description = "Latitude in degrees")]
        public double Latitude { get; set; }

        [KSField(Description = "Longitude in degrees")]
        public double Longitude { get; set; }

        [KSField(Description = "The surface normal (i.e. up vector) in the celestial frame of the body")]
        public Vector3d SurfaceNormal => Body.SurfaceNormal(Latitude, Longitude);

        [KSField("Coordinate system independent surface normal (i.e. up vector)")]
        public Vector GlobalSurfaceNormal => Body.GlobalSurfaceNormal(Latitude, Longitude);

        [KSField(Description = "Height of the terrain relative to sea-level")]
        public double TerrainHeight => Body.TerrainHeight(Latitude, Longitude);

        [KSMethod(Description = "Position of the geo coordinate in the celestial frame of the body")]
        public Vector3d AltitudePosition([KSParameter("Altitude relative to sea-level")] double altitude) {
            return Body.SurfacePosition(Latitude, Longitude, altitude);
        }

        [KSMethod(Description = "Coordinate system independent position of the geo coordinate")]
        public Position GlobalAltitudePosition([KSParameter("Altitude relative to sea-level")] double altitude) {
            return Body.GlobalSurfacePosition(Latitude, Longitude, altitude);
        }
    }
}
