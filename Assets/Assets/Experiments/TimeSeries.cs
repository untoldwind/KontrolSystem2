using System;
using System.Linq;

namespace Experiments {
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

        public class TimeSeries {
            private static int NUM_BUCKETS = 5000;

            private readonly TimeSeriesBucket[] buckets;
            private readonly string name;
            private readonly double start;
            private int lastBucketIdx;
            private double resolution;
            private readonly object seriesLock = new object();

            public TimeSeries(string name, double start, double initialResolution) {
                buckets = new TimeSeriesBucket[NUM_BUCKETS];
                resolution = initialResolution;
                lastBucketIdx = -1;
                this.name = name;
                this.start = start;
            }

            public string Name => name;

            public double StartUt => start;

            public double EndUt {
                get {
                    lock (seriesLock) {
                        return start + (lastBucketIdx + 1) * resolution;
                    }
                }
            }

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
