using Xunit;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Test {
    public class ILEmitterTests {
        [Fact]
        public void TestStackCount() {
            Assert.Equal(StackBehaviour.Pop0, OpCodes.Ldarg_0.StackBehaviourPop);
            Assert.Equal(StackBehaviour.Push1, OpCodes.Ldarg_0.StackBehaviourPush);

            Assert.Equal(StackBehaviour.Pop1, OpCodes.Starg.StackBehaviourPop);
            Assert.Equal(StackBehaviour.Push0, OpCodes.Starg.StackBehaviourPush);

            Assert.Equal(StackBehaviour.Varpop, OpCodes.Call.StackBehaviourPop);
            Assert.Equal(StackBehaviour.Varpush, OpCodes.Call.StackBehaviourPush);
        }
    }
}
