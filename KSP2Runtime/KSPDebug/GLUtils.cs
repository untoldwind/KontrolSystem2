using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public static class GLUtils {
        private static Material _colored;

        public static Material Colored {
            get {
                if (_colored == null) {
                    _colored = new Material(Shader.Find("Hidden/Internal-Colored"));
                }

                return _colored;
            }
        }

        //Tests if body occludes worldPosition, from the perspective of the planetarium camera
        // https://cesiumjs.org/2013/04/25/Horizon-culling/
        public static bool IsOccluded(Vector3d worldPosition, Vector3d bodyPosition, double bodyRadius, Vector3d camPos) {
            Vector3d vc = (bodyPosition - camPos) / (bodyRadius - 100);
            Vector3d vt = (worldPosition - camPos) / (bodyRadius - 100);

            double vtVc = Vector3d.Dot(vt, vc);

            // In front of the horizon plane
            if (vtVc < vc.sqrMagnitude - 1) return false;

            return vtVc * vtVc / vt.sqrMagnitude > vc.sqrMagnitude - 1;
        }

        public static void GLTriangle(Camera camera, Vector3d worldVertices1, Vector3d worldVertices2, Vector3d worldVertices3,
            Color c, Material material, bool map) {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.TRIANGLES);
            GL.Color(c);
            GLVertex(camera, worldVertices1, map);
            GLVertex(camera,worldVertices2, map);
            GLVertex(camera, worldVertices3, map);
            GL.End();
            GL.PopMatrix();
        }

        public static void GLVertex(Camera camera, Vector3d worldPosition, bool map) {
            Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);
            GL.Vertex3(screenPoint.x, screenPoint.y, 0);
        }
    }
}
