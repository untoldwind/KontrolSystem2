using System;
using System.Linq;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPTelemetry;

public partial class KSPTelemetryModule {
    private static readonly int NUM_BUCKETS = 5000;

    public struct TimeSeriesBucket {
        public int count;
        public double min;
        public double avg;
        public double max;

        internal void AddData(double value) {
            if (count == 0) {
                count = 1;
                min = avg = max = value;
            } else {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
                avg = (avg * count + value) / (count + 1);
                count++;
            }
        }
    }

    [KSClass("TimeSeries")]
    public class TimeSeries {
        private readonly TimeSeriesBucket[] buckets;
        private readonly object seriesLock = new();
        private int lastBucketIdx;
        private double resolution;

        public TimeSeries(string name, double start, double initialResolution) {
            buckets = new TimeSeriesBucket[NUM_BUCKETS];
            resolution = initialResolution;
            lastBucketIdx = -1;
            this.Name = name;
            this.StartUt = start;
        }

        [KSField(Description = "Name of the time series. Has to be unique.")]
        public string Name { get; }

        [KSField(Description = "Start time of the time series.")]
        public double StartUt { get; }

        [KSField(Description = "End time of the time series. This will increase when data is added.")]
        public double EndUt {
            get {
                lock (seriesLock) {
                    return StartUt + (lastBucketIdx + 1) * resolution;
                }
            }
        }

        [KSField(Description = @"
                Current time resolution of the time series.
                This will increase the longer `end_ut - start_ut` gets to prevent accumulation of too much data.
            ")]
        public double Resolution {
            get {
                lock (seriesLock) {
                    return resolution;
                }
            }
        }

        public bool HasData => lastBucketIdx >= 0;
        
        public (double, TimeSeriesBucket)[] Values {
            get {
                lock (seriesLock) {
                    return buckets.Take(lastBucketIdx + 1).Select((bucket, i) => (i * resolution + StartUt, bucket))
                        .Where(item => item.bucket.count > 0).ToArray();
                }
            }
        }

        [KSMethod(Description = "Add a data `value` at time `ut`.")]
        public bool AddData(double ut, double value) {
            if (double.IsNaN(value) || double.IsInfinity(value)) return false;
            lock (seriesLock) {
                if (ut < StartUt) return false;

                var bucketIdx = (int)((ut - StartUt) / resolution);

                if (bucketIdx >= buckets.Length) bucketIdx = Compress(bucketIdx);

                buckets[bucketIdx].AddData(value);
                lastBucketIdx = bucketIdx;

                return true;
            }
        }

        private int Compress(int bucketIdx) {
            var factor = bucketIdx / buckets.Length + 1;

            var newLastBucketIdx = lastBucketIdx / factor;
            for (var i = 0; i <= newLastBucketIdx; i++) {
                var oldIdx = i * factor;
                var combined = buckets[oldIdx];

                for (var j = 1; j < factor && oldIdx + j < buckets.Length; j++)
                    if (combined.count == 0) {
                        combined = buckets[oldIdx + j];
                    } else if (buckets[oldIdx + j].count > 0) {
                        combined.min = Math.Min(combined.min, buckets[oldIdx + j].min);
                        combined.max = Math.Max(combined.max, buckets[oldIdx + j].max);
                        combined.avg = (combined.avg * combined.count +
                                        buckets[oldIdx + j].avg * buckets[oldIdx + j].count) /
                                       (combined.count + buckets[oldIdx + j].count);
                        combined.count += buckets[oldIdx + j].count;
                    }

                buckets[i] = combined;
            }

            for (var i = newLastBucketIdx + 1; i <= lastBucketIdx; i++) buckets[i].count = 0;

            lastBucketIdx = newLastBucketIdx;
            resolution *= factor;

            return bucketIdx / factor;
        }
    }
}
