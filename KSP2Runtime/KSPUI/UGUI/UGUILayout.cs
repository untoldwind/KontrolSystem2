using System;
using System.Collections.Generic;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public abstract class UGUILayout {
    public enum Align {
        Start,
        Center,
        End,
        Stretch
    }

    protected readonly RectTransform containerTransform;
    protected readonly List<ILayoutEntry> layoutEntries;
    protected readonly Padding padding;

    protected UGUILayout(RectTransform containerTransform, Padding padding = default) {
        this.containerTransform = containerTransform;
        this.padding = padding;
        layoutEntries = new List<ILayoutEntry>();
    }

    public abstract Vector2 MinSize { get; }

    public ILayoutEntry Add(GameObject child, Align align, Vector2 minSize, float stretch = 0.0f) {
        var entry = new LayoutEntry(this, child, align, minSize, stretch);
        layoutEntries.Add(entry);
        return entry;
    }

    public (E, ILayoutEntry) Add<E>(E element, Align align = Align.Stretch, float stretch = 0.0f) where E : UGUIElement{
        var entry = new LayoutElement(this, element, align, stretch);
        layoutEntries.Add(entry);
        return (element, entry);
    }

    public ILayoutEntry AddSpace(float minSize, float stretch) {
        var entry = new LayoutSpace(this, minSize, stretch);
        layoutEntries.Add(entry);
        return entry;
    }

    public abstract Vector2 Layout();

    public interface ILayoutEntry {
        RectTransform? Transform { get; }

        Vector2 MinSize { get; }

        float Stretch { get; }

        Align Align { get; }

        void Layout();

        void Remove();
    }

    private struct LayoutSpace : ILayoutEntry {
        private readonly UGUILayout layout;
        public LayoutSpace(UGUILayout layout, float minSize, float stretch) {
            this.layout = layout;
            Stretch = stretch;
            MinSize = new Vector2(minSize, minSize);
        }

        public RectTransform? Transform => null;
        public Vector2 MinSize { get; }
        public float Stretch { get; }
        public Align Align => Align.Stretch;

        public void Layout() {
        }

        public void Remove() {
            layout.layoutEntries.Remove(this);
        }
    }

    private readonly struct LayoutElement : ILayoutEntry {
        private readonly UGUILayout layout;
        private readonly UGUIElement element;

        public LayoutElement(UGUILayout layout, UGUIElement element, Align align, float stretch) {
            this.layout = layout;
            this.element = element;
            Align = align;
            Stretch = stretch;
        }

        public RectTransform Transform => element.Transform;
        public Vector2 MinSize => element.MinSize;
        public float Stretch { get; }

        public Align Align { get; }

        public void Layout() {
            element.Layout();
        }

        public void Remove() {
            if (layout.layoutEntries.Remove(this))
                element.Destroy();
        }
    }

    private readonly struct LayoutEntry : ILayoutEntry {
        private readonly UGUILayout layout;
        private readonly GameObject child;

        public LayoutEntry(UGUILayout layout, GameObject child, Align align, Vector2 minSize, float stretch) {
            this.layout = layout;
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

        public void Remove() {
            if (layout.layoutEntries.Remove(this))
                GameObject.Destroy(child);
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

        public Padding Max(Padding other) {
            return new Padding(Math.Max(top, other.top), Math.Max(bottom, other.bottom), Math.Max(left, other.left),
                Math.Max(right, other.right));
        }
    }
}
