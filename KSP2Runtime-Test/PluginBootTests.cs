using KontrolSystem.KSP.Runtime.Testing;
using KontrolSystem.TO2;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KontrolSystem.KSP.Runtime.Test;

public class PluginBootTests(ITestOutputHelper output) {
    [Fact]
    public void RunSuite() {
        try {
            var registry = KontrolSystemKSPRegistry.CreateKSP();

            //                registry.AddDirectory(Path.Combine("..", "..", "GameData", "KontrolSystem", "to2"));
            //                registry.AddDirectory(Path.Combine(Path.Combine(".", "to2Sample")));
        } catch (CompilationErrorException e) {
            foreach (var error in e.errors) output.WriteLine(error.ToString());

            throw new XunitException(e.Message);
        }
    }
}
