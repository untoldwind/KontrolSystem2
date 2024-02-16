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

            [KSMethod(Description = "Add sub container with horizontal layout to the container")]
            public Container AddHorizontal(
                [KSParameter("Gap between each element of the container")] double gap = 10, 
                [KSParameter("Alignment of the sub container in its parent container")] UGUILayout.Align align = UGUILayout.Align.Stretch, 
                [KSParameter("Relative amount of available space to acquire (if allin = Stretch)")] double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.Horizontal((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod(Description = "Add sub container with vertical layout to the container")]
            public Container AddVertical(
                [KSParameter("Gap between each element of the container")] double gap = 10, 
                [KSParameter("Alignment of the sub container in its parent container")] UGUILayout.Align align = UGUILayout.Align.Stretch, 
                [KSParameter("Relative amount of available space to acquire (if allin = Stretch)")] double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.Vertical((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod(Description = "Add sub panel with horizontal layout to the container")]
            public Container AddHorizontalPanel(
                [KSParameter("Gap between each element of the panel")] double gap = 10, 
                [KSParameter("Alignment of the panel in its parent container")] UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.HorizontalPanel((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod(Description = "Add sub panel with vertical layout to the container")]
            public Container AddVerticalPanel(
                [KSParameter("Gap between each element of the panel")] double gap = 10, 
                [KSParameter("Alignment of the panel in its parent container")] UGUILayout.Align align = UGUILayout.Align.Stretch, 
                [KSParameter("Relative amount of available space to acquire (if allin = Stretch)")] double stretch = 0) {
                var element = layout.Add(UGUILayoutContainer.VerticalPanel((float)gap), align, (float)stretch);
                Root.Layout();
                return new Container(Root, element.layout);
            }

            [KSMethod(Description = "Add label to the container")]
            public Label AddLabel(string label, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUILabel.Create(label), align, (float)stretch);
                Root.Layout();
                return new Label(this, element);
            }

            [KSMethod(Description = "Add button to the container")]
            public Button AddButton(string label, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIButton.Create(label), align, (float)stretch);
                Root.Layout();
                return new Button(Root, element);
            }

            [KSMethod(Description = "Add toggle to the container")]
            public Toggle AddToggle(string label, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIToggle.Create(label), align, (float)stretch);
                Root.Layout();
                return new Toggle(Root, element);
            }

            [KSMethod(Description = "Add string input field to the container")]
            public StringInputField AddStringInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("", 50.0f), align, (float)stretch);
                Root.Layout();
                return new StringInputField(this, element);
            }

            [KSMethod(Description = "Add integer input field to the container")]
            public IntInputField AddIntInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("0", 50.0f), align, (float)stretch);
                Root.Layout();
                return new IntInputField(this, element);
            }

            [KSMethod(Description = "Add float input field to the container")]
            public FloatInputField AddFloatInput(UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUIInputField.Create("0.0", 50.0f), align, (float)stretch);
                Root.Layout();
                return new FloatInputField(this, element);
            }

            [KSMethod(Description = "Add horizontal slider to the container")]
            public Slider AddHorizontalSlider(double min, double max, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUISlider.CreateHorizontal(), align, (float)stretch);
                Root.Layout();
                return new Slider(this, element, min, max);
            }

            [KSMethod(Description = "Add canvas to the container")]
            public Canvas AddCanvas(double minWidth, double minHeight, UGUILayout.Align align = UGUILayout.Align.Stretch, double stretch = 0) {
                var element = layout.Add(UGUICanvas.Create((float)minWidth, (float)minHeight), align, (float)stretch);
                Root.Layout();
                return new Canvas(this, element);
            }
        }
    }
}
