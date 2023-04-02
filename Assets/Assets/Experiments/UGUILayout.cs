using System.Collections.Generic;
using UnityEngine;

namespace Experiments {
    public abstract class UGUILayout {
        protected readonly RectTransform containerTransform;
        protected readonly Padding padding;
        protected List<ILayoutEntry> layoutEntries;

        protected UGUILayout(RectTransform containerTransform, Padding padding = default) {
            this.containerTransform = containerTransform;
            this.padding = padding;
            layoutEntries = new List<ILayoutEntry>();
        }

        public void Add(GameObject child, Align align, Vector2 minSize, float stretch = 0.0f) =>
            layoutEntries.Add(new LayoutEntry(child, align, minSize, stretch));

        public E Add<E>(E element, Align align = Align.STRETCH, float stretch = 0.0f) where E : UGUIElement {
            layoutEntries.Add(new LayoutElement(element, align, stretch));
            return element;
        }

        public void AddSpace(float minSize, float strech) =>
            layoutEntries.Add(new LayoutSpace(minSize, strech));

        public abstract Vector2 MinSize { get; }
        
        public abstract Vector2 Layout();
        
        protected interface ILayoutEntry {
            RectTransform Transform { get; }

            Vector2 MinSize { get; }
            
            float Strech { get; }
            
            Align Align { get; }

            void Layout();
        }
        
        protected struct LayoutSpace : ILayoutEntry {
            private Vector2 minSize;
            private float stretch;

            public LayoutSpace(float minSize, float stretch) {
                this.stretch = stretch;
                this.minSize = new Vector2(minSize, minSize);
            }

            public RectTransform Transform => null;
            public Vector2 MinSize => minSize;
            public float Strech => stretch;
            public Align Align => Align.STRETCH;
            public void Layout() {
            }
        }
        
        protected struct LayoutElement : ILayoutEntry {
            private UGUIElement element;
            private Align align;
            private float stretch;

            public LayoutElement(UGUIElement element, Align align, float stretch) {
                this.element = element;
                this.align = align;
                this.stretch = stretch;
            }
            
            public RectTransform Transform => element.Transform;
            public Vector2 MinSize => element.MinSize;
            public float Strech => stretch;
            public Align Align => align;
            public void Layout() => element.Layout();
        }
        
        protected struct LayoutEntry : ILayoutEntry {
            private GameObject child;
            private Align align;
            private Vector2 minSize;
            private float stretch;

            public LayoutEntry(GameObject child, Align align, Vector2 minSize, float stretch) {
                this.child = child;
                this.align = align;
                this.minSize = minSize;
                this.stretch = stretch;
            }

            public RectTransform Transform => child.GetComponent<RectTransform>();
            public Vector2 MinSize => minSize;
            public float Strech => stretch;
            public Align Align => align;
            public void Layout() {
            }
        }

        public enum Align {
            START,
            CENTER,
            END,
            STRETCH,
        }
    }
}
