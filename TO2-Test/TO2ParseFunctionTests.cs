using System.Collections.Generic;
using Xunit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    public class TO2ParserFunctionTests {
        static readonly string[] IgnorePosition = { "start", "end", "parentContainer" };

        [Fact]
        public void TestFunctionParameter() {
            var result = TO2ParserFunctions.FunctionParameter.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserFunctions.FunctionParameter.TryParse("ab");

            Assert.False(result.WasSuccessful);

            result = TO2ParserFunctions.FunctionParameter.TryParse("ab:bool");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new FunctionParameter("ab", BuiltinType.Bool), result.Value, IgnorePosition);

            result = TO2ParserFunctions.FunctionParameter.TryParse("_12ab : int");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(new FunctionParameter("_12ab", BuiltinType.Int), result.Value, IgnorePosition);
        }

        [Fact]
        public void TestFunctionDeclaration() {
            var result = TO2ParserFunctions.FunctionDeclaration.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserFunctions.FunctionDeclaration.TryParse("fn demo()->Unit=0");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(
                new FunctionDeclaration(FunctionModifier.Private, true, "demo", "", new List<FunctionParameter>(),
                    BuiltinType.Unit, new LiteralInt(0)), result.Value, IgnorePosition);

            result = TO2ParserFunctions.FunctionDeclaration.TryParse("pub  fn _demo23 ( ab : string ) -> int = { 0 }");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(
                new FunctionDeclaration(FunctionModifier.Public, true, "_demo23", "",
                    new List<FunctionParameter> { new FunctionParameter("ab", BuiltinType.String) }, BuiltinType.Int,
                    new Block(new List<IBlockItem> { new LiteralInt(0) })), result.Value, IgnorePosition);

            result = TO2ParserFunctions.FunctionDeclaration.TryParse(
                "pub  fn abc34 ( ab : string, _56 : int ) -> int = { 0 }");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Helpers.ShouldDeepEqual(
                new FunctionDeclaration(FunctionModifier.Public, true, "abc34", "",
                    new List<FunctionParameter> {
                        new FunctionParameter("ab", BuiltinType.String), new FunctionParameter("_56", BuiltinType.Int)
                    }, BuiltinType.Int, new Block(new List<IBlockItem> { new LiteralInt(0) })), result.Value,
                IgnorePosition);
        }
    }
}
