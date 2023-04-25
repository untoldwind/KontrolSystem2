using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RawImage), typeof(RectTransform))]
    public class UGUICanvasDrawer : MonoBehaviour {
        private GLUIDrawer drawer;
        private RectTransform rectTransform;
        private List<GLUIDrawer.IGLUIDrawable> elements;
        
        public void Add(GLUIDrawer.IGLUIDrawable element) => elements.Add(element);

        public void Clear() => elements.Clear();

        private void Awake() {
            elements = new List<GLUIDrawer.IGLUIDrawable>();
        }

        private void Start() {
            rectTransform = GetComponent<RectTransform>();
            
            var size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
            drawer = new GLUIDrawer((int)size.x, (int)size.y);
            GetComponent<RawImage>().texture = drawer.Texture;
        }
        
        private void Update() {
            var size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
            
            drawer.Resize((int)size.x, (int)size.y);

            using (var draw = drawer.Draw()) {
                foreach (var element in elements) {
                    draw.Draw(element);
                }
            }
        }
    }
}
