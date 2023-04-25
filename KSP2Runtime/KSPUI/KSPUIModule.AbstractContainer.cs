using System;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        public abstract class AbstractContainer {
            protected UGUILayout layout;
            internal abstract Window Root { get; }

            protected AbstractContainer(UGUILayout layout) {
                this.layout = layout;
            }

            [KSMethod]
            public Container AddHorizontal(double gap = 10, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.Horizontal((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod]
            public Container AddVertical(double gap = 10, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.Vertical((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod]
            public Container AddHorizontalPanel(double gap = 10, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.HorizontalPanel((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod]
            public Container AddVerticalPanel(double gap = 10, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.VerticalPanel((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod]
            public Label AddLabel(string label, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUILabel.Create(label), align, (float)stretch);
                Root.Layout();
                return new Label(this, element);
            }

            [KSMethod]
            public Button AddButton(string label, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIButton.Create(label), align, (float)stretch);
                Root.Layout();
                return new Button(Root, element);
            }

            [KSMethod]
            public Toggle AddToggle(string label, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIToggle.Create(label), align, (float)stretch);
                Root.Layout();
                return new Toggle(Root, element);
            }

            [KSMethod]
            public StringInputField AddStringInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("", 50.0f), align, (float)stretch);
                Root.Layout();
                return new StringInputField(this, element);
            }

            [KSMethod]
            public IntInputField AddIntInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("0", 50.0f), align, (float)stretch);
                Root.Layout();
                return new IntInputField(this, element);
            }

            [KSMethod]
            public FloatInputField AddFloatInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("0.0", 50.0f), align, (float)stretch);
                Root.Layout();
                return new FloatInputField(this, element);
            }

            [KSMethod]
            public Slider AddHorizontalSlider(double min, double max, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUISlider.CreateHorizontal(), align, (float)stretch);
                Root.Layout();
                return new Slider(this, element, min, max);
            }

            [KSMethod]
            public Canvas AddCanvas(double minWidth, double minHeight, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUICanvas.Create((float)minWidth, (float)minHeight), align, (float)stretch);
                Root.Layout();
                return new Canvas(this, element);
            }
        }
    }
}
