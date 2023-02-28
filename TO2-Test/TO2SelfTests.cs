using Xunit;
using System.IO;
using KontrolSystem.TO2.Runtime;
using Xunit.Abstractions;

namespace KontrolSystem.TO2.Test {
    [Collection("KontrolRegistry")]
    public class TO2SelfTests {
        private readonly ITestOutputHelper output;

        static readonly string TO2BaseDir = Path.Combine(".", "to2SelfTest");

        public TO2SelfTests(ITestOutputHelper output) => this.output = output;

        static string demo_target(long a) {
            return $"Called with {a}";
        }

        [Fact]
        public void TestTestContext() {
            var registry = KontrolRegistry.CreateCore();

            Assert.Contains("core::testing", registry.modules.Keys);

            try {
                registry.AddFile(TO2BaseDir, "Test-TestContext.to2");

                Assert.Contains("test_testcontext", registry.modules.Keys);

                var kontrolModule = registry.modules["test_testcontext"];

                Assert.NotNull(kontrolModule.FindFunction("test_asserts"));

                var testContext = new TestRunnerContext();
                kontrolModule.FindFunction("test_asserts").Invoke(testContext);

                Assert.Equal(9, testContext.AssertionsCount);
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    output.WriteLine(error.ToString());
                }

                throw new Xunit.Sdk.XunitException(e.Message);
            }
        }

        [Fact]
        public void TestInterop() {
            var registry = KontrolRegistry.CreateCore();

            try {
                registry.AddFile(TO2BaseDir, "Test-Interop.to2");

                Assert.Contains("test_interop", registry.modules.Keys);

                var kontrolModule = registry.modules["test_interop"];

                Assert.NotNull(kontrolModule.FindFunction("test_basic_callback"));

                var testContext = new TestRunnerContext();
                kontrolModule.FindFunction("test_basic_callback")
                    .Invoke(testContext, new System.Func<long, string>(demo_target));

                Assert.Equal(2, testContext.AssertionsCount);
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    output.WriteLine(error.ToString());
                }

                throw new Xunit.Sdk.XunitException(e.Message);
            }
        }
    }
}
