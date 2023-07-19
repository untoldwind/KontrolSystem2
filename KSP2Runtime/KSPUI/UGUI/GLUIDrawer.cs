using System;
using TMPro;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public class GLUIDrawer {
        private static Material colored;
        private static TMP_FontAsset textFont;

        public static void Initialize(TMP_FontAsset font) {
            textFont = font;
            colored = new Material(Shader.Find("Hidden/Internal-Colored"));
            colored.hideFlags = HideFlags.HideAndDontSave;
        }

        private readonly RenderTexture renderTexture;

        public GLUIDrawer(int initialWidth, int initialHeight) {
            renderTexture = new RenderTexture(initialWidth, initialHeight, 0);
        }

        public void Resize(int width, int height) {
            if (width != renderTexture.width || height != renderTexture.height) {
                renderTexture.Release();
                renderTexture.width = width;
                renderTexture.height = height;
            }
        }

        public Texture Texture => renderTexture;

        public GLUIDraw Draw() => new GLUIDraw(renderTexture);

        public void Dispose() {
            renderTexture.Release();
        }

        public interface IGLUIDrawable {
            void OnDraw(GLUIDraw draw);
        }

        public class GLUIDraw : IDisposable {
            private readonly int width;
            private readonly int height;
            private Matrix4x4 currentTransform;

            internal GLUIDraw(RenderTexture renderTexture) {
                width = renderTexture.width;
                height = renderTexture.height;
                RenderTexture.active = renderTexture;

                GL.Clear(false, true, Color.black);
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, width, height, 0);
                currentTransform = Matrix4x4.Translate(new Vector3(0, height)) * Matrix4x4.Scale(new Vector3(1, -1));
                GL.MultMatrix(currentTransform);

            }

            public int Width => width;

            public int Height => height;

            public Matrix4x4 CurrentTransform {
                get => currentTransform;
                set => currentTransform = value;
            }

            public void Draw(IGLUIDrawable drawable) {
                colored.SetPass(0);
                drawable.OnDraw(this);
            }

            public void Polyline(Vector2[] points, Color color, bool closed = false) {
                colored.SetPass(0);
                GL.Begin(GL.LINE_STRIP);
                GL.Color(color);
                for (int i = 0; i < points.Length; i++) {
                    GL.Vertex3(points[i].x, points[i].y, 0);
                }

                if (closed) {
                    GL.Vertex3(points[0].x, points[0].y, 0);
                }
                GL.End();
            }

            public void LineTube(Vector3[] errors, Color color) {
                colored.SetPass(0);
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(color);
                for (int i = 0; i < errors.Length; i++) {
                    GL.Vertex3(errors[i].x, errors[i].y, 0);
                    GL.Vertex3(errors[i].x, errors[i].z, 0);
                }
                GL.End();
            }

            public void ConvexPolygon(Vector2[] points, Color color) {
                if (points.Length < 3) return;

                colored.SetPass(0);
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(color);
                int start = 0;
                int end = points.Length - 1;

                while (start <= end) {
                    GL.Vertex3(points[start].x, points[start].y, 0);
                    start++;
                    if (start <= end) {
                        GL.Vertex3(points[end].x, points[end].y, 0);
                        end--;
                    }
                }
                GL.End();
            }

            public void DrawText(Vector2 pos, string text, float size, Vector2 pivot, float degrees, Color color) {
                var textSize = TextSize(text, size);
                var lineHeight = textFont.faceInfo.lineHeight;
                var scale = size / lineHeight;
                textFont.material.SetFloat(ShaderUtilities.ID_ScaleX, 1.0f / scale);
                textFont.material.SetFloat(ShaderUtilities.ID_ScaleY, 1.0f / scale);
                textFont.material.SetPass(0);
                GL.PushMatrix();

                GL.MultMatrix(currentTransform * Matrix4x4.Translate(new Vector3(pos.x, pos.y)) * Matrix4x4.Scale(new Vector3(scale, scale)) * Matrix4x4.Rotate(Quaternion.Euler(Vector3.forward * degrees)) * Matrix4x4.Translate(new Vector3(-pivot.x * textSize.x / scale, -pivot.y * textSize.y / scale)));
                GL.Begin(GL.QUADS);
                GL.Color(color);
                var atlasWidth = (float)textFont.atlasWidth;
                var atlasHeight = (float)textFont.atlasHeight;
                var baseLine = (float)textFont.faceInfo.baseline;
                var x = 0.0f;
                var y = baseLine - textFont.faceInfo.descentLine;
                foreach (var ch in text) {
                    var glyph = TMP_FontAssetUtilities.GetCharacterFromFontAsset(ch, textFont, false,
                        FontStyles.Normal, FontWeight.Regular, out var alternative)?.glyph;

                    if (glyph == null) continue;

                    GL.TexCoord2(glyph.glyphRect.x / atlasWidth, (glyph.glyphRect.y + glyph.glyphRect.height) / atlasHeight);
                    GL.MultiTexCoord2(1, 0, 0.8f);
                    GL.Vertex3(x + glyph.metrics.horizontalBearingX, y + glyph.metrics.horizontalBearingY, 0);
                    GL.TexCoord2(glyph.glyphRect.x / atlasWidth, glyph.glyphRect.y / atlasHeight);
                    GL.MultiTexCoord2(1, 0, 0.8f);
                    GL.Vertex3(x + glyph.metrics.horizontalBearingX, y - glyph.metrics.height + glyph.metrics.horizontalBearingY, 0);
                    GL.TexCoord2((glyph.glyphRect.x + glyph.glyphRect.width) / atlasWidth, glyph.glyphRect.y / atlasHeight);
                    GL.MultiTexCoord2(1, 0, 0.8f);
                    GL.Vertex3(x + glyph.metrics.horizontalBearingX + glyph.metrics.width, y - glyph.metrics.height + glyph.metrics.horizontalBearingY, 0);
                    GL.TexCoord2((glyph.glyphRect.x + glyph.glyphRect.width) / atlasWidth,
                        (glyph.glyphRect.y + glyph.glyphRect.height) / atlasHeight);
                    GL.MultiTexCoord2(1, 0, 0.8f);
                    GL.Vertex3(x + glyph.metrics.horizontalBearingX + glyph.metrics.width, y + glyph.metrics.horizontalBearingY, 0);

                    x += glyph.metrics.horizontalAdvance;
                }

                GL.End();
                GL.PopMatrix();
            }

            private Vector2 TextSize(string text, float size) {
                var lineHeight = textFont.faceInfo.lineHeight;
                var scale = size / lineHeight;
                var x = 0.0f;

                foreach (var ch in text) {
                    var glyph = TMP_FontAssetUtilities.GetCharacterFromFontAsset(ch, textFont, false,
                        FontStyles.Normal, FontWeight.Regular, out var alternative)?.glyph;

                    if (glyph == null) continue;

                    x += scale * glyph.metrics.horizontalAdvance;
                }

                return new Vector2(x, size);
            }

            public void Dispose() {
                GL.PopMatrix();
                RenderTexture.active = null;
            }
        }
    }
}
