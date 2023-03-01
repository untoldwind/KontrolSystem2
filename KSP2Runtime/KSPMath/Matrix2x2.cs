using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    //  [a    b]
    //  [c    d]
    public struct Matrix2x2 {
        private readonly double a, b, c, d;

        public Matrix2x2(double a, double b, double c, double d) {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public double Determinant => a * d - b * c;

        public Matrix2x2 Inverse {
            get {
                double det = a * d - b * c;
                return new Matrix2x2(d / det, -b / det, -c / det, a / det);
            }
        }

        public static Matrix2x2 operator -(Matrix2x2 m) => new Matrix2x2(-m.a, -m.b, -m.c, -m.d);

        public static Vector2d operator *(Matrix2x2 m, Vector2d vec) =>
            new Vector2d(m.a * vec.x + m.b * vec.y, m.c * vec.x + m.d * vec.y);
    }
}
