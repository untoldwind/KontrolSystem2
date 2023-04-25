using System;
using System.Linq;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI.UGUI {
    public class UGUIHorizontalLayout : UGUILayout {
        private float gap;

        public UGUIHorizontalLayout(RectTransform containerTransform, float gap = 10, Padding padding = default) : base(containerTransform, padding) {
            this.gap = gap;
        }

        public override Vector2 MinSize {
            get {
                var minSize = new Vector2(padding.left + padding.right, padding.top + padding.bottom);
                foreach (var entry in layoutEntries) {
                    var entryMinSize = entry.MinSize;
                    minSize.x += entryMinSize.x;
                    minSize.y = Math.Max(minSize.y, entryMinSize.y + padding.top + padding.bottom);
                }

                if (layoutEntries.Count > 1) {
                    minSize.x += (layoutEntries.Count - 1) * gap;
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
            var leftAnchor = 0f;
            var offset = padding.left - 0.5f * gap;
            var availableWidth = parentRect.width - padding.left - padding.right - layoutEntries.Where(entry => entry.Stretch == 0).Select(entry => entry.MinSize.x).Sum();
            if (layoutEntries.Count > 1) {
                availableWidth -= (layoutEntries.Count - 1) * gap;
            }

            foreach (var entry in layoutEntries) {
                var relativeStretch = entry.Stretch > 0 ? entry.Stretch / sumStrech : 0;
                var width = Math.Max(entry.MinSize.x, availableWidth * relativeStretch);
                var entryTransform = entry.Transform;

                if (entryTransform != null) {
                    entryTransform.SetParent(containerTransform);
                    var anchorMin = Vector2.zero;
                    var anchorMax = Vector2.zero;
                    var offsetMin = Vector2.zero;
                    var offsetMax = Vector2.zero;
                    switch (entry.Align) {
                    case Align.Start:
                        anchorMin.y = 1;
                        anchorMax.y = 1;
                        offsetMin.y = 1 - padding.top - entry.MinSize.y;
                        offsetMax.y = 1 - padding.top;
                        break;
                    case Align.Center:
                        anchorMin.y = 0.5f;
                        anchorMax.y = 0.5f;
                        offsetMin.y = -0.5f * entry.MinSize.y;
                        offsetMax.y = 0.5f * entry.MinSize.y;
                        break;
                    case Align.End:
                        anchorMin.y = 0;
                        anchorMax.y = 0;
                        offsetMin.y = padding.bottom;
                        offsetMax.y = padding.bottom + entry.MinSize.y;
                        break;
                    case Align.Stretch:
                        anchorMin.y = 0;
                        anchorMax.y = 1;
                        offsetMin.y = padding.bottom;
                        offsetMax.y = -padding.top;
                        break;
                    }

                    var left = offset + 0.5f * gap;
                    var right = offset + width + 0.5f * gap;
                    anchorMin.x = leftAnchor;
                    anchorMax.x = leftAnchor + relativeStretch;
                    var anchorMinPos = anchorMin.x * parentRect.width;
                    var anchorMaxPos = anchorMax.x * parentRect.width;
                    offsetMin.x = left - anchorMinPos;
                    offsetMax.x = right - anchorMaxPos;
                    entryTransform.localPosition = Vector2.zero;
                    entryTransform.anchorMin = anchorMin;
                    entryTransform.anchorMax = anchorMax;
                    entryTransform.offsetMin = offsetMin;
                    entryTransform.offsetMax = offsetMax;
                }

                leftAnchor += relativeStretch;
                offset += width + gap;
            }

            foreach (var entry in layoutEntries) {
                entry.Layout();
            }

            return minSize;
        }
    }
}
