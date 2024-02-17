using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPTelemetry;

[KSModule("ksp::telemetry")]
public partial class KSPTelemetryModule {
    [KSFunction(Description = @"
            Create a new time series starting at `startUt` with an initial resultion `initialResolution`.
            If a time series of the `name` already exists it will be replace by the new one.
        ")]
    public static TimeSeries AddTimeSeries(string name, double startUt, double initialResolution) {
        var timeSeries = new TimeSeries(name, startUt, initialResolution);
        KSPContext.CurrentContext.TimeSeriesCollection.AddTimeSeries(timeSeries);
        return timeSeries;
    }

    [KSFunction(Description = "Get a time series by name. Will be undefined if there it does not exists")]
    public static Option<TimeSeries> GetTimeSeries(string name) {
        var timeSeries = KSPContext.CurrentContext.TimeSeriesCollection.GetTimeSeries(name);
        return timeSeries != null ? Option.Some(timeSeries) : Option.None<TimeSeries>();
    }

    [KSFunction(Description = "Remove a time series by name.")]
    public static bool RemoveTimeSeries(string name) {
        return KSPContext.CurrentContext.TimeSeriesCollection.RemoveTimeSeries(name);
    }

    [KSFunction(Description = "Remove all time series.")]
    public static void RemoveAllTimeSeries() {
        KSPContext.CurrentContext.TimeSeriesCollection.RemoveAll();
    }

    [KSFunction(Description = "Store the data of all time series to a file.")]
    public static void SaveTimeSeries(string filename) {
        KSPContext.CurrentContext.TimeSeriesCollection.SaveJson(filename);
    }
}
