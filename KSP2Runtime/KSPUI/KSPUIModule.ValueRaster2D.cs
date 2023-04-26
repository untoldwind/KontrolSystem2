using System;
using System.Linq;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class ValueRaster2D : GLUIDrawer.IGLUIDrawable {
            private double[] values;
            private int width;
            private int height;
            private GradientWrapper gradientWrapper;
            private Texture2D texture;
            private Vector2d point1;
            private Vector2d point2;
            private bool dirty;

            public ValueRaster2D(double[] values, int width, int height, GradientWrapper gradientWrapper, Vector2d point1, Vector2d point2) {
                this.values = values;
                this.width = width;
                this.height = height;
                this.gradientWrapper = gradientWrapper;
                this.point1 = point1;
                this.point2 = point2;
                texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                dirty = true;
            }

            [KSField] public long RasterWidth => width;

            [KSField] public long RasterHeight => height;

            [KSField]
            public Vector2d Point1 {
                get => point1;
                set => point1 = value;
            }

            [KSField]
            public Vector2d Point2 {
                get => point2;
                set => point2 = value;
            }

            [KSField]
            public GradientWrapper Gradient {
                get => gradientWrapper;
                set {
                    gradientWrapper = value;
                    dirty = true;
                }
            }

            [KSField]
            public double[] Values {
                get => values;
                set {
                    values = value;
                    dirty = true;
                }
            }

            public void OnDraw(GLUIDrawer.GLUIDraw draw) {
                if (dirty) {
                    Gradient gradient = gradientWrapper.Gradient;
                    Color[] colors = new Color[width * height];
                    double min = values.Length > 0 ? values.Min() : 0;
                    double max = values.Length > 0 ? values.Max() : 1;
                    double range = Math.Max(1e-5, max - min);

                    for (int i = 0; i < colors.Length; i++) {
                        colors[i] = gradient.Evaluate((float)((values[i] - min) / range));
                    }
                    texture.SetPixels(colors);
                    texture.Apply(false);
                    dirty = false;
                }

                Graphics.DrawTexture(
                    new Rect((float)Math.Min(point1.x, point1.x), (float)Math.Min(point1.y, point2.y),
                        (float)Math.Abs(point1.x - point2.x), (float)Math.Abs(point1.y - point2.y)),
                    texture);
            }
        }
    }
}
