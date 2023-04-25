using UnityEngine;

namespace Experiments {
    public class LayoutTestWindow : UGUIResizableWindow {
        public void OnEnable() {
            Initialize("Test", new Rect(100, 700, 550, 400));

            var root = RootHorizontalLayout();

            var panel1 = UGUILayoutContainer.HorizontalPanel();
            root.Add(panel1);
            
            var button2 = UGUIButton.Create("Button 2", () => { });
            root.Add(button2, UGUILayout.Align.STRETCH, 1f);

            var panel1a =  UGUILayoutContainer.HorizontalPanel();
            root.Add(panel1a, UGUILayout.Align.STRETCH);

            var vertical = UGUILayoutContainer.Vertical();
            root.Add(vertical, UGUILayout.Align.STRETCH, 2);

            var slider3 = UGUISlider.CreateHorizontal();
            vertical.Add(slider3);

            var panel4 = UGUILayoutContainer.HorizontalPanel();
            vertical.Add(panel4, UGUILayout.Align.STRETCH, 2f);

            var panel5 = UGUILayoutContainer.HorizontalPanel();
            panel4.Add(panel5);

            var canvas6 = UGUICanvas.Create(200, 200);
            panel4.Add(canvas6, UGUILayout.Align.STRETCH, 1);

            var pts = new[] {
                new Vector2(200, 200),
                new Vector2(250, 500),
                new Vector2(400, 400),
                new Vector2(600, 300),
                new Vector2(300, 350),
            };
            
            Polygon2D poly2D = new Polygon2D(Color.green, pts);
            Line2D line2D = new Line2D(Color.red, 5, true, pts);
            Rotate2D r = new Rotate2D(0, new Vector2(400, 400));

            r.Add(poly2D);
            r.Add(line2D);
            canvas6.Add(r);
            
            slider3.OnChange(value => {
                r.Degrees = 360 * value;
            });
            
            MinSize = root.MinSize;
            root.Layout();
        }
    }
}
