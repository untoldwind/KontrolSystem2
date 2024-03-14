using BenchmarkDotNet.Running;

namespace KontrolSystem.Benchmarks;

public class MainClass {
    public static void Main(string[] args) {
                var summary = BenchmarkRunner.Run<LambertBench>();
//        var summary = BenchmarkRunner.Run<ParserBench>();
    }
}
