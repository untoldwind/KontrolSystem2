using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Tooling;

namespace KontrolSystem.KSP.Runtime.Core;

public class ConsoleBufferTestReporter(KSPConsoleBuffer consoleBuffer) : ITestReporter {
    public void BeginModule(string moduleName) {
        consoleBuffer.PrintLine($"Module {moduleName}");
    }

    public void EndModule(string moduleName) {
        consoleBuffer.PrintLine("");
    }

    public void Report(TestResult testResult) {
        foreach (var message in testResult.messages)
            consoleBuffer.PrintLine(message);
        switch (testResult.state) {
        case TestResultState.Success:
            consoleBuffer.PrintLine(
                $"  {(testResult.testName + " ").PadRight(60, '.')} Success ({testResult.successfulAssertions} assertions)");
            break;
        case TestResultState.Failure:
            consoleBuffer.PrintLine(
                $"  {(testResult.testName + " ").PadRight(60, '.')} Failure (after {testResult.successfulAssertions} assertions)");
            consoleBuffer.PrintLine($"     {testResult.failure}");
            break;
        case TestResultState.Error:
            consoleBuffer.PrintLine(
                $"  {(testResult.testName + " ").PadRight(60, '.')} Error (after {testResult.successfulAssertions} assertions)");
            consoleBuffer.PrintLine($"     {testResult.exception}");
            break;
        }
    }
}
