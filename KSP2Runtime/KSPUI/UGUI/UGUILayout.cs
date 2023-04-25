using System;
using System.Collections.Generic;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public abstract class UGUILayout {
        protected readonly RectTransform containerTransform;
        protected readonly Padding padding;
        protected readonly List<ILayoutEntry> layoutEntries;

        protected UGUILayout(RectTransform containerTransform, Padding padding = default) {
            this.containerTransform = containerTransform;
            this.padding = padding;
            layoutEntries = new List<ILayoutEntry>();
        }

        public void Add(GameObject child, Align align, Vector2 minSize, float stretch = 0.0f) =>
            layoutEntries.Add(new LayoutEntry(child, align, minSize, stretch));

        public E Add<E>(E element, Align align = Align.Stretch, float stretch = 0.0f) where E : UGUIElement {
            layoutEntries.Add(new LayoutElement(element, align, stretch));
            return element;
        }

        public void AddSpace(float minSize, float stretch) =>
            layoutEntries.Add(new LayoutSpace(minSize, stretch));

        public abstract Vector2 MinSize { get; }

        public abstract Vector2 Layout();

        protected interface ILayoutEntry {
            RectTransform Transform { get; }

            Vector2 MinSize { get; }

            float Stretch { get; }

            Align Align { get; }

            void Layout();
        }

        private struct LayoutSpace : ILayoutEntry {
            public LayoutSpace(float minSize, float stretch) {
                Stretch = stretch;
                MinSize = new Vector2(minSize, minSize);
            }

            public RectTransform Transform => null;
            public Vector2 MinSize { get; }
            public float Stretch { get; }
            public Align Align => Align.Stretch;
            public void Layout() {
            }
        }

        private readonly struct LayoutElement : ILayoutEntry {
            private readonly UGUIElement element;

            public LayoutElement(UGUIElement element, Align align, float stretch) {
                this.element = element;
                Align = align;
                Stretch = stretch;
            }

            public RectTransform Transform => element.Transform;
            public Vector2 MinSize => element.MinSize;
            public float Stretch { get; }

            public Align Align { get; }

            public void Layout() => element.Layout();
        }

        private readonly struct LayoutEntry : ILayoutEntry {
            private readonly GameObject child;

            public LayoutEntry(GameObject child, Align align, Vector2 minSize, float stretch) {
                this.child = child;
                Align = align;
                MinSize = minSize;
                Stretch = stretch;
            }

            public RectTransform Transform => child.GetComponent<RectTransform>();
            public Vector2 MinSize { get; }
            public float Stretch { get; }
            public Align Align { get; }

            public void Layout() {
            }
        }

        public readonly struct Padding {
            public readonly float top;
            public readonly float bottom;
            public readonly float left;
            public readonly float right;

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

        public enum Align {
            Start,
            Center,
            End,
            Stretch,
        }
    }
}
