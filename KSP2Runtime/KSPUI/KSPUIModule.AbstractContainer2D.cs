using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        public abstract class AbstractContainer2D {
            protected abstract void AddElement(GLUIDrawer.IGLUIDrawable element);

            protected abstract void RemoveAllElements();

            [KSMethod]
            public void Clear() => RemoveAllElements();

            [KSMethod]
            public Line2D AddLine(Vector2d[] points, KSPConsoleModule.RgbaColor strokeColor, bool closed = false, double thickness = 2.0) {
                var element = new Line2D(strokeColor, thickness, closed, points);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public PixelLine2D AddPixelLine(Vector2d[] points, KSPConsoleModule.RgbaColor strokeColor, bool closed = false) {
                var element = new PixelLine2D(strokeColor, closed, points);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public Rect2D AddRect(Vector2d point1, Vector2d point2, KSPConsoleModule.RgbaColor fillColor) {
                var element = new Rect2D(point1, point2, fillColor);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public Polygon2D AddPolygon(Vector2d[] points, KSPConsoleModule.RgbaColor fillColor) {
                var element = new Polygon2D(fillColor, points);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public Text2D AddText(Vector2d position, string text, double fontSize, KSPConsoleModule.RgbaColor color,
                double degrees = 0.0, Vector2d pivot = default) {
                var element = new Text2D(position, text, fontSize, pivot, degrees, color);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public Translate2D AddTranslate(Vector2d translate) {
                var element = new Translate2D(translate);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public Scale2D AddScale(Vector2d scale) {
                var element = new Scale2D(scale);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public Rotate2D AddRotate(double degrees) {
                var element = new Rotate2D(degrees);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public ValueRaster2D AddValueRaster(Vector2d point1, Vector2d point2, double[] values, long width, long height, GradientWrapper gradientWrapper) {
                var element = new ValueRaster2D(values, (int)width, (int)height, gradientWrapper, point1, point2);
                AddElement(element);
                return element;
            }
        }
    }
}
