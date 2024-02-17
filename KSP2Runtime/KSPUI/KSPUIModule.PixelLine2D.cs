using System.Linq;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class PixelLine2D : GLUIDrawer.IGLUIDrawable {
        private Vector2[] drawPoints;
        private Vector2d[] points;

        public PixelLine2D(KSPConsoleModule.RgbaColor strokeColor, bool closed, Vector2d[] points) {
            StrokeColor = strokeColor;
            Closed = closed;
            this.points = points;
            drawPoints = points.Select(p => new Vector2((float)p.x, (float)p.y)).ToArray();
        }

        [KSField] public KSPConsoleModule.RgbaColor StrokeColor { get; set; }

        [KSField]
        public Vector2d[] Points {
            get => points;
            set {
                points = value;
                drawPoints = points.Select(p => new Vector2((float)p.x, (float)p.y)).ToArray();
            }
        }

        [KSField] public bool Closed { get; set; }

        public void OnDraw(GLUIDrawer.GLUIDraw draw) {
            draw.Polyline(drawPoints, StrokeColor.Color, Closed);
        }
    }
}
