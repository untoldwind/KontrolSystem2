using System;
using System.Linq;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Tooling;

public static class TestRunner {
    public delegate TestRunnerContext TestContextFactory();

    public static TestRunnerContext DefaultTestContextFactory() {
        return new TestRunnerContext();
    }

    public static TestResult RunTest(IKontrolModule module, IKontrolFunction testFunction,
        TestContextFactory contextFactory) {
        var testContext = contextFactory();
        try {
            testContext.ResetTimeout();
            var testReturn = testFunction.Invoke(testContext);

            switch (testReturn) {
            case bool booleanResult when !booleanResult:
                return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                    testContext.StackCallCount, "Returned false", testContext.Messages);
            case IAnyOption option when !option.Defined:
                return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                    testContext.StackCallCount, "Returned None", testContext.Messages);
            case IAnyResult result when !result.Success:
                return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                    testContext.StackCallCount, $"Returned Err({result.ErrorString})", testContext.Messages);
            case IAnyFuture future:
                ContextHolder.CurrentContext.Value = testContext;
                for (var i = 0; i < 100; i++) {
                    testContext.IncrYield();
                    testContext.ResetTimeout();
                    var result = future.Poll();
                    if (result.IsReady)
                        return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                            testContext.StackCallCount, testContext.Messages);
                }

                return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                    testContext.StackCallCount, "Future did not become ready", testContext.Messages);
            default:
                return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                    testContext.StackCallCount, testContext.Messages);
            }
        } catch (AssertException e) {
            return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                testContext.StackCallCount, e.Message,
                testContext.Messages);
        } catch (Exception e) {
            Console.Error.WriteLine(e);
            return new TestResult(module.Name + "::" + testFunction.Name, testContext.AssertionsCount,
                testContext.StackCallCount, e,
                testContext.Messages);
        } finally {
            ContextHolder.CurrentContext.Value = null;
        }
    }

    public static void RunTests(IKontrolModule module, ITestReporter reporter, TestContextFactory contextFactory) {
        if (!module.TestFunctions.Any()) return;

        reporter.BeginModule(module.Name);
        foreach (var testFunction in module.TestFunctions)
            reporter.Report(RunTest(module, testFunction, contextFactory));

        reporter.EndModule(module.Name);
    }

    public static void RunTests(KontrolRegistry registry, ITestReporter reporter,
        TestContextFactory contextFactory) {
        foreach (var module in registry.modules.Values) RunTests(module, reporter, contextFactory);
    }
}
