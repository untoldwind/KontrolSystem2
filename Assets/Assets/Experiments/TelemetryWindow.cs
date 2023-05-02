using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experiments {
    public class TelemetryWindow : UGUIResizableWindow {
        private UIList<TimeSeries, UITimeSeriesElement> timeSeries;
        private TimeSeriesCollection timeSeriesCollection;
        private GLUIDrawer drawer;
        private RawImage graphImage; 
        private Dictionary<string, Color> selectedTimeSeriesNames = new Dictionary<string, Color>();
        private UGUILayoutContainer savePopup;

        private static Color[] colors =
        {
            Color.yellow,
            Color.green,
            Color.red,
            Color.white,
            Color.blue,
        };

        internal int colorCounter;
        
        public void OnEnable() {
            Initialize("KontrolSystem: Telemetry", new Rect(400, 500, 600, 500));

            var root = RootHorizontalLayout();

            var graphPanel = UGUILayoutContainer.VerticalPanel();
            root.Add(graphPanel, UGUILayout.Align.STRETCH, 1);
            var graph = new GameObject("Graph", typeof(RawImage));
            graphPanel.Add(graph, UGUILayout.Align.STRETCH, new Vector2(200, 200), 1);
            drawer = new GLUIDrawer(600, 400);
            graphImage = graph.GetComponent<RawImage>();
            graphImage.texture = drawer.Texture;
            
            timeSeries = new UIList<TimeSeries, UITimeSeriesElement>(30, element => 
                new UITimeSeriesElement(element, () => {
                    var color = NextColor();
                    selectedTimeSeriesNames.Add(element.Name, color);
                    return color;
                }, () => selectedTimeSeriesNames.Remove(element.Name)));

            var timeSeriesContainer = root.Add(UGUILayoutContainer.Vertical());
            
            timeSeriesContainer.Add(UGUIElement.VScrollView(timeSeries, new Vector2(250, 200)), UGUILayout.Align.STRETCH, 1);
            timeSeriesContainer.Add(UGUIButton.Create("Save", ToggleSavePopup), UGUILayout.Align.START);
            
            timeSeriesCollection = ProgrammaticUI.timeSeriesCollection;
            timeSeriesCollection.changed.AddListener(OnTimeSeriesChanged);

            MinSize = root.MinSize;
            root.Layout();
            
            OnTimeSeriesChanged();
            OnResize(Vector2.zero);
        }

        public void OnDisable() {
            base.OnDisable();

            timeSeriesCollection.changed.RemoveListener(OnTimeSeriesChanged);
        }

        public void Update() {
            using (var draw = drawer.Draw()) {
                if (selectedTimeSeriesNames.Count == 0) {
                    draw.DrawText(new Vector2(draw.Width / 2, draw.Height / 2), "No data", 50, new Vector2(0.5f, 0.5f), 0, Color.yellow);
                } else {
                    var selectedTimeSeries =
                        timeSeriesCollection.AllTimeSeries.Where(t => selectedTimeSeriesNames.ContainsKey(t.Name));
                    var offsetTop = 2;
                    var offsetBottom = 18;
                    var offsetLeft = 18;
                    var offsetRight = 2;

                    var startUT = selectedTimeSeries.Min(t => t.StartUt);
                    var startUTStr = startUT.ToString("F2", CultureInfo.InvariantCulture);
                    var endUT = selectedTimeSeries.Max(t => t.EndUt);
                    var endUTStr = endUT.ToString("F2", CultureInfo.InvariantCulture); ;

                    draw.DrawText(new Vector2(offsetLeft, 1), startUTStr, 16, new Vector2(0, 0), 0, Color.white);
                    draw.DrawText(new Vector2(draw.Width - offsetRight, 1), endUTStr, 16, new Vector2(1, 0), 0, Color.white);

                    var allValues = selectedTimeSeries.Select(t => (selectedTimeSeriesNames[t.Name], t.Values)).ToArray();
                    var min = allValues.SelectMany(value => value.Item2).Min(v => v.Item2.min);
                    var minStr = min.ToString("F3", CultureInfo.InvariantCulture);
                    var max = allValues.SelectMany(value => value.Item2).Max(v => v.Item2.max);
                    var maxStr = max.ToString("F3", CultureInfo.InvariantCulture);

                    draw.DrawText(new Vector2(offsetLeft - 2, offsetBottom), minStr, 16, new Vector2(0, 0), 90, Color.white);
                    draw.DrawText(new Vector2(offsetLeft - 2, draw.Height - offsetTop), maxStr, 16, new Vector2(1, 0), 90, Color.white);

                    var xScale = (endUT - startUT) / (draw.Width - offsetLeft - offsetRight);
                    var yScale = (max - min) / (draw.Height - offsetBottom - offsetTop);

                    if (min < 0 && max > 0) {
                        draw.Polyline(new Vector2[] {
                            new Vector2(offsetLeft,  (float)((0 - min) / yScale) + offsetBottom),
                            new Vector2(draw.Width - offsetRight,  (float)((0 - min) / yScale) + offsetBottom)
                        }, Color.gray);
                    }

                    foreach (var (color, values) in allValues) {
                        draw.LineTube(values.Select(i =>
                            new Vector3((float)((i.Item1 - startUT) / xScale) + offsetLeft,
                                (float)((i.Item2.min - min) / yScale) + offsetBottom,
                                (float)((i.Item2.max - min) / yScale) + offsetBottom)).ToArray(), new Color(color.r, color.g, color.b, 0.5f));

                        draw.Polyline(values.Select(i =>
                            new Vector2((float)((i.Item1 - startUT) / xScale) + offsetLeft,
                                (float)((i.Item2.avg - min) / yScale) + offsetBottom)).ToArray(), color);
                    }

                    draw.Polyline(new Vector2[] {
                        new Vector2(offsetLeft, offsetBottom),
                        new Vector2(offsetLeft, draw.Height - offsetTop ),
                        new Vector2(draw.Width - offsetRight, draw.Height - offsetTop),
                        new Vector2(draw.Width - offsetRight, offsetBottom)
                    }, Color.white, true);
                }
            }
        }

        internal void ToggleSavePopup() {
            if (savePopup != null) {
                Destroy(savePopup.GameObject);
                savePopup = null;
            } else {
                savePopup = UGUILayoutContainer.HorizontalPanel(10, new Padding(15, 15, 15, 15));
                UIFactory.Layout(savePopup.GameObject, windowTransform, UIFactory.LAYOUT_STRECH, UIFactory.LAYOUT_END, 10, -50, -40, 60);
                
                var fileNameInput = UGUIInputField.Create("blub", 120);
                savePopup.Add(fileNameInput, UGUILayout.Align.STRETCH, 1);

                savePopup.Add(UGUIButton.Create("Save", () => { }));
                
                savePopup.Layout();
            }
        }

        internal void OnTimeSeriesChanged() {
            timeSeries.Elements = timeSeriesCollection.AllTimeSeries;
        }

        internal Color NextColor() {
            var color = colors[colorCounter++];
            colorCounter = colorCounter < colors.Length ? colorCounter : 0;
            return color;
        }
        
        protected override void OnResize(Vector2 delta) {
            base.OnResize(delta);
            
            var graphTransform = graphImage.GetComponent<RectTransform>();
            var size = Vector2.Scale(graphTransform.rect.size, graphTransform.lossyScale);
            
            drawer.Resize((int)size.x, (int)size.y);
        }

        internal class UITimeSeriesElement : UIListElement<TimeSeries> {
            private readonly TimeSeries timeSeries;
            private readonly UGUILayoutContainer root;
            private readonly UGUIToggle toggle;

            public UITimeSeriesElement(TimeSeries timeSeries, Func<Color> onAdd, Action onRemove) {
                this.timeSeries = timeSeries;

                root = UGUILayoutContainer.Horizontal();

                toggle = root.Add(UGUIToggle.Create(timeSeries.Name, (selected) => {
                    if (selected) {
                        toggle.CheckmarkColor = onAdd();
                    } else {
                        onRemove();
                    }
                }), UGUILayout.Align.STRETCH, 1);
                
                var closeButton = UIFactory.Instance.CreateDeleteButton();
                root.Add(closeButton, UGUILayout.Align.CENTER, new Vector2(24, 24));
                closeButton.GetComponent<Button>().onClick.AddListener(() => ProgrammaticUI.timeSeriesCollection.RemoveTimeSeries(timeSeries.Name));
                
                root.Layout();
            }

            public TimeSeries Element => timeSeries;

            public GameObject Root => root.GameObject;
            
            public void Update(TimeSeries element) {
                toggle.Label = element.Name;
            }
        }
    }
}
