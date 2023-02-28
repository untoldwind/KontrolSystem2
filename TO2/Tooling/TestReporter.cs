using System.Collections.Generic;

namespace KontrolSystem.TO2.Tooling {
    public interface ITestReporter {
        void BeginModule(string moduleName);

        void EndModule(string moduleName);

        void Report(TestResult testResult);
    }

    public delegate void LineWriter(string message);

    public class ConsoleTestReporter : ITestReporter {
        private readonly LineWriter output;
        private readonly List<TestResult> failures = new List<TestResult>();
        private readonly List<TestResult> errors = new List<TestResult>();

        public ConsoleTestReporter(LineWriter output) => this.output = output;

        public void BeginModule(string moduleName) {
            output($"Module {moduleName}");
            output("");
        }

        public void EndModule(string moduleName) {
            output("");
        }

        public void Report(TestResult testResult) {
            foreach (string message in testResult.messages)
                output(message);
            switch (testResult.state) {
            case TestResultState.Success:
                output(
                    $"  {(testResult.testName + " ").PadRight(60, '.')} Success ({testResult.successfulAssertions} assertions)");
                break;
            case TestResultState.Failure:
                failures.Add(testResult);
                output(
                    $"  {(testResult.testName + " ").PadRight(60, '.')} Failure (after {testResult.successfulAssertions} assertions)");
                break;
            case TestResultState.Error:
                errors.Add(testResult);
                output(
                    $"  {(testResult.testName + " ").PadRight(60, '.')} Error (after {testResult.successfulAssertions} assertions)");
                break;
            }
        }

        public bool WasSuccessful => failures.Count == 0 && errors.Count == 0;

        public List<TestResult> Failures => failures;

        public List<TestResult> Errors => errors;
    }
}
