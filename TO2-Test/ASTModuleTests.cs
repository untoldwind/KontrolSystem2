using Xunit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    public class AstModuleTests {
        [Fact]
        public void TestBuildModuleName() {
            Assert.Equal("test_testcontext", TO2Module.BuildName("Test-TestContext.to2"));
            Assert.Equal("sub::mod::demo", TO2Module.BuildName("sub\\mod\\demo.to2"));
            Assert.Equal("sub::mod::demo34", TO2Module.BuildName("sub/mod/demo34"));
        }
    }
}
