using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public partial class KSPOrbitModule {
    [KSClass]
    public class GeoCoordinates {
        public GeoCoordinates(IBody body, double latitude, double longitude) {
            this.Body = body;
            Latitude = latitude;
            Longitude = longitude;
        }

        [KSField] public IBody Body { get; }

        [KSField] public double Latitude { get; set; }

        [KSField] public double Longitude { get; set; }

        [KSField] public Vector3d SurfaceNormal => Body.SurfaceNormal(Latitude, Longitude);

        [KSField] public Vector GlobalSurfaceNormal => Body.GlobalSurfaceNormal(Latitude, Longitude);

        [KSField] public double TerrainHeight => Body.TerrainHeight(Latitude, Longitude);

        [KSMethod]
        public Vector3d AltitudePosition(double altitude) {
            return Body.SurfacePosition(Latitude, Longitude, altitude);
        }

        [KSMethod]
        public Position GlobalAltitudePosition(double altitude) {
            return Body.GlobalSurfacePosition(Latitude, Longitude, altitude);
        }
    }
}
