using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public partial class KSPDebugModule {
    [KSClass("GroundMarker",
        Description = "Represents a ground marker on a given celestial body."
    )]
    public class GroundMarker : IMarker {
        public GroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates, KSPConsoleModule.RgbaColor color,
            double rotation) {
            Color = color;
            GeoCoordinates = geoCoordinates;
            Rotation = rotation;
            Visible = true;
        }

        [KSField] public double Rotation { get; set; }


        [KSField] public KSPOrbitModule.GeoCoordinates GeoCoordinates { get; set; }

        [KSField(Description = "The color of the ground marker vector")]
        public KSPConsoleModule.RgbaColor Color { get; set; }

        [KSField(Description = "Controls if the ground marker is currently visible (initially `true`)")]
        public bool Visible { get; set; }

        public void OnUpdate() {
        }

        public void OnRender() {
            var surfaceNormal = GeoCoordinates.GlobalSurfaceNormal;
            var position = GeoCoordinates.GlobalAltitudePosition(GeoCoordinates.TerrainHeight);
            Vector3d up;
            Vector3d bodyUp;
            Vector3d localPos;
            double radius;

            if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore) && mapCore.IsEnabled) {
                var space = mapCore.map3D.GetSpaceProvider();
                radius = 6000 / space.Map3DScaleInv;

                localPos = space.TranslateSimPositionToMapPosition(position);
                up = (space.TranslateSimPositionToMapPosition(position + surfaceNormal) - localPos).normalized;
                bodyUp = (space.TranslateSimPositionToMapPosition(position + GeoCoordinates.Body.GlobalUp) - localPos)
                    .normalized;
            } else {
                var frame = KSPContext.CurrentContext.ActiveVessel?.transform?.celestialFrame;
                localPos = frame?.ToLocalPosition(position) ?? Vector3d.zero;
                up = frame?.ToLocalVector(surfaceNormal) ?? Vector3d.zero;
                bodyUp = frame?.ToLocalVector(GeoCoordinates.Body.GlobalUp) ?? Vector3d.up;
                radius = 5;
            }

            var camera = KSPContext.CurrentContext.Game.SessionManager.GetMyActiveCamera();

            var color = Color.Color;
            Vector3d camPos = camera.transform.position;

            //                if (GLUtils.IsOccluded(center, localPos, GeoCoordinates.Body.Radius, camPos)) return;

            var north = Vector3d.Exclude(up, bodyUp).normalized;

            GLUtils.GLTriangle(camera,
                localPos,
                localPos + radius * (QuaternionD.AngleAxis(Rotation - 10, up) * north),
                localPos + radius * (QuaternionD.AngleAxis(Rotation + 10, up) * north),
                color, GLUtils.Colored);

            GLUtils.GLTriangle(camera,
                localPos,
                localPos + radius * (QuaternionD.AngleAxis(Rotation + 110, up) * north),
                localPos + radius * (QuaternionD.AngleAxis(Rotation + 130, up) * north),
                color, GLUtils.Colored);

            GLUtils.GLTriangle(camera,
                localPos,
                localPos + radius * (QuaternionD.AngleAxis(Rotation - 110, up) * north),
                localPos + radius * (QuaternionD.AngleAxis(Rotation - 130, up) * north),
                color, GLUtils.Colored);
        }

        [KSMethod]
        public void Remove() {
            KSPContext.CurrentContext.RemoveMarker(this);
        }
    }
}
