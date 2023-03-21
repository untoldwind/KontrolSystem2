using System;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;
using KSP.Map;
using KSP.Sim;
using TMPro;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("DebugBillboard",
            Description = "Represents a ground marker on a given celestial body."
        )]
        public class BillboardRenderer : IMarker {
            [KSField(Description = "The color of the billboard text")]
            public KSPConsoleModule.RgbaColor Color { get; set; }
            
            [KSField]
            public long FontSize { get; set; }
            
            private bool enable;

            private Func<Position> positionProvider;
            private Func<string> textProvider;

            private GameObject billboardObj;
            private TextMeshPro billboard;

            public BillboardRenderer(Func<Position> positionProvider, Func<string> textProvider, KSPConsoleModule.RgbaColor color, long fontSize) {
                this.positionProvider = positionProvider;
                this.textProvider = textProvider;
                Color = color;
                FontSize = fontSize;
            }


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
                            GameObject.Destroy(billboardObj);
                            billboard = null;
                        }
                    }
                    
                    enable = value;
                }
            }
            
            [KSMethod]
            public void Remove() => KSPContext.CurrentContext.RemoveMarker(this);

            public void OnUpdate() {
                if (billboard != null) {
                    Position position = positionProvider();
                    Vector3d positionLocal;
                    double mapWidthMult = 1.0;

                    if (KSPContext.CurrentContext.Game.Map.TryGetMapCore(out MapCore mapCore) && mapCore.IsEnabled) {
                        var space = mapCore.map3D.GetSpaceProvider();

                        positionLocal = space.TranslateSimPositionToMapPosition(position);
                        billboardObj.layer = 27;
                        mapWidthMult = 1500 / space.Map3DScaleInv;
                    } else {
                        var frame = KSPContext.CurrentContext.ActiveVessel.transform?.coordinateSystem;
                        positionLocal = frame.ToLocalPosition(position);
                        billboardObj.layer = 0;
                    }
                    Camera camera = KSPContext.CurrentContext.Game.SessionManager.GetMyActiveCamera();
                    billboard.text = TextWrap(textProvider());
                    billboard.fontSize = (float)(FontSize * mapWidthMult);
                    billboard.transform.position = positionLocal;
                    billboard.transform.rotation = camera.transform.rotation;
                }
            }

            public void OnRender() {
            }

            private static string TextWrap(string text) => $"<mspace=0.7em><mark=#10101040><align=left>{text}</align></mark></mspace>";
        }
    }
}
