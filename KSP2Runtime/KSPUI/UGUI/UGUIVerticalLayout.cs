using System;
using System.Linq;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public class UGUIVerticalLayout : UGUILayout {
        private float gap;

        public UGUIVerticalLayout(RectTransform containerTransform, float gap = 10, Padding padding = default) : base(containerTransform, padding) {
            this.gap = gap;
        }

        public override Vector2 MinSize {
            get {
                var minSize = new Vector2(padding.left + padding.right, padding.top + padding.bottom);
                foreach (var entry in layoutEntries) {
                    var entryMinSize = entry.MinSize;
                    minSize.x = Math.Max(minSize.x + padding.left + padding.right, entryMinSize.x);
                    minSize.y += entryMinSize.y;
                }

                if (layoutEntries.Count > 1) {
                    minSize.y += (layoutEntries.Count - 1) * gap;
                }

                return minSize;
            }
        }

        public override Vector2 Layout() {
            var minSize = MinSize;
            var parentRect = containerTransform.rect;
            containerTransform.sizeDelta += new Vector2(
                Math.Max(parentRect.width, minSize.x) - parentRect.width,
                Math.Max(parentRect.height, minSize.y) - parentRect.height
            );
            parentRect = containerTransform.rect;

            var sumStrech = layoutEntries.Select(entry => entry.Stretch).Sum();
            var topAnchor = 1f;
            var offset = parentRect.height - padding.top + 0.5f * gap;
            var availableHeight = parentRect.height - padding.top - padding.bottom - layoutEntries.Where(entry => entry.Stretch == 0).Select(entry => entry.MinSize.y).Sum();
            if (layoutEntries.Count > 1) {
                availableHeight -= (layoutEntries.Count - 1) * gap;
            }

            foreach (var entry in layoutEntries) {
                var relativeStretch = entry.Stretch > 0 ? entry.Stretch / sumStrech : 0;
                var height = Math.Max(entry.MinSize.y, availableHeight * relativeStretch);
                var entryTransform = entry.Transform;

                if (entryTransform != null) {
                    entryTransform.SetParent(containerTransform);
                    var anchorMin = Vector2.zero;
                    var anchorMax = Vector2.zero;
                    var offsetMin = Vector2.zero;
                    var offsetMax = Vector2.zero;
                    switch (entry.Align) {
                    case Align.Start:
                        anchorMin.x = 0;
                        anchorMax.x = 0;
                        offsetMin.x = padding.left;
                        offsetMax.x = padding.left + entry.MinSize.x;
                        break;
                    case Align.Center:
                        anchorMin.x = 0.5f;
                        anchorMax.x = 0.5f;
                        offsetMin.x = -0.5f * entry.MinSize.x;
                        offsetMax.x = 0.5f * entry.MinSize.x;
                        break;
                    case Align.End:
                        anchorMin.x = 1;
                        anchorMax.x = 1;
                        offsetMin.x = -padding.right - entry.MinSize.x;
                        offsetMax.x = -padding.right;
                        break;
                    case Align.Stretch:
                        anchorMin.x = 0;
                        anchorMax.x = 1;
                        offsetMin.x = padding.left;
                        offsetMax.x = -padding.right;
                        break;
                    }

                    var top = offset - 0.5f * gap;
                    var bottom = offset - height - 0.5f * gap;
                    anchorMax.y = topAnchor;
                    anchorMin.y = topAnchor - relativeStretch;
                    var anchorMinPos = anchorMin.y * parentRect.height;
                    var anchorMaxPos = anchorMax.y * parentRect.height;
                    offsetMax.y = top - anchorMaxPos;
                    offsetMin.y = bottom - anchorMinPos;
                    entryTransform.localPosition = Vector2.zero;
                    entryTransform.anchorMin = anchorMin;
                    entryTransform.anchorMax = anchorMax;
                    entryTransform.offsetMin = offsetMin;
                    entryTransform.offsetMax = offsetMax;
                }

                topAnchor -= relativeStretch;
                offset -= height + gap;
            }

            foreach (var entry in layoutEntries) {
                entry.Layout();
            }

            return minSize;
        }
    }
}
