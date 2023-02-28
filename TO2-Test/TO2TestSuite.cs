using System.IO;
using KontrolSystem.TO2.Tooling;
using KontrolSystem.TO2.Binding;
using Xunit;
using Xunit.Abstractions;

namespace KontrolSystem.TO2.Test {
    [Collection("KontrolRegistry")]
    public class TO2TestSuite {
        private readonly ITestOutputHelper output;

        public TO2TestSuite(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void RunSuite() {
            ConsoleTestReporter reporter = new ConsoleTestReporter(output.WriteLine);

            try {
                var registry = KontrolRegistry.CreateCore();

                registry.RegisterModule(BindingGenerator.BindModule(typeof(TestModule)));

                var context = registry.AddDirectory(Path.Combine(".", "to2Core"));

                context.Save("demo.dll");

                TestRunner.RunTests(registry, reporter, TestRunner.DefaultTestContextFactory);
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    output.WriteLine(error.ToString());
                }

                throw new Xunit.Sdk.XunitException(e.Message);
            }

            if (!reporter.WasSuccessful) {
                if (reporter.Failures.Count > 0) {
                    output.WriteLine("");
                    output.WriteLine("Failures:");
                    output.WriteLine("");

                    foreach (TestResult failure in reporter.Failures) {
                        output.WriteLine($"    {failure.testName}:");
                        output.WriteLine($"         {failure.failure}");
                    }
                }

                if (reporter.Errors.Count > 0) {
                    output.WriteLine("");
                    output.WriteLine("Errors:");
                    output.WriteLine("");

                    foreach (TestResult error in reporter.Errors) {
                        output.WriteLine($"    {error.testName}:");
                        output.WriteLine(error.exception.ToString());
                    }
                }

                throw new Xunit.Sdk.XunitException("KSSTestSuite was not successful");
            }
        }
    }
}
