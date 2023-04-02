using System;

namespace Experiments {
    public struct Padding {
        public float top;
        public float bottom;
        public float left;
        public float right;

        public Padding(float top, float bottom, float left, float right) {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
        }

        public Padding Max(Padding other) =>
            new Padding(Math.Max(top, other.top), Math.Max(bottom, other.bottom), Math.Max(left, other.left),
                Math.Max(right, other.right));
    }
}
