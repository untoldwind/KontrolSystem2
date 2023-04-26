using UnityEngine;

namespace Experiments {
    public class RgbaColor {
        public double Red { get; }

        public double Green { get; }

        public double Blue { get; }

        public double Alpha { get; }

        public RgbaColor(double red, double green, double blue, double alpha = 1.0) {
            Red = red;
            Blue = blue;
            Green = green;
            Alpha = alpha;
        }

        public Color Color => new Color((float)Red, (float)Green, (float)Blue, (float)Alpha);
    }
}
