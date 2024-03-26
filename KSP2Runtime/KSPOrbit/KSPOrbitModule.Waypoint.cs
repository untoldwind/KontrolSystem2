using KontrolSystem.TO2.Binding;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPOrbit;

public partial class KSPOrbitModule {
    [KSClass("Waypoint")]
    public class WaypointAdapter(IKSPContext context, WaypointComponent waypoint) {
        [KSField(Description = "Name/label of the waypoint")]
        public string Name => waypoint.DisplayName;

        [KSField(Description = "Celestial body the waypoint is based on")]
        public IBody Body => new BodyWrapper(context, waypoint.MainBody);

        [KSField(Description = "Coordinate system independent position of the waypoint")]
        public Position GlobalPosition => waypoint.LabelPosition;

        [KSField(Description = "Get `GeoCoordinates` of the waypoint")]
        public GeoCoordinates GeoCoordinates {
            get {
                waypoint.MainBody.GetLatLonAltFromRadius(waypoint.LabelPosition, out var lat, out var lon, out _);

                return new GeoCoordinates(new BodyWrapper(context, waypoint.MainBody), lat, lon);
            }
        }

        [KSField(Description = "Get altitude above sea-level of waypoint")]
        public double Altitude => waypoint.MainBody.GetAltitudeFromRadius(waypoint.LabelPosition);
    }
}
