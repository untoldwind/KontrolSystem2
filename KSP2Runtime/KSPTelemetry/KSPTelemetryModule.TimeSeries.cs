using System;
using System.Linq;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPTelemetry {
    public partial class KSPTelemetryModule {
        private static int NUM_BUCKETS = 5000;
        
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
            private readonly double start;
            private int lastBucketIdx;
            private double resolution;
            private readonly object seriesLock = new object();

            public TimeSeries(string name, double start, double initialResolution) {
                buckets = new TimeSeriesBucket[NUM_BUCKETS];
                resolution = initialResolution;
                lastBucketIdx = -1;
                Name = name;
                this.start = start;
            }
            
            [KSField(Description = "Name of the time series. Has to be unique.")]
            public string Name { get; set; }

            [KSField(Description = "Start time of the time series.")]
            public double StartUt => start;

            [KSField(Description = "End time of the time series. This will increase when data is added.")]
            public double EndUt {
                get {
                    lock (seriesLock) {
                        return start + (lastBucketIdx + 1) * resolution;
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

            public (double, TimeSeriesBucket)[] Values {
                get {
                    lock (seriesLock) {
                        return buckets.Take(lastBucketIdx + 1).Select((bucket, i) => (i * resolution + start, bucket)).Where(item => item.bucket.count > 0).ToArray();
                    }
                }
            }
            
            [KSMethod(Description = "Add a data `value` at time `ut`.")]
            public bool AddData(double ut, double value) {
                if (Double.IsNaN(value) || Double.IsInfinity(value)) return false;
                lock (seriesLock) {
                    if (ut < start) return false;

                    int bucketIdx = (int)((ut - start) / resolution);

                    if (bucketIdx >= buckets.Length) {
                        bucketIdx = Compress(bucketIdx);
                    }

                    buckets[bucketIdx].AddData(value);
                    lastBucketIdx = bucketIdx;

                    return true;
                }
            }

            private int Compress(int bucketIdx) {
                int factor = bucketIdx / buckets.Length + 1;

                var newLastBucketIdx = lastBucketIdx / factor;
                for (int i = 0; i <= newLastBucketIdx; i++) {
                    var oldIdx = i * factor;
                    var combined = buckets[oldIdx];

                    for (int j = 1; j < factor && oldIdx + j < buckets.Length; j++) {
                        if (combined.count == 0) {
                            combined = buckets[oldIdx + j];
                        } else if (buckets[oldIdx + j].count > 0) {
                            combined.min = Math.Min(combined.min, buckets[oldIdx + j].min);
                            combined.max = Math.Max(combined.max, buckets[oldIdx + j].max);
                            combined.avg = (combined.avg * combined.count + buckets[oldIdx + j].avg * buckets[oldIdx + j].count) /
                                           (combined.count + buckets[oldIdx + j].count);
                            combined.count += buckets[oldIdx + j].count;
                        }    
                    }
                    
                    buckets[i] = combined;
                }

                for (int i = newLastBucketIdx + 1; i <= lastBucketIdx; i++) {
                    buckets[i].count = 0;
                }
                
                lastBucketIdx = newLastBucketIdx;
                resolution *= factor;
                
                return bucketIdx / factor;
            }
        }
    }

}
