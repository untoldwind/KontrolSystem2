using System.Collections.Generic;
using UnityEngine;

namespace Experiments {
    public abstract class Transform2D : Container2D {
        protected Matrix4x4 transform;
        
        protected Transform2D(Matrix4x4 transform) {
            this.transform = transform;
        }

        public override void OnDraw(GLUIDrawer.GLUIDraw draw) {
            var currentTransform = draw.CurrentTransform;
            GL.PushMatrix();
            GL.MultMatrix(currentTransform * transform);
            draw.CurrentTransform = currentTransform * transform;

            base.OnDraw(draw);

            GL.PopMatrix();
            draw.CurrentTransform = currentTransform;
        }
    }

    public class Translate2D : Transform2D {
        private Vector2 translate;

        public Translate2D(Vector2 translate) : base(Matrix4x4.Translate(translate)) {
            this.translate = translate;
        }

        public Vector2 Translate {
            get => translate;
            set {
                translate = value;
                transform = Matrix4x4.Translate(value);
            }
        }
    }

    public class Scale2D : Transform2D {
        private Vector2 scale;
        private Vector2 pivot;

        public Scale2D(Vector2 scale, Vector2 pivot = default) : base(MakeTransform(scale, pivot)) {
            this.scale = scale;
        }

        public Vector2 Scale {
            get => scale;
            set {
                scale = value;
                transform = MakeTransform(value, pivot);
            }
        }
        
        public Vector2 Pivot {
            get => pivot;
            set {
                pivot = value;
                transform = MakeTransform(scale, value);
            }
        }
        private static Matrix4x4 MakeTransform(Vector2 scale, Vector2 pivot) =>
            Matrix4x4.Translate(pivot) * Matrix4x4.Scale(scale) * Matrix4x4.Translate(-pivot);
    }

    public class Rotate2D : Transform2D {
        private float degrees;
        private Vector2 pivot;

        public Rotate2D(float degrees, Vector2 pivot = default) : base(MakeTransform(degrees, pivot)) {
            this.degrees = degrees;
            this.pivot = pivot;
        }

        public float Degrees {
            get => degrees;
            set {
                degrees = value;
                transform = MakeTransform(value, pivot);
            }
        }
        
        public Vector2 Pivot {
            get => pivot;
            set {
                pivot = value;
                transform = MakeTransform(degrees, value);
            }
        }

        private static Matrix4x4 MakeTransform(float degrees, Vector2 pivot) =>
            Matrix4x4.Translate(pivot) * Matrix4x4.Rotate(Quaternion.Euler(Vector3.forward * degrees)) * Matrix4x4.Translate(-pivot);
    }
}
