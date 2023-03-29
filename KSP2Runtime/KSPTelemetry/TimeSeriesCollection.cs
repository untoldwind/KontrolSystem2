using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.KSP.Runtime.KSPTelemetry {
    public class TimeSeriesCollection {
        private static int MAX_NUM_TIMESERIES = 10;
        private static LinkedList<KSPTelemetryModule.TimeSeries> timeSeriesList;
        private readonly object collectionLock = new object();

        public TimeSeriesCollection() {
            timeSeriesList = new LinkedList<KSPTelemetryModule.TimeSeries>();
        }

        public void AddTimeSeries(KSPTelemetryModule.TimeSeries timeSeries) {
            lock (collectionLock) {
                var existing = timeSeriesList.FirstOrDefault(t => t.Name == timeSeries.Name);

                if (existing != null) {
                    timeSeriesList.Remove(existing);
                    timeSeriesList.AddLast(timeSeries);
                } else {
                    if(timeSeriesList.Count >= MAX_NUM_TIMESERIES) timeSeriesList.RemoveFirst();
                    timeSeriesList.AddLast(timeSeries);
                }
            }
        }

        public KSPTelemetryModule.TimeSeries[] AllTimeSeries {
            get {
                lock(collectionLock) {
                    return timeSeriesList.ToArray();
                }
            }
        }

        public KSPTelemetryModule.TimeSeries GetTimeSeries(string name) {
            lock (collectionLock) {
                return timeSeriesList.FirstOrDefault(t => t.Name == name);
            }
        }

        public bool RemoveTimeSeries(string name) {
            lock (collectionLock) {
                var existing = timeSeriesList.FirstOrDefault(t => t.Name == name);

                if (existing != null) {
                    timeSeriesList.Remove(existing);
                    return true;
                }

                return false;
            }
        }

        public void RemoveAll() {
            lock (collectionLock) {
                timeSeriesList.Clear();
            }
        }

    }
}
