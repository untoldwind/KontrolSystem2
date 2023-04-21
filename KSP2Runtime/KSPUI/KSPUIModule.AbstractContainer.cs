using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        public abstract class AbstractContainer {
            protected UGUILayout layout;
            protected abstract Window Root { get; }

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
                return new Label(element);
            }

            [KSMethod]
            public Button AddButton(string label, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIButton.Create(label), align, (float)stretch);
                Root.Layout();
                return new Button(element);
            }

            [KSMethod]
            public StringInputField AddStringInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("", 50.0f), align, (float)stretch);
                Root.Layout();
                return new StringInputField(element);
            }
            
            [KSMethod]
            public IntInputField AddIntInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("0", 50.0f), align, (float)stretch);
                Root.Layout();
                return new IntInputField(element);
            }
            
            [KSMethod]
            public FloatInputField AddFloatInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("0.0", 50.0f), align, (float)stretch);
                Root.Layout();
                return new FloatInputField(element);
            }
        }
    }
}
