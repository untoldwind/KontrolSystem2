using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Gradient")]
        public class GradientWrapper {
            private SortedList<double, KSPConsoleModule.RgbaColor> colors;
            private Gradient gradient;
            private bool dirty;

            public GradientWrapper(KSPConsoleModule.RgbaColor start, KSPConsoleModule.RgbaColor end) {
                colors = new SortedList<double, KSPConsoleModule.RgbaColor>();
                colors.Add(0, start);
                colors.Add(1, end);
                gradient = new Gradient();
                dirty = true;
            }

            [KSMethod]
            public bool AddColor(double value, KSPConsoleModule.RgbaColor color) {
                if (value < 0 || value > 1) return false;
                if (!colors.ContainsKey(value) && colors.Count >= 8) return false;
                colors[value] = color;
                dirty = true;
                return true;
            }

            public Gradient Gradient {
                get {
                    if (dirty) {
                        var colorKeys = new GradientColorKey[colors.Count];
                        var alphaKeys = new GradientAlphaKey[colors.Count];

                        for (int i = 0; i < colors.Count; i++) {
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
        }
    }
}
