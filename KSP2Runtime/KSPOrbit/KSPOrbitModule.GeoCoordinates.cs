using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public partial class KSPOrbitModule {
    [KSClass(Description = "Represents a geo coordinate (longitude, latitude) of a specific celestial body.")]
    public class GeoCoordinates(IBody body, double latitude, double longitude) {
        [KSField(Description = "The celestial body the geo coordinate is based on.")]
        public IBody Body { get; } = body;

        [KSField(Description = "Latitude in degrees")]
        public double Latitude { get; set; } = latitude;

        [KSField(Description = "Longitude in degrees")]
        public double Longitude { get; set; } = longitude;

        [KSField(Description = "The surface normal (i.e. up vector) in the celestial frame of the body")]
        public Vector3d SurfaceNormal => Body.SurfaceNormal(Latitude, Longitude);

        [KSField(Description = "Coordinate system independent surface normal (i.e. up vector)")]
        public Vector GlobalSurfaceNormal => Body.GlobalSurfaceNormal(Latitude, Longitude);

        [KSField(Description = "Height of the terrain relative to sea-level")]
        public double TerrainHeight => Body.TerrainHeight(Latitude, Longitude);

        [KSMethod(Description = "Position of the geo coordinate in the celestial frame of the body")]
        public Vector3d AltitudePosition([KSParameter("Altitude relative to sea-level")] double altitude) =>
            Body.SurfacePosition(Latitude, Longitude, altitude);

        [KSMethod(Description = "Coordinate system independent position of the geo coordinate")]
        public Position GlobalAltitudePosition([KSParameter("Altitude relative to sea-level")] double altitude) =>
            Body.GlobalSurfacePosition(Latitude, Longitude, altitude);
    }
}
