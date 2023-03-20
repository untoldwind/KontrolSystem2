using System;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using KSP.Map;
using KSP.Sim;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("DebugPath",
            Description = "Represents a debugging path in the current scene."
        )]
        public class PathRenderer : IMarker {
            [KSField(Description = "The color of the debugging path")]
            public KSPConsoleModule.RgbaColor Color { get; set; }

            [KSField(Description = "The width of the debugging path")]
            public double Width { get; set; }

            [KSField] public Position[] Path { get; set; }

            private bool enable;
            
            private LineRenderer line;

            private GameObject lineObj;
            
            public PathRenderer(Position[] path, KSPConsoleModule.RgbaColor color, double width) {
                Path = path;                
                Color = color;
                Width = width;
            }

            [KSField(Description = "Controls if the debug path is currently visible (initially `true`)")]
            public bool Visible {
                get => enable;
                set {
                    if (value) {
                        if (line == null ) {
                            lineObj = new GameObject("KS2DebugPath", typeof(LineRenderer));

                            line = lineObj.GetComponent<LineRenderer>();

                            line.useWorldSpace = false;
                            
                            line.material = GLUtils.Colored;

                            RenderValues();
                        }

                        line.enabled = true;
                    } else {
                        if (line != null) {
                            line.enabled = false;
                            GameObject.Destroy(line);
                            line = null;
                        }

                        lineObj = null;
                    }

                    enable = value;
                }
            }
            
            [KSMethod]
            public void Remove() => KSPContext.CurrentContext.RemoveMarker(this);

            public void OnUpdate() {
                if (line == null) return;
                if (!enable) return;
                
                RenderPointCoords();
                LabelPlacement();
            }

            public void OnRender() {
            }

            private void RenderPointCoords() {
                if (line != null ) {
                    double mapLengthMult = 1.0; // for scaling when on map view.
                    double mapWidthMult = 1.0; // for scaling when on map view.
                    float useWidth;
                    Position[] positions = Path;
                    int nStep = Math.Max(1, positions.Length / 100);
                    
                    Vector3d[] points;
                    
                    
                    if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out MapCore mapCore) && mapCore.IsEnabled) {
                        var space = mapCore.map3D.GetSpaceProvider();

                        points = Path.Where((x, i) => i % nStep == 0).Select(p => space.TranslateSimPositionToMapPosition(p)).ToArray();
                        lineObj.layer = 27;
                        mapWidthMult = 1500 / space.Map3DScaleInv;
                    } else {
                        var frame = KSPContext.CurrentContext.ActiveVessel.transform?.coordinateSystem;
                        points = Path.Where((x, i) => i % nStep == 0).Select(p => frame.ToLocalPosition(p)).ToArray();
                        lineObj.layer = 0;
                    }
                    
                    useWidth = (float)(Width * mapWidthMult);

                    
                    // Position the arrow line:
                    line.positionCount = points.Length;
                    line.startWidth = useWidth;
                    line.endWidth = useWidth;
                    for (int i = 0; i < points.Length; i++) {
                        line.SetPosition(i, points[i]);
                    }
                }
            }            

            public void RenderColor() {
                Color c1 = Color.Color;
                Color c2 = Color.Color;
                c1.a = c1.a * (float)0.25;
                Color lCol =
                    UnityEngine.Color.Lerp(c2, UnityEngine.Color.white, 0.7f); // "whiten" the label color a lot.

                if (line != null) {
                    // If Wiping, then the line has the fade effect from color c1 to color c2,
                    // else it stays at c2 the whole way:
                    line.startColor = c1;
                    line.endColor = c2;
                }
            }
            
            public void RenderValues() {
                RenderPointCoords();
                RenderColor();
                LabelPlacement();
            }
            
            private void LabelPlacement() {
                
            }            
        }
    }
}
