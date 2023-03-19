using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KSP.Map;
using KSP.Sim;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("GroundMarker",
            Description = "Represents a ground marker on a given celestial body."
        )]
        public class GroundMarker : IMarker {
            [KSField(Description = "Controls if the ground marker is currently visible (initially `true`)")]
            public bool Visible { get; set; }

            [KSField] public double Rotation { get; set; }


            [KSField] public KSPOrbitModule.GeoCoordinates GeoCoordinates { get; set; }

            [KSField(Description = "The color of the ground marker vector")]
            public KSPConsoleModule.RgbaColor Color { get; set; }

            public GroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates, KSPConsoleModule.RgbaColor color,
                double rotation) {
                Color = color;
                GeoCoordinates = geoCoordinates;
                Rotation = rotation;
                Visible = true;
            }

            [KSMethod]
            public void Remove() => KSPContext.CurrentContext.RemoveMarker(this);

            public void OnUpdate() {
            }

            public void OnRender() {
                Vector surfaceNormal = GeoCoordinates.GlobalSurfaceNormal;
                Position position = GeoCoordinates.GlobalAltitudePosition(GeoCoordinates.TerrainHeight);
                Vector3d up;
                Vector3d bodyUp;
                Vector3d localPos;
                double radius;
                
                if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out MapCore mapCore) && mapCore.IsEnabled) {
                    var space = mapCore.map3D.GetSpaceProvider();
                    radius = 6000 / space.Map3DScaleInv;

                    localPos = space.TranslateSimPositionToMapPosition(position);
                    up = (space.TranslateSimPositionToMapPosition(position + surfaceNormal) - localPos).normalized;
                    bodyUp = (space.TranslateSimPositionToMapPosition(position + GeoCoordinates.Body.GlobalUp) - localPos).normalized;
                } else {
                    var frame = KSPContext.CurrentContext.ActiveVessel.transform?.coordinateSystem;
                    localPos = frame.ToLocalPosition(position);
                    up = frame.ToLocalVector(surfaceNormal);
                    bodyUp = frame.ToLocalVector(GeoCoordinates.Body.GlobalUp);
                    radius = 5;
                }

                Camera camera = KSPContext.CurrentContext.Game.SessionManager.GetMyActiveCamera();
                
                Color color = Color.Color;
                Vector3d camPos = camera.transform.position;

//                if (GLUtils.IsOccluded(center, localPos, GeoCoordinates.Body.Radius, camPos)) return;

                Vector3d north = Vector3d.Exclude(up, bodyUp).normalized;
                
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
        }
    }
}
