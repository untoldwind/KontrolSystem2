using System;
using System.Collections.Generic;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI;

public abstract class UGUILayout(RectTransform containerTransform, UGUILayout.Padding padding = default) {
    public enum Align {
        Start,
        Center,
        End,
        Stretch
    }

    protected readonly RectTransform containerTransform = containerTransform;
    protected readonly List<ILayoutEntry> layoutEntries = [];
    protected readonly Padding padding = padding;

    public abstract Vector2 MinSize { get; }

    public ILayoutEntry Add(GameObject child, Align align, Vector2 minSize, float stretch = 0.0f, Func<Vector2>? layoutAction = null) {
        var entry = new LayoutEntry(this, child, align, minSize, stretch, layoutAction);
        layoutEntries.Add(entry);
        return entry;
    }

    public (E, ILayoutEntry) Add<E>(E element, Align align = Align.Stretch, float stretch = 0.0f) where E : UGUIElement {
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

    private struct LayoutSpace(UGUILayout layout, float minSize, float stretch) : ILayoutEntry {
        public readonly RectTransform? Transform => null;
        public Vector2 MinSize { get; } = new Vector2(minSize, minSize);
        public float Stretch { get; } = stretch;
        public readonly Align Align => Align.Stretch;

        public readonly void Layout() {
        }

        public readonly void Remove() {
            layout.layoutEntries.Remove(this);
        }
    }

    private readonly struct LayoutElement(UGUILayout layout, UGUIElement element, UGUILayout.Align align, float stretch) : ILayoutEntry {
        public RectTransform Transform => element.Transform;
        public Vector2 MinSize => element.MinSize;
        public float Stretch { get; } = stretch;

        public Align Align { get; } = align;

        public void Layout() {
            element.Layout();
        }

        public void Remove() {
            if (layout.layoutEntries.Remove(this))
                element.Destroy();
        }
    }

    private readonly struct LayoutEntry(UGUILayout layout, GameObject child, UGUILayout.Align align, Vector2 minSize, float stretch, Func<Vector2>? layoutAction) : ILayoutEntry {
        public RectTransform Transform => child.GetComponent<RectTransform>();
        public Vector2 MinSize { get; } = minSize;
        public float Stretch { get; } = stretch;
        public Align Align { get; } = align;

        public void Layout() {
            layoutAction?.Invoke();
        }

        public void Remove() {
            if (layout.layoutEntries.Remove(this))
                GameObject.Destroy(child);
        }
    }

    public readonly struct Padding(float top, float bottom, float left, float right) {
        public readonly float top = top;
        public readonly float bottom = bottom;
        public readonly float left = left;
        public readonly float right = right;

        public Padding Max(Padding other) {
            return new Padding(Math.Max(top, other.top), Math.Max(bottom, other.bottom), Math.Max(left, other.left),
                Math.Max(right, other.right));
        }
    }
}
