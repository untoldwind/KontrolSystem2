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
                else
                {
                    var offsetTop = 2;
                    var offsetBottom = 18;
                    var offsetLeft = 18;
                    var offsetRight = 2;
                    
                    draw.Polygon(new Vector2[] {
                        new Vector2(offsetLeft, draw.Height - offsetBottom), 
                        new Vector2(offsetLeft, offsetTop ),
                        new Vector2(draw.Width - offsetRight, offsetTop),
                        new Vector2(draw.Width - offsetRight, draw.Height - offsetBottom)
                    }, Color.white, true);

                    var startUT = selectedTimeSeries.Min(t => t.StartUt).ToString("F2", CultureInfo.InvariantCulture);
                    var endUT = selectedTimeSeries.Max(t => t.EndUt).ToString("F2", CultureInfo.InvariantCulture);;
                    
                    draw.DrawText(new Vector2(offsetLeft, draw.Height - 1), startUT, 14, 0, Color.white);
                    var endUTSize = draw.TextSize(endUT, 14, 0);
                    draw.DrawText(new Vector2(draw.Width - offsetRight - endUTSize.width, draw.Height - 1), endUT, 14, 0, Color.white);
                    draw.DrawText(new Vector2(20, 90), "Hallo äö m/s²", 20, 90, Color.yellow);
                }
            }
        }
    }
}
