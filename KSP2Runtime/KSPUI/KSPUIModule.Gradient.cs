using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass("Gradient")]
    public class GradientWrapper(KSPConsoleModule.RgbaColor start, KSPConsoleModule.RgbaColor end) {
        private readonly SortedList<double, KSPConsoleModule.RgbaColor> colors = new() {
            { 0, start },
            { 1, end }
        };
        private bool dirty = true;
        private readonly Gradient gradient = new();

        public Gradient Gradient {
            get {
                if (dirty) {
                    var colorKeys = new GradientColorKey[colors.Count];
                    var alphaKeys = new GradientAlphaKey[colors.Count];

                    for (var i = 0; i < colors.Count; i++) {
                        colorKeys[i].time = alphaKeys[i].time = (float)colors.Keys[i];
                        var color = colors.Values[i];
                        colorKeys[i].color = color.Color;
                        alphaKeys[i].alpha = (float)color.Alpha;
                    }

                    gradient.SetKeys(colorKeys, alphaKeys);
                    dirty = false;
                }

                return gradient;
            }
        }

        [KSMethod]
        public bool AddColor(double value, KSPConsoleModule.RgbaColor color) {
            if (value < 0 || value > 1) return false;
            if (!colors.ContainsKey(value) && colors.Count >= 8) return false;
            colors[value] = color;
            dirty = true;
            return true;
        }
    }
}
