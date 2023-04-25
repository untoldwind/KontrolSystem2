using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class UGUICanvas : UGUIElement {
        protected UGUICanvasDrawer drawer;
        
        protected UGUICanvas(GameObject gameObject, float minWidth, float minHeight) : base(gameObject, Vector2.zero) {
            drawer = gameObject.GetComponent<UGUICanvasDrawer>();

            minSize = new Vector2(minWidth, minHeight);
        }
        
        public void Add(GLUIDrawer.IGLUIDrawable element) => drawer.Add(element);

        public void Clear() => drawer.Clear();
        
        public static UGUICanvas Create(float minWidth, float minHeight) {
            return new UGUICanvas( new GameObject("Canvas", typeof(RawImage), typeof(UGUICanvasDrawer)), minWidth, minHeight);
        }
    }
}
