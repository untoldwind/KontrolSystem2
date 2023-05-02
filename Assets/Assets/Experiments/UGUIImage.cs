using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class UGUIImage : UGUIElement {
        private RawImage rawImage;
        
        protected UGUIImage(GameObject gameObject, float minWidth, float minHeight) : base(gameObject, Vector2.zero) {
            rawImage = gameObject.GetComponent<RawImage>();
            
            minSize = new Vector2(minWidth, minHeight);
        }

        public Color Color {
            get => rawImage.color;
            set => rawImage.color = value;
        }
        
        public static UGUIImage Create(float minWidth, float minHeight) {
            return new UGUIImage( new GameObject("Image", typeof(RawImage)), minWidth, minHeight);
        }
    }
}
