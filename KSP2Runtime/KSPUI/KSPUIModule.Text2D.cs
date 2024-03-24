using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Text2D(Vector2d position, string text, double fontSize, Vector2d pivot, double degrees,
        KSPConsoleModule.RgbaColor color) : GLUIDrawer.IGLUIDrawable {
        [KSField] public Vector2d Position { get; set; } = position;

        [KSField] public KSPConsoleModule.RgbaColor Color { get; set; } = color;

        [KSField] public string Text { get; set; } = text;

        [KSField] public double FontSize { get; set; } = fontSize;

        [KSField] public Vector2d Pivot { get; set; } = pivot;

        [KSField] public double Degrees { get; set; } = degrees;

        public void OnDraw(GLUIDrawer.GLUIDraw draw) {
            draw.DrawText(new Vector2((float)Position.x, (float)Position.y), Text, (float)FontSize,
                new Vector2((float)Pivot.x, (float)Pivot.y), (float)Degrees, Color.Color);
        }
    }
}
