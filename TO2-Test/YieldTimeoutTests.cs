using System.IO;
using Xunit;
using KontrolSystem.TO2.Tooling;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using Xunit.Abstractions;

namespace KontrolSystem.TO2.Test {
    public class YieldTimeoutTests {
        private readonly ITestOutputHelper output;

        static readonly string TO2BaseDir = Path.Combine(".", "to2SelfTest");

        public YieldTimeoutTests(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void SyncInfiniteLoop() {
            var function = SetupModule().FindFunction("sync_infinite_loop");
            Assert.NotNull(function);

            var result = function.Invoke(TestRunner.DefaultTestContextFactory(), 10L);
            Assert.Equal(55L, result);

            Assert.Throws<YieldTimeoutException>(() => {
                function.Invoke(TestRunner.DefaultTestContextFactory(), -10L);
            });
        }

        [Fact]
        public void AsyncInfiniteLoop() {
            var function = SetupModule().FindFunction("async_infinite_loop");
            Assert.NotNull(function);

            var context = TestRunner.DefaultTestContextFactory();
            var future = function.Invoke(context, 10L) as Future<long>;
            int i;
            long result = 0;

            for (i = 0; i < 20; i++) {
                context.ResetTimeout();
                ContextHolder.CurrentContext.Value = context;
                var pollResult = future!.PollValue();
                if (pollResult.IsReady) {
                    result = pollResult.value;
                    break;
                }
            }

            Assert.Equal(55L, result);
            Assert.Equal(9, i);

            Assert.Throws<YieldTimeoutException>(() => {
                var nextFuture = function.Invoke(context, -10L) as Future<long>;

                for (i = 0; i < 20; i++) {
                    context.ResetTimeout();
                    ContextHolder.CurrentContext.Value = context;
                    if (nextFuture!.PollValue().IsReady) break;
                }
            });
        }

        private IKontrolModule SetupModule() {
            try {
                var registry = KontrolRegistry.CreateCore();

                registry.RegisterModule(BindingGenerator.BindModule(typeof(TestModule)));

                registry.AddFile(TO2BaseDir, "Test-Timeout.to2");

                var kontrolModule = registry.modules["test_timeout"];
                Assert.NotNull(kontrolModule);

                return kontrolModule;
            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    output.WriteLine(error.ToString());
                }

                throw new Xunit.Sdk.XunitException(e.Message);
            }
        }
    }
}
