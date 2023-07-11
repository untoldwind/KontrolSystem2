using KontrolSystem.TO2.Runtime;
using Xunit;

namespace KontrolSystem.TO2.Test {
    public class DirectBindingMathTests {
        [Fact]
        public void TestIntPower() {
            Assert.Equal(1, DirectBindingMath.IntPow(12, 0));
            Assert.Equal(12, DirectBindingMath.IntPow(12, 1));
            Assert.Equal(12 * 12, DirectBindingMath.IntPow(12, 2));
            Assert.Equal(12 * 12 * 12, DirectBindingMath.IntPow(12, 3));
            Assert.Equal(12 * 12 * 12 * 12, DirectBindingMath.IntPow(12, 4));
            Assert.Equal(12 * 12 * 12 * 12 * 12, DirectBindingMath.IntPow(12, 5));
        }
    }
}
