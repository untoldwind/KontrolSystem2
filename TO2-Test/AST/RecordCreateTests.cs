using Xunit;

namespace KontrolSystem.TO2.Test.AST {
    public class RecordCreateTests {
        [Fact]
        public void TestCreateSimpleRecord() {
            var block = TestHelper.CompileExpression(
                @"{ 
                    const tuple = (a: 1234, b: 12.34) 
                  }");

            Assert.Equal("(a : int, b : float)", block.FindVariable("tuple").Type.ToString());

            block.AssertCommands(
                "<declare local> 1 = System.ValueTuple`2[System.Int64,System.Double]",
                "ldloca.s <local>1",
                "dup",
                "ldc.i8 00000000000004D2",
                "stfld <field>Item1",
                "ldc.r8 1.234000E+001",
                "stfld <field>Item2");
        }
    }
}
