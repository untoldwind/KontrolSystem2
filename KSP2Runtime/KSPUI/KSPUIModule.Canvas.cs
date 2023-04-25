using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass]
        public class Canvas : AbstractContainer2D {
            private AbstractContainer parent;
            private UGUICanvas canvas;

            public Canvas(AbstractContainer parent, UGUICanvas canvas) {
                this.parent = parent;
                this.canvas = canvas;
            }

            [KSField]
            public Vector2d MinSize {
                get => new Vector2d(canvas.MinSize.x, canvas.MinSize.y);
                set {
                    canvas.MinSize = new Vector2((float)value.x, (float)value.y);
                    parent.Root.Layout();
                }
            }

            [KSField] public double Width => canvas.Width;

            [KSField] public double Height => canvas.Height;

            protected override void AddElement(GLUIDrawer.IGLUIDrawable element) => canvas.Add(element);

            protected override void RemoveAllElements() => canvas.Clear();
        }
    }
}
