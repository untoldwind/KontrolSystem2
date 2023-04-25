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
            public Line2D AddLine(Vector2d[] points, KSPConsoleModule.RgbaColor color, bool closed = false, double thickness = 2.0) {
                var element = new Line2D(color, thickness, closed, points);
                AddElement(element);
                return element;
            }

            [KSMethod]
            public Polygon2D AddPolygon(Vector2d[] points, KSPConsoleModule.RgbaColor color) {
                var element = new Polygon2D(color, points);
                AddElement(element);
                return element;
            }
        }
    }
}
