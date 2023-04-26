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

            var values = new double[100 * 100];
            for (int y = 0; y < 100; y++) {
                for (int x = 0; x < 100; x++) {
                    values[y * 100 + x] = 0;
                    var cre = 3.0 * x / 100.0 - 2;
                    var cim = 2.0 * y / 100.0 - 1;
                    var re = cre;
                    var im = cim;
                    var rere = re * re;
                    var imim = im * im;
                    var reim = re * im;
                    for (int iter = 0; iter < 100; iter++) {
                        if (rere * imim > 10.0) {
                            values[y * 100 + x] = (iter % 20) + 1;
                        }
                        re = rere - imim + cre;
                        im = reim + reim + cim;
                        rere = re * re;
                        imim = im * im;
                        reim = re * im;
                    }
//                    values[y * 100 + x] = x / 100.0;
                }
            }

            var gradient = new GradientWrapper(new RgbaColor(0, 0, 0, 1), new RgbaColor(1, 0, 0, 1));
            gradient.AddColor(0.01, new RgbaColor(0, 0, 1, 1));
            gradient.AddColor(0.33, new RgbaColor(0, 1, 1, 1));
            gradient.AddColor(0.66, new RgbaColor(1, 1, 0, 1));

            Polygon2D poly2D = new Polygon2D(Color.green, pts);
            Line2D line2D = new Line2D(Color.red, 5, true, pts);
            ValueRaster2D raster2D =
                new ValueRaster2D(values, 100, 100, gradient, new Vector2(50, 50), new Vector2(200, 200));
            Rotate2D r = new Rotate2D(0, new Vector2(400, 400));

            r.Add(poly2D);
            r.Add(line2D);
            r.Add(raster2D);
            canvas6.Add(r);
            
            slider3.OnChange(value => {
                r.Degrees = 360 * value;
            });
            
            MinSize = root.MinSize;
            root.Layout();
        }
    }
}
