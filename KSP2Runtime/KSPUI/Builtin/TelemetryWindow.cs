using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using KontrolSystem.KSP.Runtime.KSPUI.UGUI;
using KontrolSystem.TO2.AST;
using UnityEngine;
using UnityEngine.UI;

namespace KontrolSystem.KSP.Runtime.KSPUI.Builtin;

public class TelemetryWindow : UGUIResizableWindow {
    private static readonly Color[] colors = {
        Color.yellow,
        Color.green,
        Color.red,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.white
    };

    internal int colorCounter;
    private GLUIDrawer? drawer;
    private RawImage? graphImage;
    private UGUILayoutContainer? savePopup;
    private readonly Dictionary<string, Color> selectedTimeSeriesNames = new();
    private UIList<KSPTelemetryModule.TimeSeries, UITimeSeriesElement>? timeSeries;
    private TimeSeriesCollection? timeSeriesCollection;

    public void Update() {
        using (var draw = drawer!.Draw()) {
            var selectedTimeSeries =
                timeSeriesCollection!.AllTimeSeries.Where(t => selectedTimeSeriesNames.ContainsKey(t.Name) && t.HasData).ToArray();
            if (selectedTimeSeries.Length == 0) {
                draw.DrawText(new Vector2(draw.Width / 2, draw.Height / 2), "No data", 50, new Vector2(0.5f, 0.5f), 0,
                    Color.yellow);
            } else {
                var offsetTop = 2;
                var offsetBottom = 20;
                var offsetLeft = 20;
                var offsetRight = 2;

                var startUT = selectedTimeSeries.Min(t => t.StartUt);
                var startUTStr = startUT.ToString("F2", CultureInfo.InvariantCulture);
                var endUT = selectedTimeSeries.Max(t => t.EndUt);
                var endUTStr = endUT.ToString("F2", CultureInfo.InvariantCulture);

                draw.DrawText(new Vector2(offsetLeft, 1), startUTStr, 18, new Vector2(0, 0), 0, Color.white);
                draw.DrawText(new Vector2(draw.Width - offsetRight, 1), endUTStr, 18, new Vector2(1, 0), 0,
                    Color.white);

                var allValues = selectedTimeSeries.Select(t => (selectedTimeSeriesNames[t.Name], t.Values)).ToArray();
                var min = allValues.SelectMany(value => value.Item2).Min(v => v.Item2.min);
                var minStr = min.ToString("F3", CultureInfo.InvariantCulture);
                var max = allValues.SelectMany(value => value.Item2).Max(v => v.Item2.max);
                var maxStr = max.ToString("F3", CultureInfo.InvariantCulture);

                draw.DrawText(new Vector2(offsetLeft - 2, offsetBottom), minStr, 18, new Vector2(0, 0), 90,
                    Color.white);
                draw.DrawText(new Vector2(offsetLeft - 2, draw.Height - offsetTop), maxStr, 18, new Vector2(1, 0), 90,
                    Color.white);

                var xScale = (endUT - startUT) / (draw.Width - offsetLeft - offsetRight);
                var yScale = (max - min) / (draw.Height - offsetBottom - offsetTop);

                if (min < 0 && max > 0)
                    draw.Polyline(new Vector2[] {
                        new(offsetLeft, (float)((0 - min) / yScale) + offsetBottom),
                        new(draw.Width - offsetRight, (float)((0 - min) / yScale) + offsetBottom)
                    }, Color.gray);

                foreach (var (color, values) in allValues) {
                    draw.LineTube(values.Select(i =>
                            new Vector3((float)((i.Item1 - startUT) / xScale) + offsetLeft,
                                (float)((i.Item2.min - min) / yScale) + offsetBottom,
                                (float)((i.Item2.max - min) / yScale) + offsetBottom)).ToArray(),
                        new Color(color.r, color.g, color.b, 0.5f));

                    draw.Polyline(values.Select(i =>
                        new Vector2((float)((i.Item1 - startUT) / xScale) + offsetLeft,
                            (float)((i.Item2.avg - min) / yScale) + offsetBottom)).ToArray(), color);
                }

                draw.Polyline(new Vector2[] {
                    new(offsetLeft, offsetBottom),
                    new(offsetLeft, draw.Height - offsetTop),
                    new(draw.Width - offsetRight, draw.Height - offsetTop),
                    new(draw.Width - offsetRight, offsetBottom)
                }, Color.white, true);
            }
        }
    }

