using System.Collections.Generic;

namespace KontrolSystem.TO2.Tooling;

public interface ITestReporter {
    void BeginModule(string moduleName);

    void EndModule(string moduleName);

    void Report(TestResult testResult);
}

public delegate void LineWriter(string message);

public class ConsoleTestReporter : ITestReporter {
    private readonly LineWriter output;

    public ConsoleTestReporter(LineWriter output) {
        this.output = output;
    }

    public bool WasSuccessful => Failures.Count == 0 && Errors.Count == 0;

    public List<TestResult> Failures { get; } = new();

    public List<TestResult> Errors { get; } = new();

    public void BeginModule(string moduleName) {
        output($"Module {moduleName}");
        output("");
    }

    public void EndModule(string moduleName) {
        output("");
    }

    public void Report(TestResult testResult) {
        foreach (var message in testResult.messages)
            output(message);
        switch (testResult.state) {
        case TestResultState.Success:
            if (testResult.stackCount != 0) Errors.Add(testResult);
            output(
                $"  {(testResult.testName + " ").PadRight(60, '.')} Success ({testResult.successfulAssertions} assertions)");
            break;
        case TestResultState.Failure:
            Failures.Add(testResult);
            output(
                $"  {(testResult.testName + " ").PadRight(60, '.')} Failure (after {testResult.successfulAssertions} assertions)");
            break;
        case TestResultState.Error:
            Errors.Add(testResult);
            output(
                $"  {(testResult.testName + " ").PadRight(60, '.')} Error (after {testResult.successfulAssertions} assertions)");
            break;
        }
    }
}
