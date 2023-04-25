using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Line2D : GLUIDrawer.IGLUIDrawable {
            private static float EPSILON = 1e-5f;

            private double thickness;
            private bool closed;
            private Vector2d[] points;
            private Vector2d[] triangleStrip;

            public Line2D(KSPConsoleModule.RgbaColor strokeColor, double thickness, bool closed, Vector2d[] points) {
                StrokeColor = strokeColor;
                this.thickness = thickness;
                this.closed = closed;
                this.points = points;
                Triangulate();
            }

            [KSField]
            public KSPConsoleModule.RgbaColor StrokeColor { get; set; }

            [KSField]
            public Vector2d[] Points {
                get => points;
                set {
                    points = value;
                    Triangulate();
                }
            }

            [KSField]
            public bool Closed {
                get => closed;
                set {
                    closed = value;
                    Triangulate();
                }
            }

            [KSField]
            public double Thickness {
                get => thickness;
                set {
                    thickness = value;
                    Triangulate();
                }
            }

            public void OnDraw(GLUIDrawer.GLUIDraw draw) {
                if (triangleStrip.Length < 3) return;

                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(StrokeColor.Color);
                for (int i = 0; i < triangleStrip.Length; i++) {
                    GL.Vertex3((float)triangleStrip[i].x, (float)triangleStrip[i].y, 0);
                }

                GL.End();
            }

            private void Triangulate() {
                triangleStrip = new Vector2d[2 * points.Length + (closed ? 2 : 0)];

                if (points.Length < 2) {
                    triangleStrip = Array.Empty<Vector2d>();
                    return;
                }

                var offset = 0.5f * thickness * NormalTo((points[1] - points[0]).normalized);
                triangleStrip[0] = points[0] - offset;
                triangleStrip[1] = points[0] + offset;
                triangleStrip[2] = points[1] - offset;
                triangleStrip[3] = points[1] + offset;

                for (int i = 2; i < points.Length; i++) {
                    if (MakeOffset(points[i - 2], points[i - 1], points[i], out var offset1, out var offset2)) {
                        triangleStrip[2 * i - 2] = offset1;
                        triangleStrip[2 * i - 1] = offset2;
                    }

                    offset = 0.5f * thickness * NormalTo((points[i] - points[i - 1]).normalized);
                    triangleStrip[2 * i] = points[i] - offset;
                    triangleStrip[2 * i + 1] = points[i] + offset;
                }

                if (closed && points.Length > 2) {
                    Vector2d offset1;
                    Vector2d offset2;
                    if (MakeOffset(points[points.Length - 2], points[points.Length - 1], points[0], out offset1,
                            out offset2)) {
                        triangleStrip[2 * points.Length - 2] = offset1;
                        triangleStrip[2 * points.Length - 1] = offset2;
                    }

                    if (MakeOffset(points[points.Length - 1], points[0], points[1], out offset1, out offset2)) {
                        triangleStrip[0] = offset1;
                        triangleStrip[1] = offset2;
                    }

                    triangleStrip[2 * points.Length] = triangleStrip[0];
                    triangleStrip[2 * points.Length + 1] = triangleStrip[1];
                }
            }

            private static Vector2d NormalTo(Vector2d v) => new Vector2d(-v.y, v.x);

            private bool MakeOffset(Vector2d prev1, Vector2d prev2, Vector2d pt, out Vector2d offset1,
                out Vector2d offset2) {
                var prevDir = (prev2 - prev1).normalized;
                var prevNorm = NormalTo(prevDir);
                var currentDir = (pt - prev2).normalized;
                var currentNorm = NormalTo(currentDir);

                if (AreColinear(prevDir, currentDir)) {
                    offset1 = offset2 = Vector2d.zero;
                    return false;
                }

                var refPt1 = prev2 - 0.5f * thickness * prevNorm;
                var refPt2 = prev2 - 0.5f * thickness * currentNorm;
                offset1 = Intersection(prevDir, refPt1, currentDir, refPt2);

                refPt1 = prev2 + 0.5f * thickness * prevNorm;
                refPt2 = prev2 + 0.5f * thickness * currentNorm;
                offset2 = Intersection(prevDir, refPt1, currentDir, refPt2);

                return true;
            }

            private static bool AreColinear(Vector2d dir1, Vector2d dir2) =>
                Math.Abs(dir1.x * dir2.y - dir1.y * dir2.x) < EPSILON;

            private static Vector2d Intersection(Vector2d dir1, Vector2d pt1, Vector2d dir2, Vector2d pt2) {
                if (dir1.x == 0f) {
                    if (dir2.x == 0f) return Vector2d.Lerp(pt1, pt2, 0.5f); // This case should not happen
                    var y = (dir2.y / dir2.x) * (pt1.x - pt2.x) + pt2.y;
                    return new Vector2d(pt1.x, y);
                }

                if (dir2.x == 0f) {
                    var y = (dir1.y / dir1.x) * (pt2.x - pt1.x) + pt1.y;
                    return new Vector2d(pt2.x, y);
                }

                var a1 = dir1.y / dir1.x;
                var b1 = pt1.y - a1 * pt1.x;
                var a2 = dir2.y / dir2.x;
                var b2 = pt2.y - a2 * pt2.x;
                var x = (b2 - b1) / (a1 - a2);
                return new Vector2d(x, a2 * x + b2);
            }
        }
    }
}
