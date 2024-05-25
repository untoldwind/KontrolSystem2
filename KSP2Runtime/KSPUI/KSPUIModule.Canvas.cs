﻿using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    [KSClass]
    public class Canvas(KSPUIModule.AbstractContainer parent, UGUICanvas canvas, UGUILayout.ILayoutEntry entry) : AbstractContainer2D {
        [KSField(Description = "Minimum size of the canvas.")]
        public Vector2d MinSize {
            get => new(canvas.MinSize.x, canvas.MinSize.y);
            set {
                canvas.MinSize = new Vector2((float)value.x, (float)value.y);
                parent.Root.Layout();
            }
        }

        [KSField]
        public bool Visible {
            get => canvas.Visible;
            set {
                canvas.Visible = value;
                parent.Root.Layout();
            }
        }

        [KSField(Description = "Current width of the canvas (determined by the surrounding container)")]
        public double Width => canvas.Width;

        [KSField(Description = "Current height of the canvas (determined by the surrounding container)")]
        public double Height => canvas.Height;

        protected override void AddElement(GLUIDrawer.IGLUIDrawable element) {
            canvas.Add(element);
        }

        protected override void RemoveAllElements() {
            canvas.Clear();
        }

        [KSMethod]
        public void Remove() {
            entry.Remove();
            parent.Root.Layout();
        }
    }
}
