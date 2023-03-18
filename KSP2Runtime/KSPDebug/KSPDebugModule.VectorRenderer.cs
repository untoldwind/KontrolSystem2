using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using KSP.Game;
using KSP.Map;
using KSP.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("DebugVector",
            Description = "Represents a debugging vector in the current scene."
        )]
        public class VectorRenderer : IMarker {
            [KSField(Description = "The color of the debugging vector")]
            public KSPConsoleModule.RgbaColor Color { get; set; }

            [KSField] public double Scale { get; set; }

            [KSField(Description = "The width of the debugging vector")]
            public double Width { get; set; }

            [KSField(Description = "Controls if an arrow should be drawn at the end.")]
            public bool Pointy { get; set; }

            private bool enable;

            private Func<Position> startProvider;
            private Func<Position> endProvider;
            
            private LineRenderer line;
            private LineRenderer hat;

            private GameObject lineObj;
            private GameObject hatObj;
            private GameObject labelObj;

            private TextMesh label;

            private string labelStr;

            private Vector3 labelLocation;

            public VectorRenderer(Func<Position> startProvider, Func<Position> endProvider,
                KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) {
                this.startProvider = startProvider;
                this.endProvider = endProvider;
                Color = color;
                Scale = 1.0;
                Width = width;
                Pointy = pointy;
                labelStr = label;                
            }

            [KSField(Description = "Controls if the debug-vector is currently visible (initially `true`)")]
            public bool Visible {
                get { return enable; }
                set {
                    if (value) {
                        if (line == null || hat == null) {
                            lineObj = new GameObject("vecdrawLine");
                            hatObj = new GameObject("vecdrawHat");

                            line = lineObj.AddComponent<LineRenderer>();
                            hat = hatObj.AddComponent<LineRenderer>();

                            labelObj = new GameObject("vecdrawLabel", typeof(TextMesh));
                            label = labelObj.GetComponent<TextMesh>();

                            line.useWorldSpace = false;
                            hat.useWorldSpace = false;
                            
                            // Note the Shader name string below comes from Kerbal's packaged shaders the
                            // game ships with - there's many to choose from but they're not documented what
                            // they are.  This was settled upon via trial and error:
                            // Additionally, Note that in KSP 1.8 because of the Unity update, some of these
                            // shaders Unity previously supplied were removed from Unity's DLLs.  SQUAD packaged them
                            // inside its own DLLs in 1.8 for modders who had been using them.  But because of that,
                            // mods have to use this different path to get to them:
                            Shader vecShader = Shader.Find("Sprites/Default"); // for when KSP version is < 1.8
                            if (vecShader != null) {
                                line.material = new Material(vecShader);
                                hat.material = new Material(vecShader);
                            }

                            // This is how font loading would work if other fonts were available in KSP:
                            // Font lblFont = (Font)Resources.Load( "Arial", typeof(Font) );
                            // SafeHouse.Logger.Log( "lblFont is " + (lblFont == null ? "null" : "not null") );
                            // _label.font = lblFont;

                            label.text = labelStr;
                            label.alignment = TextAlignment.Center;

                            RenderValues();
                        }

                        line.enabled = true;
                        hat.enabled = Pointy;
                    } else {
                        if (label != null) {
                            GameObject.Destroy(label);
                            label = null;
                        }

                        if (hat != null) {
                            hat.enabled = false;
                            GameObject.Destroy(hat);
                            hat = null;
                        }

                        if (line != null) {
                            line.enabled = false;
                            GameObject.Destroy(line);
                            line = null;
                        }

                        labelObj = null;
                        hatObj = null;
                        lineObj = null;
                    }

                    enable = value;
                }
            }
            
            public void OnUpdate() {
                if (line == null || hat == null) return;
                if (!enable) return;
                
                RenderPointCoords();
                LabelPlacement();
            }

            public void OnRender() {
            }

            [KSField(Description = "The current starting position of the debugging vector.")]
            public Position Start {
                get => startProvider();
                set => startProvider = () => value;
            }


            [KSField(Description = "The current end position of the debugging vector.")]
            public Position End {
                get => endProvider();
                set => endProvider = () => value;
            }
            
            private void RenderPointCoords() {
                if (line != null && hat != null) {
                    double mapLengthMult = 1.0; // for scaling when on map view.
                    double mapWidthMult = 1.0; // for scaling when on map view.
                    float useWidth;
                    Vector3d start;
                    Vector3d vector;

                    if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out MapCore mapCore) && mapCore.IsEnabled) {
                        var space = mapCore.map3D.GetSpaceProvider();

                        start = space.TranslateSimPositionToMapPosition(startProvider());
                        vector = space.TranslateSimPositionToMapPosition(endProvider()) - start;
                        lineObj.layer = 27;
                        labelObj.layer = 27;
                        hatObj.layer = 27;
                        mapWidthMult = 2000 / space.Map3DScaleInv;
                    } else {
                        var frame = KSPContext.CurrentContext.ActiveVessel.transform?.coordinateSystem;
                        start = frame.ToLocalPosition( startProvider());
                        vector = frame.ToLocalPosition(endProvider()) - start;
                        lineObj.layer = 0;
                        labelObj.layer = 0;
                        hatObj.layer = 0;
                    }
                    
                    Vector3d point1 = mapLengthMult * start;
                    Vector3d point2 = mapLengthMult * (start + (Scale * 0.95 * vector));
                    Vector3d point3 = mapLengthMult * (start + (Scale * vector));

                    label.fontSize = (int)(12.0 * (Width / 0.2) * Scale * mapWidthMult);

                    useWidth = (float)(Width * Scale * mapWidthMult);

                    // Position the arrow line:
                    line.positionCount = 2;
                    line.startWidth = useWidth;
                    line.endWidth = useWidth;
                    line.SetPosition(0, point1);
                    line.SetPosition(1, Pointy ? point2 : point3);

                    // Position the arrow hat.  Note, if Pointy = false, this will be invisible.
                    hat.positionCount = 2;
                    hat.startWidth = useWidth * 3.5f;
                    hat.endWidth = 0.0f;
                    hat.SetPosition(0, point2);
                    hat.SetPosition(1, point3);

                    // Put the label at the midpoint of the arrow:
                    labelLocation = (point1 + point3) / 2;
                    label.transform.position = labelLocation;
                }
            }            

            public void RenderColor() {
                Color c1 = Color.Color;
                Color c2 = Color.Color;
                c1.a = c1.a * (float)0.25;
                Color lCol =
                    UnityEngine.Color.Lerp(c2, UnityEngine.Color.white, 0.7f); // "whiten" the label color a lot.

                if (line != null && hat != null) {
                    // If Wiping, then the line has the fade effect from color c1 to color c2,
                    // else it stays at c2 the whole way:
                    line.startColor = c1;
                    line.endColor = c2;
                    // The hat does not have the fade effect, staying at color c2 the whole way:
                    hat.startColor = c2;
                    hat.endColor = c2;
                    label.color = lCol; // The label does not have the fade effect.
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
