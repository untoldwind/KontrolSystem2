using System;
using AwesomeTechnologies.Utility.Quadtree;
using KontrolSystem.KSP.Runtime.KSPMath;
using TMPro;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPTelemetry {
    public class GLUIDrawer {
        private static Material colored;
        private static TMP_FontAsset textFont;
        private static Material textColored;

        public static void Initialize(TMP_FontAsset font) {
            textFont = font;
            colored = new Material(Shader.Find("Hidden/Internal-Colored"));
            colored.hideFlags = HideFlags.HideAndDontSave;
            textColored = new Material(Shader.Find("TextMeshPro/Distance Field Overlay"));
            textColored.hideFlags = HideFlags.HideAndDontSave;
            textColored.SetTexture("_MainTex", textFont.atlasTexture);
        }

        private RenderTexture _renderTexture;

        public GLUIDrawer(int initialWidth, int initialHeight)
        {
            _renderTexture = new RenderTexture(initialWidth, initialHeight, 0);
        }

        public void Resize(int width, int height)
        {
            if (width != _renderTexture.width || height != _renderTexture.height)
            {
                _renderTexture.Release();
                _renderTexture.width = width;
                _renderTexture.height = height;
            }
        }

        public Texture Texture => _renderTexture;

        public GLUIDraw Draw() => new GLUIDraw(_renderTexture);
        
        public void Dispose()
        {
            _renderTexture.Release();
        }
        
        public class GLUIDraw : IDisposable
        {
            private readonly int width;
            private readonly int height;
            
            internal GLUIDraw(RenderTexture renderTexture)
            {
                width = renderTexture.width;
                height = renderTexture.height;
                RenderTexture.active = renderTexture;
                
                GL.Clear(false, true, Color.black);
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, width, height, 0);
            }

            public int Width => width;

            public int Height => height;

            public void Polygon(Vector2[] points, Color color, bool closed = false)
            {
                colored.SetPass(0);
                GL.Begin(GL.LINES);
                GL.Color(color);
                for (int i = 0; i < points.Length - 1; i++) {
                    GL.Vertex3(points[i].x, points[i].y, 0);
                    GL.Vertex3(points[i+1].x, points[i+1].y, 0);
                }

                if (closed) {
                    GL.Vertex3(points[points.Length - 1].x, points[points.Length - 1].y, 0);
                    GL.Vertex3(points[0].x, points[0].y, 0);
                    
                }
                GL.End();
            }

            public void DrawText(Vector2 pos, string text, float size, float degrees, Color color) {
                textColored.SetPass(0);
                GL.Begin(GL.QUADS);
                GL.Color(color);
                var pivot = new Vector3(pos.x, pos.y);
                var x = 0.0f;
                var y = 0.0f;
                var atlasWidth = (float)textFont.atlasWidth;
                var atlasHeight = (float)textFont.atlasHeight;
                var scale = size / textFont.faceInfo.pointSize;
                var rotation = Quaternion.Euler(Vector3.forward * degrees);
                for (int i = 0; i < text.Length; i++) {
                    var glyph = textFont.characterLookupTable[text[i]]?.glyph;

                    if (glyph == null) continue;

                    GL.TexCoord2(glyph.glyphRect.x / atlasWidth, (glyph.glyphRect.y + glyph.glyphRect.height) / atlasHeight);
                    GL.MultiTexCoord2(1, 0, -0.1f);
                    GL.Vertex(pivot + rotation * new Vector3( x , y - scale * glyph.metrics.horizontalBearingY, 0));
                    GL.TexCoord2(glyph.glyphRect.x / atlasWidth, glyph.glyphRect.y / atlasHeight);
                    GL.MultiTexCoord2(1, 0, -0.1f);
                    GL.Vertex(pivot +rotation * new Vector3(x, y + scale * (glyph.metrics.height - glyph.metrics.horizontalBearingY), 0));
                    GL.TexCoord2((glyph.glyphRect.x + glyph.glyphRect.width) / atlasWidth, glyph.glyphRect.y / atlasHeight);
                    GL.MultiTexCoord2(1, 0, -0.1f);
                    GL.Vertex(pivot +rotation * new Vector3(x + scale * glyph.metrics.width, y + scale * (glyph.metrics.height - glyph.metrics.horizontalBearingY), 0));
                    GL.TexCoord2((glyph.glyphRect.x + glyph.glyphRect.width) / atlasWidth,
                        (glyph.glyphRect.y + glyph.glyphRect.height) / atlasHeight);
                    GL.MultiTexCoord2(1, 0, -0.1f);
                    GL.Vertex(pivot + rotation * new Vector3(x + scale * glyph.metrics.width, y - scale * glyph.metrics.horizontalBearingY, 0));

                    x += scale * glyph.metrics.horizontalAdvance;
                }

                GL.End();
            }

            public Rect TextSize(string text, float size, float degrees)
            {
                var x = 0.0f;
                var y = 0.0f;
                var scale = size / textFont.faceInfo.pointSize;
                var rotation = Quaternion.Euler(Vector3.forward * degrees);
                var rect = new Rect(0, 0, 0, 0);

                for (int i = 0; i < text.Length; i++) {
                    var glyph = textFont.characterLookupTable[text[i]]?.glyph;

                    if (glyph == null) continue;
                    
                    var v = rotation * new Vector3( x , y - scale * glyph.metrics.horizontalBearingY, 0);
                    rect.xMin = Math.Min(rect.xMin, v.x);
                    rect.yMin = Math.Min(rect.yMin, v.y);
                    rect.xMax = Math.Max(rect.xMax, v.x);
                    rect.yMax = Math.Max(rect.yMax, v.y);
                    v = rotation * new Vector3(x, y + scale * (glyph.metrics.height - glyph.metrics.horizontalBearingY), 0);
                    rect.xMin = Math.Min(rect.xMin, v.x);
                    rect.yMin = Math.Min(rect.yMin, v.y);
                    rect.xMax = Math.Max(rect.xMax, v.x);
                    rect.yMax = Math.Max(rect.yMax, v.y);
                    v = rotation * new Vector3(x + scale * glyph.metrics.width, y + scale * (glyph.metrics.height - glyph.metrics.horizontalBearingY), 0);
                    rect.xMin = Math.Min(rect.xMin, v.x);
                    rect.yMin = Math.Min(rect.yMin, v.y);
                    rect.xMax = Math.Max(rect.xMax, v.x);
                    rect.yMax = Math.Max(rect.yMax, v.y);
                    v = rotation * new Vector3(x + scale * glyph.metrics.width, y - scale * glyph.metrics.horizontalBearingY, 0);
                    rect.xMin = Math.Min(rect.xMin, v.x);
                    rect.yMin = Math.Min(rect.yMin, v.y);
                    rect.xMax = Math.Max(rect.xMax, v.x);
                    rect.yMax = Math.Max(rect.yMax, v.y);

                    x += scale * glyph.metrics.horizontalAdvance;
                }

                return rect;
            }

            public void Dispose()
            {
                GL.PopMatrix();
                RenderTexture.active = null;
            }
        }

    }
}
