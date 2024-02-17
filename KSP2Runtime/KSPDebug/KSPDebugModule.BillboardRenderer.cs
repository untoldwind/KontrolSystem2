using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using KSP.Sim;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KontrolSystem.KSP.Runtime.KSPDebug;

public partial class KSPDebugModule {
    [KSClass("DebugBillboard",
        Description = "Represents a ground marker on a given celestial body."
    )]
    public class BillboardRenderer : IMarker {
        private TextMeshPro? billboard;

        private GameObject? billboardObj;

        private bool enable;

        private readonly Func<Position> positionProvider;
        private readonly Func<string> textProvider;

        public BillboardRenderer(Func<Position> positionProvider, Func<string> textProvider,
            KSPConsoleModule.RgbaColor color, long fontSize) {
            this.positionProvider = positionProvider;
            this.textProvider = textProvider;
            Color = color;
            FontSize = fontSize;
        }

        [KSField(Description = "The color of the billboard text")]
        public KSPConsoleModule.RgbaColor Color { get; set; }

        [KSField] public long FontSize { get; set; }


        [KSField(Description = "Controls if the billboard is currently visible (initially `true`)")]
        public bool Visible {
            get => enable;
            set {
                if (value) {
                    if (billboard == null) {
                        billboardObj = new GameObject("KS2DebugBillboard", typeof(TextMeshPro));
                        billboard = billboardObj.GetComponent<TextMeshPro>();

                        billboard.fontSize = FontSize;
                        billboard.text = TextWrap(textProvider());
                        billboard.horizontalAlignment = HorizontalAlignmentOptions.Center;
                        billboard.verticalAlignment = VerticalAlignmentOptions.Middle;
                        billboard.color = Color.Color;
                        billboard.enableWordWrapping = false;
                        billboard.lineSpacing = 1.0f;
                    }
                } else {
                    if (billboard != null) {
                        Object.Destroy(billboardObj);
                        billboard = null;
                    }
                }

                enable = value;
            }
        }

        public void OnUpdate() {
            if (billboard != null) {
                var position = positionProvider();
                Vector3d positionLocal;
                var mapWidthMult = 1.0;

                if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out var mapCore) && mapCore.IsEnabled) {
                    var space = mapCore.map3D.GetSpaceProvider();

                    positionLocal = space.TranslateSimPositionToMapPosition(position);
                    billboardObj!.layer = 27;
                    mapWidthMult = 1500 / space.Map3DScaleInv;
                } else {
                    var frame = KSPContext.CurrentContext.ActiveVessel?.transform?.celestialFrame;
                    positionLocal = frame?.ToLocalPosition(position) ?? Vector3d.zero;
                    billboardObj!.layer = 0;
                }

                var camera = KSPContext.CurrentContext.Game.SessionManager.GetMyActiveCamera();
                billboard.text = TextWrap(textProvider());
                billboard.fontSize = (float)(FontSize * mapWidthMult);
                billboard.transform.position = positionLocal;
                billboard.transform.rotation = camera.transform.rotation;
            }
        }

        public void OnRender() {
        }

        [KSMethod]
        public void Remove() {
            KSPContext.CurrentContext.RemoveMarker(this);
        }

        private static string TextWrap(string text) {
            return $"<mspace=0.7em><mark=#10101040><align=left>{text}</align></mark></mspace>";
        }
    }
}
