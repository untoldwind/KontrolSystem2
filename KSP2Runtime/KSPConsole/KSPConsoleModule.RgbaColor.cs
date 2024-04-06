﻿using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPConsole;

public partial class KSPConsoleModule {
    [KSClass("RgbaColor",
        Description = "Interface color with alpha channel."
    )]
    public class RgbaColor(double red, double green, double blue, double alpha = 1.0) {
        [KSField] public double Red { get; } = DirectBindingMath.Clamp(red, 0, 1);

        [KSField] public double Green { get; } = DirectBindingMath.Clamp(green, 0, 1);

        [KSField] public double Blue { get; } = DirectBindingMath.Clamp(blue, 0, 1);

        [KSField] public double Alpha { get; } = DirectBindingMath.Clamp(alpha, 0, 1);

        public Color Color => new((float)Red, (float)Green, (float)Blue, (float)Alpha);

        /// <summary>
        ///     Returns a string representing the Hex color code "#rrggbb" format
        ///     for the color.  (i.e. RED is "#ff0000").  Note that this cannot represent
        ///     the transparency (alpha) information, and will treat the color as if it was
        ///     fully opaque regardless of whether it really is or not.  Although there have
        ///     been some extensions to the HTML specification that added a fourth byte for
        ///     alpha information, i.e. "#80ff0000" would be a semitransparent red, those never
        ///     got accepted as standard and remain special proprietary extensions.
        /// </summary>
        /// <returns>A color in hexadecimal notation</returns>
        public string ToHexNotation() {
            var redByte = (byte)Math.Min(255, (int)(Red * 255f));
            var greenByte = (byte)Math.Min(255, (int)(Green * 255f));
            var blueByte = (byte)Math.Min(255, (int)(Blue * 255f));
            return $"#{redByte:x2}{greenByte:x2}{blueByte:x2}";
        }
    }
}
