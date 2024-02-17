using System;
using KontrolSystem.KSP.Runtime.KSPTelemetry;
using Xunit;

namespace KontrolSystem.KSP.Runtime.Test;

public class KSPTimeSeriesTests {
    [Fact]
    public void TestFillConstant() {
        var timeSeries = new KSPTelemetryModule.TimeSeries("Test", 0.0, 0.05);

        for (var i = 0; i < 20000; i++) {
            var t = i * 0.5;
            timeSeries.AddData(t, Math.Cos(2 * Math.PI * t / 5000));
        }

        Assert.Equal(0, timeSeries.StartUt);
        Assert.Equal(3.2, timeSeries.Resolution, 6);
        Assert.Equal(10000, timeSeries.EndUt);

        var buckets = timeSeries.Values;

        Assert.Equal(3125, buckets.Length);

        for (var i = 0; i < buckets.Length; i++) {
            var (ut, bucket) = buckets[i];
            Assert.Equal(i * timeSeries.Resolution, ut);

            var refT = (i + 0.5) * timeSeries.Resolution;
            var refValue = Math.Cos(2 * Math.PI * refT / 5000);

            Assert.True(bucket.count > 0);
            Assert.True(bucket.min <= refValue);
            Assert.True(bucket.max >= refValue);
            Assert.True(bucket.avg >= bucket.min);
            Assert.True(bucket.avg <= bucket.max);
            Assert.True(bucket.max - bucket.min < 0.01);
        }
    }

    [Fact]
    public void TestFillErrors() {
        var timeSeries2 = new KSPTelemetryModule.TimeSeries("Errors", 0.0, 0.1);
        timeSeries2.AddData(0, 0.5);
        timeSeries2.AddData(0, 0.7);
        timeSeries2.AddData(500, 0.3);
        timeSeries2.AddData(500, 0.8);
        timeSeries2.AddData(1500, 0.3);
        timeSeries2.AddData(1500, 0.4);
        timeSeries2.AddData(2000, 0.8);
        timeSeries2.AddData(2000, 0.9);
        var buckets = timeSeries2.Values;

        Assert.True(buckets.Length == 4);
        Assert.Equal(0.0, buckets[0].Item1);
        Assert.Equal(0.6, buckets[0].Item2.avg, 6);
        Assert.Equal(0.5, buckets[0].Item2.min, 6);
        Assert.Equal(0.7, buckets[0].Item2.max, 6);
        Assert.Equal(500.0, buckets[1].Item1);
        Assert.Equal(0.55, buckets[1].Item2.avg, 6);
        Assert.Equal(0.3, buckets[1].Item2.min, 6);
        Assert.Equal(0.8, buckets[1].Item2.max, 6);
        Assert.Equal(1500.0, buckets[2].Item1);
        Assert.Equal(0.35, buckets[2].Item2.avg, 6);
        Assert.Equal(0.3, buckets[2].Item2.min, 6);
        Assert.Equal(0.4, buckets[2].Item2.max, 6);
        Assert.Equal(2000.0, buckets[3].Item1);
        Assert.Equal(0.85, buckets[3].Item2.avg, 6);
        Assert.Equal(0.8, buckets[3].Item2.min, 6);
        Assert.Equal(0.9, buckets[3].Item2.max, 6);
    }
}
