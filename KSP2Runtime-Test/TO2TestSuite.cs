using System.IO;
using KontrolSystem.KSP.Runtime.Testing;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Tooling;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KontrolSystem.KSP.Runtime.Test;

public class TO2TestSuite {
    private readonly ITestOutputHelper output;

    public TO2TestSuite(ITestOutputHelper output) {
        this.output = output;
    }

    [Fact]
    public void RunSuite() {
        var reporter = new ConsoleTestReporter(output.WriteLine);

        try {
            var registry = KontrolSystemKSPRegistry.CreateKSP();

            var context = registry.AddDirectory(Path.Combine(".", "to2KSP"));

            context.Save("demo.dll");

            TestRunner.RunTests(registry, reporter, () => new KSPTestRunnerContext());
        } catch (CompilationErrorException e) {
            foreach (var error in e.errors) output.WriteLine(error.ToString());

            throw new XunitException(e.Message);
        }

        if (!reporter.WasSuccessful) {
            if (reporter.Failures.Count > 0) {
                output.WriteLine("");
                output.WriteLine("Failures:");
                output.WriteLine("");

                foreach (var failure in reporter.Failures) {
                    output.WriteLine($"    {failure.testName}:");
                    output.WriteLine($"         {failure.failure}");
                }
            }

            if (reporter.Errors.Count > 0) {
                output.WriteLine("");
                output.WriteLine("Errors:");
                output.WriteLine("");

                foreach (var error in reporter.Errors) {
                    output.WriteLine($"    {error.testName}:");
                    output.WriteLine(error.exception?.ToString());
                }
            }

            throw new XunitException("KSSTestSuite was not successful");
        }
    }
}
