using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.Benchmarks;

public class LambertBench {
    delegate Vector3d LambertSolve(Vector3d r1, Vector3d r2, double tof, double mu, bool clockwise);

    LambertSolve csharpSolver;
    
    LambertSolve to2Solver;

    public IEnumerable<TestSet> TestSets => TestSetsDefault.testSets.Take(10);
    
    public LambertBench() {
        csharpSolver =
            typeof(LambertIzzoSolver).GetMethod("Solve")
                    .CreateDelegate(typeof(LambertSolve)) as
                LambertSolve; // Also use delegate so that invocation overhead ist the same
        
        var registry = KontrolSystemKSPRegistry.CreateKSP();

        registry.AddDirectory("to2Bench");

        var to2Lambert = registry.modules["lambert"];
        var to2LambertSolve = to2Lambert.FindFunction("solve_lambert").PreferSync;

        ContextHolder.CurrentContext.Value = new EmptyContext(false);
        to2Solver = to2LambertSolve.RuntimeMethod.CreateDelegate(typeof(LambertSolve)) as LambertSolve;
    }

    [Benchmark]
    [ArgumentsSource(nameof(TestSets))]
    public object LamberCSharp(TestSet testSet) {
        return csharpSolver(testSet.R1, testSet.R2, testSet.dt, testSet.mu, testSet.shortway);
    }
    
    [Benchmark]
    [ArgumentsSource(nameof(TestSets))]
    public object LamberTO2(TestSet testSet) {
        return to2Solver(testSet.R1, testSet.R2, testSet.dt, testSet.mu, testSet.shortway);
    }

}
