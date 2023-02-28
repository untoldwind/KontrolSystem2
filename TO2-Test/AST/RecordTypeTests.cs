using Xunit;
using Xunit.Abstractions;

namespace KontrolSystem.TO2.Test.AST {
    public class RecordTypeTests {
        private readonly ITestOutputHelper output;

        public RecordTypeTests(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void TestRecordUpdate() {
            var block = TestHelper.CompileExpression(
                @"{ 
                    const tuple1 = (done: false, a: 1234, b: 12.34)
                    const tuple2 = tuple1 & (done: true)
                  }");

            output.WriteLine(block.ILCommands);

            Assert.Equal("(a : int, b : float, done : bool)", block.FindVariable("tuple1").Type.ToString());
            Assert.Equal("(a : int, b : float, done : bool)", block.FindVariable("tuple2").Type.ToString());

            block.AssertCommands(
                "<declare local> 1 = System.ValueTuple`3[System.Int64,System.Double,System.Boolean]",
                "ldloca.s <local>1",
                "dup",
                "ldc.i8 00000000000004D2",
                "stfld <field>Item1",
                "dup",
                "ldc.r8 1.234000E+001",
                "stfld <field>Item2",
                "ldc.i4.0",
                "stfld <field>Item3",
                "<declare local> 2 = System.ValueTuple`3[System.Int64,System.Double,System.Boolean]",
                "ldloc.1",
                "<declare local> 3 = System.ValueTuple`1[System.Boolean]",
                "ldloca.s <local>3",
                "ldc.i4.1",
                "stfld <field>Item1",
                "ldloc.3",
                "<declare local> 4 = System.ValueTuple`1[System.Boolean]",
                "stloc.s <local>4",
                "<declare local> 5 = System.ValueTuple`3[System.Int64,System.Double,System.Boolean]",
                "stloc.s <local>5",
                "ldloca.s <local>5",
                "dup",
                "ldloca.s <local>4",
                "ldfld <field>Item1",
                "stfld <field>Item3",
                "pop",
                "ldloc.s <local>5",
                "stloc.2");
        }
    }
}
