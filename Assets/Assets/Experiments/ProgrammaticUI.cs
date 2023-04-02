using System;
using Experiments;
using UnityEngine;

public class ProgrammaticUI : MonoBehaviour {
    private Canvas _canvas;

    internal static TimeSeriesCollection timeSeriesCollection = new TimeSeriesCollection();
    
    // Start is called before the first frame update
    void Start() {
        UIFactory.Init(new GFXAdapter());


        gameObject.AddComponent<ModuleManagerWindow>();
        gameObject.AddComponent<ConsoleWindow>();
        gameObject.AddComponent<TelemetryWindow>();
        gameObject.AddComponent<EditorWindow>();
        gameObject.AddComponent<LayoutTestWindow>();

        var timeSeries1 = new TimeSeries("Cos", 0, 0.1f);
        
        timeSeriesCollection.AddTimeSeries(timeSeries1);

        var timeSeries2 = new TimeSeries("Sin", 0, 0.1f);
        
        timeSeriesCollection.AddTimeSeries(timeSeries2);

        for (int i = 0; i < 2000; i++) {
            timeSeries1.AddData(0.5 * i, Math.Cos(i * Math.PI / 400.0));
            timeSeries2.AddData(0.5 * i, Math.Sin(i * Math.PI / 300.0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
