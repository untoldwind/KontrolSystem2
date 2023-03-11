using System;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    public partial class KSPConsoleModule {
        [KSClass("RgbaColor",
            Description = "Interface color with alpha channel."
        )]
        public class RgbaColor {
            [KSField] public double Red { get; }

            [KSField] public double Green { get; }

            [KSField] public double Blue { get; }

            [KSField] public double Alpha { get; }

            public RgbaColor(double red, double green, double blue, double alpha = 1.0) {
                Red = DirectBindingMath.Clamp(red, 0, 1);
                Green = DirectBindingMath.Clamp(green, 0, 1);
                Blue = DirectBindingMath.Clamp(blue, 0, 1);
                Alpha = DirectBindingMath.Clamp(alpha, 0, 1);
            }

            public Color Color => new Color((float)Red, (float)Green, (float)Blue, (float)Alpha);

            /// <summary>
            /// Returns a string representing the Hex color code "#rrggbb" format
            /// for the color.  (i.e. RED is "#ff0000").  Note that this cannot represent
            /// the transparency (alpha) information, and will treat the color as if it was
            /// fully opaque regardless of whether it really is or not.  Although there have
            /// been some extensions to the HTML specification that added a fourth byte for
            /// alpha information, i.e. "#80ff0000" would be a semitransparent red, those never
            /// got accepted as standard and remain special proprietary extensions.
            /// </summary>
            /// <returns>A color in hexadecimal notation</returns>
            public String ToHexNotation() {
                var redByte = (byte)Math.Min(255, (int)(Red * 255f));
                var greenByte = (byte)Math.Min(255, (int)(Green * 255f));
                var blueByte = (byte)Math.Min(255, (int)(Blue * 255f));
                return $"#{redByte:x2}{greenByte:x2}{blueByte:x2}";
            }
        }
    }
}
