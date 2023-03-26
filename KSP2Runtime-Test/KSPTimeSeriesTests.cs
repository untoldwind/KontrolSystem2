using System;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using Xunit;

namespace KontrolSystem.KSP.Runtime.Test {
    public class KSPTimeSeriesTests {
        [Fact]
        public void TestFillConstant() {
            var timeSeries = new KSPTelemetryModule.TimeSeries("Test", 0.0, 0.05);

            for (int i = 0; i < 20000; i++) {
                var t = i * 0.5;
                timeSeries.AddData(t, Math.Cos(2 * Math.PI * t / 5000));
            }
            
            Assert.Equal(0, timeSeries.StartUt);
            Assert.Equal(3.2, timeSeries.Resolution, 6);
            Assert.Equal(10000, timeSeries.EndUt);

            var buckets = timeSeries.Values;
            
            Assert.Equal(3125, buckets.Length);

            for (int i = 0; i < buckets.Length; i++) {
                var (ut, bucket) = buckets[i];
                Assert.Equal(i * timeSeries.Resolution, ut);
                
                var refT = (i + 0.5) * timeSeries.Resolution;
                var refValue = Math.Cos(2 * Math.PI * refT / 5000);
                
                Assert.True(bucket.count > 0);
                Assert.True(bucket.min <= refValue);
                Assert.True(bucket.max >= refValue);
                Assert.True(bucket.mean >= bucket.min);
                Assert.True(bucket.mean <= bucket.max);
                Assert.True(bucket.max - bucket.min < 0.01);
            }
        }
    }
}
