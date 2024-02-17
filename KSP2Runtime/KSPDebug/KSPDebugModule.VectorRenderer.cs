using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public partial class KSPDebugModule {
    [KSClass("DebugVector",
        Description = "Represents a debugging vector in the current scene."
    )]
    public class VectorRenderer : IMarker {
        private bool enable;
        private Func<Position>? endProvider;
        private LineRenderer? hat;
        private GameObject? hatObj;

        private TextMeshPro? label;

        private Vector3 labelLocation;
        private GameObject? labelObj;

        private readonly string labelStr;

        private LineRenderer? line;

        private GameObject? lineObj;

        private Func<Position> startProvider;
        private Func<Vector>? vectorProvider;

        private VectorRenderer(Func<Position> startProvider, Func<Position>? endProvider, Func<Vector>? vectorProvider,
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
            KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) : this(startProvider,
            endProvider, null, color, label, width, pointy) {
        }

        public VectorRenderer(Func<Position> startProvider, Func<Vector> vectorProvider,
            KSPConsoleModule.RgbaColor color, string label, double width, bool pointy) : this(startProvider, null,
            vectorProvider, color, label, width, pointy) {
        }

        [KSField(Description = "The color of the debugging vector")]
        public KSPConsoleModule.RgbaColor Color { get; set; }

        [KSField] public double Scale { get; set; }

        [KSField(Description = "The width of the debugging vector")]
        public double Width { get; set; }

        [KSField(Description = "Controls if an arrow should be drawn at the end.")]
        public bool Pointy { get; set; }

        [KSField(Description = "The current starting position of the debugging vector.")]
        public Position Start {
            get => startProvider();
            set => startProvider = () => value;
        }

        [KSField(Description = "The current end position of the debugging vector.")]
        public Position End {
            get => endProvider?.Invoke() ?? (vectorProvider != null ? startProvider() + vectorProvider() : startProvider());
            set {
                endProvider = () => value;
                vectorProvider = null;
            }
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
                        Object.Destroy(label);
                        label = null;
                    }

                    if (hat != null) {
                        hat.enabled = false;
                        Object.Destroy(hat);
                        hat = null;
                    }

                    if (line != null) {
                        line.enabled = false;
                        Object.Destroy(line);
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

        [KSMethod]
        public void Remove() {
            KSPContext.CurrentContext.RemoveMarker(this);
        }

        [KSMethod(Description = "Change the function providing the start position of the debug vector.")]
        public void SetStartProvider(Func<Position> startProvider) {
            this.startProvider = startProvider;
        }

        [KSMethod(Description = "Change the function providing the end position of the debug vector.")]
        public void SetEndProvider(Func<Position> endProvider) {
            this.endProvider = endProvider;
            vectorProvider = null;
        }

        [KSMethod(Description = "Change the function providing the vector/direction of the debug vector.")]
        public void SetVectorProvider(Func<Vector> vectorProvider) {
            endProvider = null;
            this.vectorProvider = vectorProvider;
        }

        private void RenderPointCoords() {
            if (line != null && hat != null) {
                var mapLengthMult = 1.0; // for scaling when on map view.
                var mapWidthMult = 1.0; // for scaling when on map view.
                float useWidth;
                var start = startProvider();
                var end = endProvider?.Invoke() ?? (vectorProvider != null ? start + vectorProvider() : start);
                Vector3d startLocal;
                Vector3d vectorLocal;

                if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore) && mapCore.IsEnabled) {
                    var space = mapCore.map3D.GetSpaceProvider();

                    startLocal = space.TranslateSimPositionToMapPosition(start);
                    vectorLocal = space.TranslateSimPositionToMapPosition(end) - startLocal;
                    lineObj!.layer = 27;
                    labelObj!.layer = 27;
                    hatObj!.layer = 27;
                    mapWidthMult = 1500 / space.Map3DScaleInv;
                } else {
                    var frame = KSPContext.CurrentContext.ActiveVessel?.transform?.celestialFrame;
                    startLocal = frame?.ToLocalPosition(start) ?? Vector3d.zero;
                    vectorLocal = (frame?.ToLocalPosition(end) ?? startLocal) - startLocal;
                    lineObj!.layer = 0;
                    labelObj!.layer = 0;
                    hatObj!.layer = 0;
                }

                var camera = KSPContext.CurrentContext.Game.SessionManager.GetMyActiveCamera();

                var point1 = mapLengthMult * startLocal;
                var point2 = mapLengthMult * (startLocal + Scale * 0.95 * vectorLocal);
                var point3 = mapLengthMult * (startLocal + Scale * vectorLocal);

                label!.fontSize = (float)(12.0 * Scale * mapWidthMult);

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
            var c1 = Color.Color;
            var c2 = Color.Color;
            c1.a = c1.a * (float)0.25;
            var lCol =
                UnityEngine.Color.Lerp(c2, UnityEngine.Color.white, 0.7f); // "whiten" the label color a lot.

            if (line != null && hat != null) {
                // If Wiping, then the line has the fade effect from color c1 to color c2,
                // else it stays at c2 the whole way:
                line.startColor = c1;
                line.endColor = c2;
                // The hat does not have the fade effect, staying at color c2 the whole way:
                hat.startColor = c2;
                hat.endColor = c2;
                label!.color = lCol; // The label does not have the fade effect.
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
