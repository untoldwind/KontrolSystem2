using System;
using UnityEngine;

namespace Experiments {
    public class UGUILayoutContainer : UGUIElement {
        public readonly UGUILayout layout;

        private UGUILayoutContainer(GameObject gameObject, UGUILayout layout) {
            GameObject = gameObject;
            this.layout = layout;
        }

        public override Vector2 MinSize {
            get {
                var layoutMinSize = layout.MinSize;
                return new Vector2(Math.Max(minSize.x, layoutMinSize.x), Math.Max(minSize.y, layoutMinSize.y));
            }
        } 
        
        public override Vector2 Layout() => layout.Layout();

        public void Add(GameObject child, UGUILayout.Align align, Vector2 minSize, float stretch = 0.0f) =>
            layout.Add(child, align, minSize, stretch);

        public E Add<E>(E element, UGUILayout.Align align = UGUILayout.Align.STRETCH,
            float stretch = 0.0f) where E: UGUIElement {
            layout.Add(element, align, stretch);
            return element;
        }

        public void AddSpace(float minSize, float strech) =>
            layout.AddSpace(minSize, strech);

        public static UGUILayoutContainer Vertical(float gap = 10, Padding padding = default) {
            var gameObject = new GameObject("VerticalContainer", typeof(RectTransform));
            var layout = new UGUIVerticalLayout(gameObject.GetComponent<RectTransform>(), gap, padding);
            
            return new UGUILayoutContainer(gameObject, layout);
        }
        
        public static UGUILayoutContainer Horizontal(float gap = 10, Padding padding = default) {
            var gameObject = new GameObject("HorizontalContainer", typeof(RectTransform));
            var layout = new UGUIHorizontalLayout(gameObject.GetComponent<RectTransform>(), gap, padding);

            return new UGUILayoutContainer(gameObject, layout);
        }
        
        public static UGUILayoutContainer VerticalPanel(float gap = 10, Padding padding = default) {
            var panel = UIFactory.Instance.CreatePanel();
            var layout = new UGUIVerticalLayout(panel.GetComponent<RectTransform>(), gap, padding.Max(new Padding(5,5,5,5)));
            
            return new UGUILayoutContainer(panel, layout);
        }
        
        public static UGUILayoutContainer HorizontalPanel(float gap = 10, Padding padding = default) {
            var panel = UIFactory.Instance.CreatePanel();
            var layout = new UGUIHorizontalLayout(panel.GetComponent<RectTransform>(), gap, padding.Max(new Padding(5,5,5,5)));

            return new UGUILayoutContainer(panel, layout);
        }        
    }
}
