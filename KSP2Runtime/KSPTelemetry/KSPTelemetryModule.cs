using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPTelemetry {
    [KSModule("ksp::telemetry")]
    public partial class KSPTelemetryModule {
        [KSFunction]
        public static TimeSeries AddTimeSeries(string name, double startUt, double initialResolution) {
            var timeSeries = new TimeSeries(name, startUt, initialResolution);
            KSPContext.CurrentContext.TimeSeriesCollection.AddTimeSeries(timeSeries);
            return timeSeries;
        }

        [KSFunction]
        public static Option<TimeSeries> GetTimeSeries(string name) {
            var timeSeries = KSPContext.CurrentContext.TimeSeriesCollection.GetTimeSeries(name);
            return timeSeries != null ? Option.Some<TimeSeries>(timeSeries) : Option.None<TimeSeries>();
        }
        
        [KSFunction]
        public static bool RemoveTimeSeries(string name) {
            return KSPContext.CurrentContext.TimeSeriesCollection.RemoveTimeSeries(name);
        }

        [KSFunction]
        public static void RemoveAllTimeSeries() {
            KSPContext.CurrentContext.TimeSeriesCollection.RemoveAll();
        }

        [KSFunction]
        public static void SaveTimeSeries(string filename) {
            KSPContext.CurrentContext.TimeSeriesCollection.SaveJson(filename);
        }
    }
}