    public void OnEnable() {
        Initialize("KontrolSystem: Telemetry", new Rect(400, Screen.height - 200, 600, 500));

        var root = RootHorizontalLayout();

        var graphPanel = UGUILayoutContainer.VerticalPanel();
        root.Add(graphPanel, UGUILayout.Align.Stretch, 1);
        var graph = new GameObject("Graph", typeof(RawImage));
        graphPanel.Add(graph, UGUILayout.Align.Stretch, new Vector2(200, 200), 1);
        drawer = new GLUIDrawer(600, 400);
        graphImage = graph.GetComponent<RawImage>();
        graphImage.texture = drawer.Texture;

        var timeSeriesContainer = root.Add(UGUILayoutContainer.Vertical()).Item1;

        timeSeries = new UIList<KSPTelemetryModule.TimeSeries, UITimeSeriesElement>(UIFactory.Instance!.uiFontSize + 10,
            element =>
                new UITimeSeriesElement(element, selectedTimeSeriesNames, NextColor));

        timeSeriesContainer.Add(UGUIElement.VScrollView(timeSeries, new Vector2(250, 200)), UGUILayout.Align.Stretch,
            1);
        timeSeriesContainer.Add(UGUIButton.Create("Save", ToggleSavePopup), UGUILayout.Align.Start);

        timeSeriesCollection = Mainframe.Instance!.TimeSeriesCollection;
        timeSeriesCollection.changed.AddListener(OnTimeSeriesChanged);

        MinSize = root.MinSize;
        root.Layout();

        OnTimeSeriesChanged();
        OnResize(Vector2.zero);
    }

    public override void OnDisable() {
        base.OnDisable();

        timeSeriesCollection?.changed.RemoveListener(OnTimeSeriesChanged);
        drawer?.Dispose();
    }

    internal void ToggleSavePopup() {
        if (savePopup != null) {
            savePopup.Destroy();
            savePopup = null;
        } else {
            savePopup = UGUILayoutContainer.HorizontalPanel(10, new UGUILayout.Padding(15, 15, 15, 15));
            UIFactory.Layout(savePopup.GameObject, windowTransform!, UIFactory.LAYOUT_STRETCH, UIFactory.LAYOUT_END,
                10, -50, -40, 60);

            var fileNameInput = UGUIInputField.Create(
                Path.Combine(Mainframe.Instance!.LocalLibPath,
                    $"TimeSeries-{(long)Game.SpaceSimulation.UniverseModel.UniverseTime}.json"), 120);
            savePopup.Add(fileNameInput, UGUILayout.Align.Stretch, 1);

            savePopup.Add(UGUIButton.Create("Save", () => {
                timeSeriesCollection?.SaveJson(fileNameInput.Value);
                savePopup.Destroy();
                savePopup = null;
            }));

            savePopup.Layout();
        }
    }

    internal void OnTimeSeriesChanged() {
        timeSeries!.Elements = timeSeriesCollection!.AllTimeSeries;
    }

    internal Color NextColor() {
        var color = colors[colorCounter++];
        colorCounter = colorCounter < colors.Length ? colorCounter : 0;
        return color;
    }

    protected override void OnResize(Vector2 delta) {
        base.OnResize(delta);

        var graphTransform = graphImage!.GetComponent<RectTransform>();
        var size = Vector2.Scale(graphTransform.rect.size, graphTransform.lossyScale);

        drawer?.Resize((int)size.x, (int)size.y);
    }

    internal class UITimeSeriesElement : UIListElement<KSPTelemetryModule.TimeSeries> {
        private readonly UGUILayoutContainer root;
        private readonly Dictionary<string, Color> selectedTimeSeriesNames;
        private readonly UGUIToggle toggle;

        public UITimeSeriesElement(KSPTelemetryModule.TimeSeries timeSeries,
            Dictionary<string, Color> selectedTimeSeriesNames, Func<Color> nextColor) {
            Element = timeSeries;
            this.selectedTimeSeriesNames = selectedTimeSeriesNames;

            root = UGUILayoutContainer.Horizontal();
            toggle = root.Add(UGUIToggle.Create(timeSeries.Name, selected => {
                if (selected) {
                    var color = nextColor();
                    this.selectedTimeSeriesNames.Add(timeSeries.Name, color);
                    toggle!.CheckmarkColor = color;
                } else {
                    this.selectedTimeSeriesNames.Remove(timeSeries.Name);
                }
            }), UGUILayout.Align.Stretch, 1).Item1;
            toggle.IsOn = this.selectedTimeSeriesNames.ContainsKey(timeSeries.Name);

            var closeButton = UIFactory.Instance!.CreateDeleteButton();
            root.Add(closeButton, UGUILayout.Align.Center,
                new Vector2(UIFactory.Instance.uiFontSize + 4, UIFactory.Instance.uiFontSize + 4));
            closeButton.GetComponent<Button>().onClick.AddListener(() => {
                selectedTimeSeriesNames.Remove(timeSeries.Name);
                Mainframe.Instance!.TimeSeriesCollection.RemoveTimeSeries(timeSeries.Name);
            });

            root.Layout();
        }

        public KSPTelemetryModule.TimeSeries Element { get; }

        public GameObject Root => root.GameObject;

        public void Update(KSPTelemetryModule.TimeSeries element) {
            toggle.Label = element.Name;
            toggle.IsOn = selectedTimeSeriesNames.ContainsKey(element.Name);
            toggle.CheckmarkColor = selectedTimeSeriesNames.Get(element.Name);
        }
    }
}
