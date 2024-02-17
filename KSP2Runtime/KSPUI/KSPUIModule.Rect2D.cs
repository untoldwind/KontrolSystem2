using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Rect2D : GLUIDrawer.IGLUIDrawable {
        private Vector2[]? drawPoints;
        private Vector2d point1;
        private Vector2d point2;

        public Rect2D(Vector2d point1, Vector2d point2, KSPConsoleModule.RgbaColor fillColor) {
            this.point1 = point1;
            this.point2 = point2;
            FillColor = fillColor;
            UpdateDrawPoints();
        }

        [KSField]
        public Vector2d Point1 {
            get => point1;
            set {
                point1 = value;
                UpdateDrawPoints();
            }
        }

        [KSField]
        public Vector2d Point2 {
            get => point2;
            set {
                point2 = value;
                UpdateDrawPoints();
            }
        }

        [KSField] public KSPConsoleModule.RgbaColor FillColor { get; set; }

        public void OnDraw(GLUIDrawer.GLUIDraw draw) {
            draw.ConvexPolygon(drawPoints!, FillColor.Color);
        }

        private void UpdateDrawPoints() {
            drawPoints = new[] {
                new Vector2((float)point1.x, (float)point1.y),
                new Vector2((float)point2.x, (float)point1.y),
                new Vector2((float)point2.x, (float)point2.y),
                new Vector2((float)point1.x, (float)point2.y)
            };
        }
    }
}
