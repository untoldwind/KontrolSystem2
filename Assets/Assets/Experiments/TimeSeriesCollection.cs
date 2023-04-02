using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Experiments {
    public class TimeSeriesCollection {
        private static int MAX_NUM_TIMESERIES = 20;
        private static LinkedList<TimeSeries> timeSeriesList;
        private readonly object collectionLock = new object();
        public UnityEvent changed = new UnityEvent();
        
        public TimeSeriesCollection() {
            timeSeriesList = new LinkedList<TimeSeries>();
        }

        public void AddTimeSeries(TimeSeries timeSeries) {
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

        public TimeSeries[] AllTimeSeries {
            get {
                lock (collectionLock) {
                    return timeSeriesList.ToArray();
                }
            }
        }

        public TimeSeries GetTimeSeries(string name) {
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
        }
    }
}
