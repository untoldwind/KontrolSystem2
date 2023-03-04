using System.Collections.Generic;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        internal struct MovingAverageValue {
            internal readonly double sampleTime;
            internal readonly double value;

            internal MovingAverageValue(double sampleTime, double value) {
                this.sampleTime = sampleTime;
                this.value = value;
            }
        }

        // For the most part this is a rip-off from KOS
        [KSClass("MovingAverage")]
        public class MovingAverage {
            private List<MovingAverageValue> values = new List<MovingAverageValue>();

            [KSField] public double Mean { get; private set; }

            [KSField] public double MeanDiff { get; private set; }

            [KSField] public long ValueCount => values.Count;

            [KSField] public long SampleLimit { get; set; }

            public MovingAverage() {
                Reset();
                SampleLimit = 30;
            }

            [KSMethod]
            public void Reset() {
                Mean = 0;
                MeanDiff = 0;
                if (values == null) values = new List<MovingAverageValue>();
                else values.Clear();
            }

            [KSMethod]
            public double Update(double sampleTime, double value) {
                lock (values) {
                    if (double.IsInfinity(value) || double.IsNaN(value)) return value;

                    values.Add(new MovingAverageValue(sampleTime, value));
                    while (values.Count > SampleLimit) {
                        values.RemoveAt(0);
                    }

                    //if (Values.Count > 5) Mean = Values.OrderBy(e => e).Skip(1).Take(Values.Count - 2).Average();
                    //else Mean = Values.Average();
                    //Mean = Values.Average();
                    double sum = 0;
                    double diffSum = 0;
                    int count = 0;
                    int diffCount = 0;
                    double max = double.MinValue;
                    double min = double.MaxValue;
                    double lastSampleTime = double.MinValue;
                    double lastVal = double.MinValue;
                    for (int i = 0; i < values.Count; i++) {
                        double val = values[i].value;
                        double time = values[i].sampleTime;

                        if (time > lastSampleTime) {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (lastSampleTime != double.MinValue) {
                                diffSum += (val - lastVal) / (time - lastSampleTime);
                                diffCount++;
                            }

                            lastSampleTime = time;
                            lastVal = val;
                        }

                        if (val > max) {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (max != double.MinValue) {
                                sum += max;
                                count++;
                            }

                            max = val;
                        } else if (val < min) {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (min != double.MaxValue) {
                                sum += min;
                                count++;
                            }

                            min = val;
                        } else {
                            sum += val;
                            count++;
                        }
                    }

                    if (count == 0) {
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (max != double.MinValue) {
                            sum += max;
                            count++;
                        }

                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (min != double.MaxValue) {
                            sum += min;
                            count++;
                        }
                    }

                    Mean = sum / count;
                    MeanDiff = diffCount == 0 ? 0 : diffSum / diffCount;
                    return Mean;
                }
            }
        }
    }
}
