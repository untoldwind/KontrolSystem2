﻿using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass]
        public class GeoCoordinates {
            private readonly IBody body;

            [KSField] public IBody Body => body;

            [KSField] public double Latitude { get; set; }

            [KSField] public double Longitude { get; set; }

            public GeoCoordinates(IBody body, double latitude, double longitude) {
                this.body = body;
                Latitude = latitude;
                Longitude = longitude;
            }

            [KSField] public Vector SurfaceNormal => body.SurfaceNormal(Latitude, Longitude);

            [KSField] public double TerrainHeight => body.TerrainHeight(Latitude, Longitude);

            [KSMethod]
            public Position AltitudePosition(double altitude) => body.SurfacePosition(Latitude, Longitude, altitude);
        }
    }
}
