using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Events;

namespace KontrolSystem.KSP.Runtime.KSPTelemetry {
    public class TimeSeriesCollection {
        private static int MAX_NUM_TIMESERIES = 20;
        private static LinkedList<KSPTelemetryModule.TimeSeries> timeSeriesList;
        private readonly object collectionLock = new object();
        public UnityEvent changed = new UnityEvent();

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
                    if (timeSeriesList.Count >= MAX_NUM_TIMESERIES) timeSeriesList.RemoveFirst();
                    timeSeriesList.AddLast(timeSeries);
                }
            }
            changed.Invoke();
        }

        public KSPTelemetryModule.TimeSeries[] AllTimeSeries {
            get {
                lock (collectionLock) {
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
            bool removed;
            lock (collectionLock) {
                var existing = timeSeriesList.FirstOrDefault(t => t.Name == name);

                if (existing != null) {
                    timeSeriesList.Remove(existing);
                    removed = true;
                } else {
                    removed = false;
                }
            }
            changed.Invoke();
            return removed;
        }

        public void RemoveAll() {
            lock (collectionLock) {
                timeSeriesList.Clear();
            }
            changed.Invoke();
        }

        public async void SaveJson(string filename) {
            var allTimeSeries = AllTimeSeries;
            var serializer = JsonSerializer.CreateDefault();

            using (StreamWriter outputFile = new StreamWriter(filename, false, Encoding.UTF8, 65536)) {
                using (JsonWriter writer = new JsonTextWriter(outputFile)) {
                    writer.Formatting = Formatting.Indented;

                    await writer.WriteStartArrayAsync();
                    foreach (var timeSeries in allTimeSeries) {
                        await writer.WriteStartObjectAsync();

                        await writer.WritePropertyNameAsync("name");
                        await writer.WriteValueAsync(timeSeries.Name);

                        await writer.WritePropertyNameAsync("startUT");
                        await writer.WriteValueAsync(timeSeries.StartUt);

                        await writer.WritePropertyNameAsync("endUT");
                        await writer.WriteValueAsync(timeSeries.EndUt);

                        await writer.WritePropertyNameAsync("resolution");
                        await writer.WriteValueAsync(timeSeries.Resolution);

                        await writer.WritePropertyNameAsync("values");
                        await writer.WriteStartArrayAsync();

                        foreach (var value in timeSeries.Values) {
                            await writer.WriteStartObjectAsync();

                            await writer.WritePropertyNameAsync("ut");
                            await writer.WriteValueAsync(value.Item1);

                            await writer.WritePropertyNameAsync("count");
                            await writer.WriteValueAsync(value.Item2.count);

                            await writer.WritePropertyNameAsync("min");
                            await writer.WriteValueAsync(value.Item2.min);

                            await writer.WritePropertyNameAsync("avg");
                            await writer.WriteValueAsync(value.Item2.avg);

                            await writer.WritePropertyNameAsync("max");
                            await writer.WriteValueAsync(value.Item2.max);

                            await writer.WriteEndObjectAsync();
                        }
                        await writer.WriteEndArrayAsync();
                        await writer.WriteEndObjectAsync();
                    }
                    await writer.WriteEndArrayAsync();
                }
            }
        }
    }
}
