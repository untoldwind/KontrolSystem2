using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using KSP.Game;
using KSP.Map;
using KSP.Sim;
using TMPro;
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
            private Func<Vector> vectorProvider;

            private LineRenderer line;
            private LineRenderer hat;

            private GameObject lineObj;
            private GameObject hatObj;
            private GameObject labelObj;

            private TextMeshPro label;

            private string labelStr;

            private Vector3 labelLocation;

            private VectorRenderer(Func<Position> startProvider, Func<Position> endProvider, Func<Vector> vectorProvider,
                KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) {
                this.startProvider = startProvider;
                this.endProvider = endProvider;
                this.vectorProvider = vectorProvider;
                Color = color;
                Scale = 1.0;
                Width = width;
                Pointy = pointy;
                labelStr = label;
            }

            public VectorRenderer(Func<Position> startProvider, Func<Position> endProvider,
                KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) : this(startProvider, endProvider, null, color, label, width, pointy) {
            }

            public VectorRenderer(Func<Position> startProvider, Func<Vector> vectorProvider,
                KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) : this(startProvider, null, vectorProvider, color, label, width, pointy) {
            }

            [KSField(Description = "Controls if the debug-vector is currently visible (initially `true`)")]
            public bool Visible {
                get => enable;
                set {
                    if (value) {
                        if (line == null || hat == null) {
                            lineObj = new GameObject("KS2DebugVectorLine", typeof(LineRenderer));
                            hatObj = new GameObject("KS2DebugVectorHat", typeof(LineRenderer));

                            line = lineObj.GetComponent<LineRenderer>();
                            hat = hatObj.GetComponent<LineRenderer>();

                            labelObj = new GameObject("KS2DebugVectorLabel", typeof(TextMeshPro));
                            label = labelObj.GetComponent<TextMeshPro>();

                            line.useWorldSpace = false;
                            hat.useWorldSpace = false;

                            line.material = GLUtils.Colored;
                            hat.material = GLUtils.Colored;

                            label.text = labelStr;
                            label.horizontalAlignment = HorizontalAlignmentOptions.Center;
                            label.verticalAlignment = VerticalAlignmentOptions.Middle;
                            label.color = Color.Color;
                            label.enableWordWrapping = false;
                            label.lineSpacing = 1.0f;

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

            [KSMethod]
            public void Remove() => KSPContext.CurrentContext.RemoveMarker(this);

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
                get => endProvider?.Invoke() ?? startProvider() + vectorProvider();
                set {
                    endProvider = () => value;
                    vectorProvider = null;
                }
            }

            [KSMethod(Description = "Change the function providing the start position of the debug vector.")]
            public void SetStartProvider(Func<Position> startProvider) {
                this.startProvider = startProvider;
            }

            [KSMethod(Description = "Change the function providing the end position of the debug vector.")]
            public void SetEndProvider(Func<Position> endProvider) {
                this.endProvider = endProvider;
                this.vectorProvider = null;
            }

            [KSMethod(Description = "Change the function providing the vector/direction of the debug vector.")]
            public void SetVectorProvider(Func<Vector> vectorProvider) {
                this.endProvider = null;
                this.vectorProvider = vectorProvider;
            }

            private void RenderPointCoords() {
                if (line != null && hat != null) {
                    double mapLengthMult = 1.0; // for scaling when on map view.
                    double mapWidthMult = 1.0; // for scaling when on map view.
                    float useWidth;
                    Position start = startProvider();
                    Position end = endProvider?.Invoke() ?? start + vectorProvider();
                    Vector3d startLocal;
                    Vector3d vectorLocal;

                    if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out MapCore mapCore) && mapCore.IsEnabled) {
                        var space = mapCore.map3D.GetSpaceProvider();

                        startLocal = space.TranslateSimPositionToMapPosition(start);
                        vectorLocal = space.TranslateSimPositionToMapPosition(end) - startLocal;
                        lineObj.layer = 27;
                        labelObj.layer = 27;
                        hatObj.layer = 27;
                        mapWidthMult = 1500 / space.Map3DScaleInv;
                    } else {
                        var frame = KSPContext.CurrentContext.ActiveVessel.transform?.coordinateSystem;
                        startLocal = frame.ToLocalPosition(start);
                        vectorLocal = frame.ToLocalPosition(end) - startLocal;
                        lineObj.layer = 0;
                        labelObj.layer = 0;
                        hatObj.layer = 0;
                    }
                    Camera camera = KSPContext.CurrentContext.Game.SessionManager.GetMyActiveCamera();

                    Vector3d point1 = mapLengthMult * startLocal;
                    Vector3d point2 = mapLengthMult * (startLocal + (Scale * 0.95 * vectorLocal));
                    Vector3d point3 = mapLengthMult * (startLocal + (Scale * vectorLocal));

                    label.fontSize = (float)(12.0 * Scale * mapWidthMult);

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
                    label.transform.rotation = camera.transform.rotation;
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
