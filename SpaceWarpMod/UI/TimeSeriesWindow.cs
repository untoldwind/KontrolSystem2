using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public class TimeSeriesWindow : ResizableWindow {
        private GLUIDrawer drawer;
        private TimeSeriesCollection timeSeriesCollection;
        private List<KSPTelemetryModule.TimeSeries> selectedTimeSeries = new List<KSPTelemetryModule.TimeSeries>();
        private Vector2 timeSeriesScrollPos;

        private static Color[] colors =
        {
            Color.yellow,
            Color.green,
            Color.red,
            Color.white,
            Color.blue,
        };
        
        public void Awake() {
            Initialize($"TimeSeries", new Rect(200, 50, 600, 400), 400, 300, false);

            drawer = new GLUIDrawer(600, 400);
        }

        public void ConnectTo(TimeSeriesCollection timeSeriesCollection)
        {
            this.timeSeriesCollection = timeSeriesCollection;
            this.selectedTimeSeries.Clear();
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginHorizontal();

            DrawTimeSeriesGraph();

            DrawTimeSeriesSelect();

            GUILayout.EndHorizontal();
        }
        
        private void DrawTimeSeriesSelect() {
            GUILayout.BeginVertical();
            
            timeSeriesScrollPos = GUILayout.BeginScrollView(timeSeriesScrollPos, GUILayout.MinWidth(200));
            GUILayout.BeginVertical();

            if (timeSeriesCollection != null)
            {
                foreach (var timeSeries in timeSeriesCollection.AllTimeSeries)
                {
                    if (GUILayout.Toggle(selectedTimeSeries.Contains(timeSeries), timeSeries.Name)) {
                        if(!selectedTimeSeries.Contains(timeSeries)) selectedTimeSeries.Add(timeSeries);
                    }
                    else {
                        selectedTimeSeries.Remove(timeSeries);
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            GUILayout.EndVertical();
        }

        private void DrawTimeSeriesGraph() {
            GUILayout.BeginVertical();
            var graphRect = GUILayoutUtility.GetRect(200, 200,
                new GUILayoutOption[]
                {
                    GUILayout.MinWidth(200), GUILayout.MaxWidth(3000), GUILayout.ExpandWidth(true),
                    GUILayout.MinHeight(200), GUILayout.MaxHeight(3000), GUILayout.ExpandHeight(true)
                });
            var width = (int)graphRect.width;
            var height = (int)graphRect.height;
            if(width > 1 && height > 1) drawer.Resize(width, height);

            DrawGraph();

            GUI.DrawTexture(graphRect, drawer.Texture);  
            
            GUILayout.EndVertical();
        }
        
        private void DrawGraph() {
            using (var draw = drawer.Draw())
            {
                if (selectedTimeSeries.Count == 0) {
                    var textSize = draw.TextSize("No data", 20, 0);
                    draw.DrawText(new Vector2(draw.Width /2 + (textSize.x - textSize.width) / 2, draw.Height /2 - (textSize.height - textSize.y) / 2 ), "No data", 20, 0, Color.yellow);
                }
                else {
                    var offsetTop = 2;
                    var offsetBottom = 18;
                    var offsetLeft = 18;
                    var offsetRight = 2;
                    
                    var startUT = selectedTimeSeries.Min(t => t.StartUt);
                    var startUTStr = startUT.ToString("F2", CultureInfo.InvariantCulture);
                    var endUT = selectedTimeSeries.Max(t => t.EndUt);
                    var endUTStr = endUT.ToString("F2", CultureInfo.InvariantCulture);;
                    
                    draw.DrawText(new Vector2(offsetLeft, draw.Height - 1), startUTStr, 14, 0, Color.white);
                    var endUTSize = draw.TextSize(endUTStr, 14, 0);
                    draw.DrawText(new Vector2(draw.Width - offsetRight - endUTSize.width, draw.Height - 1), endUTStr, 14, 0, Color.white);

                    var allValues = selectedTimeSeries.Select(t => t.Values).ToArray();
                    var min = allValues.SelectMany(value => value).Min(v => v.Item2.min);
                    var minStr = min.ToString("F3", CultureInfo.InvariantCulture);
                    var max = allValues.SelectMany(value => value).Max(v => v.Item2.max);
                    var maxStr = max.ToString("F3", CultureInfo.InvariantCulture);
                    
                    draw.DrawText(new Vector2(offsetLeft - 4, draw.Height - offsetBottom), minStr, 14, -90, Color.white);
                    var maxSize = draw.TextSize(maxStr, 14, -90);
                    draw.DrawText(new Vector2(offsetLeft - 4, offsetTop + maxSize.height), maxStr, 14, -90, Color.white);

                    var xScale = (endUT - startUT) / (draw.Width - offsetLeft - offsetRight); 
                    var yScale = (max - min) / (draw.Height - offsetBottom - offsetTop);
                    
                    if (min < 0 && max > 0) {
                        draw.Polygon(new Vector2[] {
                            new Vector2(offsetLeft,  draw.Height - offsetBottom - (float)((0 - min) / yScale)), 
                            new Vector2(draw.Width - offsetRight,  draw.Height - offsetBottom - (float)((0 - min) / yScale))
                        }, Color.gray);                        
                    }

                    for (int i = 0; i < allValues.Length; i++) {
                        var color = colors[i % colors.Length];

                        draw.LineTube(allValues[i].Select(i =>
                            new Vector3((float)((i.Item1 - startUT) / xScale) + offsetLeft,
                                draw.Height - offsetBottom - (float)((i.Item2.min - min) / yScale),
                                draw.Height - offsetBottom - (float)((i.Item2.max - min) / yScale))).ToArray(), new Color(color.r, color.g, color.b, 0.5f));

                        draw.Polygon(allValues[i].Select(i =>
                            new Vector2((float)((i.Item1 - startUT) / xScale) + offsetLeft,
                                draw.Height - offsetBottom - (float)((i.Item2.avg - min) / yScale))).ToArray(), color);
                    }
                    
                    draw.Polygon(new Vector2[] {
                        new Vector2(offsetLeft, draw.Height - offsetBottom), 
                        new Vector2(offsetLeft, offsetTop ),
                        new Vector2(draw.Width - offsetRight, offsetTop),
                        new Vector2(draw.Width - offsetRight, draw.Height - offsetBottom)
                    }, Color.white, true);
                }
            }
        }
    }
}
