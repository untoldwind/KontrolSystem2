using Xunit;
using Xunit.Abstractions;
using System.IO;
using KontrolSystem.TO2;

namespace KontrolSystem.KSP.Runtime.Test {
    public class PluginBootTests {
        private readonly ITestOutputHelper output;

        public PluginBootTests(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void RunSuite() {
            try {
                var registry = KontrolSystemKSPRegistry.CreateKSP();

                //                registry.AddDirectory(Path.Combine("..", "..", "GameData", "KontrolSystem", "to2"));
                //                registry.AddDirectory(Path.Combine(Path.Combine(".", "to2Sample")));


            } catch (CompilationErrorException e) {
                foreach (var error in e.errors) {
                    output.WriteLine(error.ToString());
                }

                throw new Xunit.Sdk.XunitException(e.Message);
            }
        }
    }
}
