using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Polygon2D : GLUIDrawer.IGLUIDrawable {
        private static readonly float EPSILON = 1e-5f;

        private Vector2d[] points;
        private Vector2d[]? triangles;

        public Polygon2D(KSPConsoleModule.RgbaColor fillColor, Vector2d[] points) {
            FillColor = fillColor;
            this.points = points;
            Triangulate();
        }

        [KSField] public KSPConsoleModule.RgbaColor FillColor { get; set; }

        [KSField]
        public Vector2d[] Points {
            get => points;
            set {
                points = value;
                Triangulate();
            }
        }

        public void OnDraw(GLUIDrawer.GLUIDraw draw) {
            if (triangles!.Length < 3) return;

            GL.Begin(GL.TRIANGLES);
            GL.Color(FillColor.Color);
            for (var i = 0; i < triangles.Length; i++) GL.Vertex3((float)triangles[i].x, (float)triangles[i].y, 0);

            GL.End();
        }

        private void Triangulate() {
            var n = points.Length;

            if (n < 3) {
                triangles = Array.Empty<Vector2d>();
                return;
            }

            var indices = new int[n];
            if (Area() > 0f)
                for (var v = 0; v < n; v++)
                    indices[v] = v;
            else
                for (var v = 0; v < n; v++)
                    indices[v] = n - 1 - v;

            var newTriangles = new List<Vector2d>(3 * n);
            for (var nv = n; nv > 2; nv--) {
                var found = false;
                for (var v = 0; v < nv; v++) {
                    var u = v > 0 ? v - 1 : nv - 1;
                    var w = v + 1 < nv ? v + 1 : 0;

                    if (IsEar(u, v, w, nv, indices)) {
                        found = true;
                        newTriangles.Add(points[indices[u]]);
                        newTriangles.Add(points[indices[v]]);
                        newTriangles.Add(points[indices[w]]);
                        for (var t = v + 1; t < nv; t++) indices[t - 1] = indices[t];

                        break;
                    }
                }

                if (!found) break;
            }

            triangles = newTriangles.ToArray();
        }

        private double Area() {
            var n = points.Length;
            var area = 0.0;
            var q = 0;
            for (var p = n - 1; q < n; p = q++) {
                var pt = points[p];
                var qt = points[q];
                area += pt.x * qt.y - qt.x * pt.y;
            }

            return area * 0.5;
        }

        private bool IsEar(int u, int v, int w, int idxCount, int[] idx) {
            var a = points[idx[u]];
            var b = points[idx[v]];
            var c = points[idx[w]];

            if ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x) < EPSILON ||
                (a - b).sqrMagnitude < EPSILON || (a - c).sqrMagnitude < EPSILON ||
                (b - c).sqrMagnitude < EPSILON)
                return false;

            for (var p = 0; p < idxCount; p++) {
                if (p == u || p == v || p == w) continue;
                var pt = points[idx[p]];
                if (Inside(a, b, c, pt)) return false;
            }

            return true;
        }

        private static bool Inside(Vector2d a, Vector2d b, Vector2d c, Vector2d pt) {
            var ax = c.x - b.x;
            var ay = c.y - b.y;
            var bx = a.x - c.x;
            var by = a.y - c.y;
            var cx = b.x - a.x;
            var cy = b.y - a.y;
            var apx = pt.x - a.x;
            var apy = pt.y - a.y;
            var bpx = pt.x - b.x;
            var bpy = pt.y - b.y;
            var cpx = pt.x - c.x;
            var cpy = pt.y - c.y;

            return ax * bpy - ay * bpx >= 0.0f && bx * cpy - by * cpx >= 0.0f && cx * apy - cy * apx >= 0.0f;
        }
    }
}
