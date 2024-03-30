using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    public abstract class AbstractContainer(UGUILayout layout) {
        protected UGUILayout layout = layout;

        internal abstract Window Root { get; }

        [KSMethod(Description = "Add sub container with horizontal layout to the container")]
        public Container AddHorizontal(
            [KSParameter("Gap between each element of the container")]
            double gap = 10,
            [KSParameter("Alignment of the sub container in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUILayoutContainer.Horizontal((float)gap), align, (float)stretch);
            Root.Layout();
            return new Container(Root, element.layout, entry);
        }

        [KSMethod(Description = "Add sub container with vertical layout to the container")]
        public Container AddVertical(
            [KSParameter("Gap between each element of the container")]
            double gap = 10,
            [KSParameter("Alignment of the sub container in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUILayoutContainer.Vertical((float)gap), align, (float)stretch);
            Root.Layout();
            return new Container(Root, element.layout, entry);
        }

        [KSMethod(Description = "Add sub panel with horizontal layout to the container")]
        public Container AddHorizontalPanel(
            [KSParameter("Gap between each element of the panel")]
            double gap = 10,
            [KSParameter("Alignment of the panel in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch, 
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")] 
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUILayoutContainer.HorizontalPanel((float)gap), align, (float)stretch);
            Root.Layout();
            return new Container(Root, element.layout, entry);
        }
        
        [KSMethod(Description = "Add sub panel with vertical layout to the container")]
        public Container AddVerticalPanel(
            [KSParameter("Gap between each element of the panel")]
            double gap = 10,
            [KSParameter("Alignment of the panel in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUILayoutContainer.VerticalPanel((float)gap), align, (float)stretch);
            Root.Layout();
            return new Container(Root, element.layout, entry);
        }

        [KSMethod(Description = "Add vertical scroll view to the container")]
        public Container AddVerticalScroll(
            [KSParameter("Minimum width of the scroll view")] 
            double minWidth,
            [KSParameter("Minimum height of the scroll view")] 
            double minHeight,
            [KSParameter("Gap between each element of the panel")]
            double gap = 10,
            [KSParameter("Alignment of the panel in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch, 
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (scroll, container) = UGUILayoutContainer.VerticalScroll((float)gap);
            var entry = layout.Add(scroll, align, new Vector2((float)minWidth, (float)minHeight), (float)stretch, container.Layout);
            Root.Layout();
            return new Container(Root, container.layout, entry);
        }
        
        [KSMethod(Description = "Add label to the container")]
        public Label AddLabel(string label,
            [KSParameter("Alignment of the label in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUILabel.Create(label), align, (float)stretch);
            Root.Layout();
            return new Label(this, element, entry);
        }

        [KSMethod(Description = "Add button to the container")]
        public Button AddButton(string label,
            [KSParameter("Alignment of the button in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUIButton.Create(label), align, (float)stretch);
            Root.Layout();
            return new Button(Root, element, entry);
        }

        [KSMethod(Description = "Add toggle to the container")]
        public Toggle AddToggle(string label,
            [KSParameter("Alignment of the toggle in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUIToggle.Create(label), align, (float)stretch);
            Root.Layout();
            return new Toggle(Root, element, entry);
        }

        [KSMethod(Description = "Add string input field to the container")]
        public StringInputField AddStringInput(
            [KSParameter("Alignment of the input field in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUIInputField.Create("", 50.0f), align, (float)stretch);
            Root.Layout();
            return new StringInputField(this, element, entry);
        }

        [KSMethod(Description = "Add integer input field to the container")]
        public IntInputField AddIntInput(
            [KSParameter("Alignment of the input field in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUIInputField.Create("0", 50.0f), align, (float)stretch);
            Root.Layout();
            return new IntInputField(this, element, entry);
        }

        [KSMethod(Description = "Add float input field to the container")]
        public FloatInputField AddFloatInput(
            [KSParameter("Alignment of the input field in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUIInputField.Create("0.0", 50.0f), align, (float)stretch);
            Root.Layout();
            return new FloatInputField(this, element, entry);
        }

        [KSMethod(Description = "Add horizontal slider to the container")]
        public Slider AddHorizontalSlider(
            [KSParameter("Minimum value of the slider")]
            double min,
            [KSParameter("Maximum value of the slider")]
            double max,
            [KSParameter("Alignment of the slider in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUISlider.CreateHorizontal(), align, (float)stretch);
            Root.Layout();
            return new Slider(this, element, min, max, entry);
        }

        [KSMethod(Description = "Add canvas to the container")]
        public Canvas AddCanvas(
            [KSParameter("Minimum width of the canvas")]
            double minWidth,
            [KSParameter("Minimum height of the canvas")]
            double minHeight,
            [KSParameter("Alignment of the canvas in its parent container")]
            UGUILayout.Align align = UGUILayout.Align.Stretch,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {
            var (element, entry) = layout.Add(UGUICanvas.Create((float)minWidth, (float)minHeight), align, (float)stretch);
            Root.Layout();
            return new Canvas(this, element, entry);
        }

        [KSMethod(Description = "Add empty space between elements")]
        public void AddSpacer(
            [KSParameter("Minimum amount of space between elements")]
            double size,
            [KSParameter("Relative amount of available space to acquire (beyond minimal space)")]
            double stretch = 0) {

            layout.AddSpace((float)size, (float)stretch);
        }
    }
}
