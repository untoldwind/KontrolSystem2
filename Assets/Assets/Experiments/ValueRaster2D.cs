using System;
using System.Linq;
using UnityEngine;

namespace Experiments {
    public class ValueRaster2D : GLUIDrawer.IGLUIDrawable {
        private double[] values;
        private int width;
        private int height;
        private GradientWrapper gradientWrapper;
        private Texture2D texture;
        private Vector2 point1;
        private Vector2 point2;
        private bool dirty;
        
        public ValueRaster2D(double[] values, int width, int height, GradientWrapper gradientWrapper, Vector2 point1, Vector2 point2) {
            this.values = values;
            this.width = width;
            this.height = height;
            this.gradientWrapper = gradientWrapper;
            this.point1 = point1;
            this.point2 = point2;
            texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            dirty = true;
        }
        
        public void OnDraw(GLUIDrawer.GLUIDraw draw) {
            if (dirty) {
                Gradient gradient = gradientWrapper.Gradient;
                Color[] colors = new Color[width * height];
                double min = values.Length > 0 ? values.Min() : 0;
                double max = values.Length > 0 ? values.Max() : 1;
                double range = Math.Max(1e-5, max - min);
                    
                for (int i = 0; i < colors.Length; i++) {
                    colors[i] = gradient.Evaluate((float)((values[i] - min) / range));
                }
                texture.SetPixels(colors);
                texture.Apply(false);
                dirty = false;
            }
            
            Graphics.DrawTexture(new Rect(Math.Min(point1.x, point1.x), Math.Min(point1.y, point2.y), Math.Abs(point1.x - point2.x), Math.Abs(point1.y - point2.y)), texture);
        }
    }
}
