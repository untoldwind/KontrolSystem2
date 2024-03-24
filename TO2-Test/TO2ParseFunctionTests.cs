using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using Xunit;

namespace KontrolSystem.TO2.Test;

public class TO2ParserFunctionTests {
    private static readonly string[]? IgnorePosition = { "start", "end", "parentContainer" };

    [Fact]
    public void TestFunctionParameter() {
        var result = TO2ParserFunctions.FunctionParameter.TryParse("");

        Assert.False(result.success);

        result = TO2ParserFunctions.FunctionParameter.TryParse("ab");

        Assert.False(result.success);

        result = TO2ParserFunctions.FunctionParameter.TryParse("ab:bool");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Helpers.ShouldDeepEqual(new FunctionParameter("ab", BuiltinType.Bool, null), result.value, IgnorePosition);

        result = TO2ParserFunctions.FunctionParameter.TryParse("_12ab : int");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Helpers.ShouldDeepEqual(new FunctionParameter("_12ab", BuiltinType.Int, null), result.value, IgnorePosition);
    }

    [Fact]
    public void TestFunctionDeclaration() {
        var result = TO2ParserFunctions.FunctionDeclaration.TryParse("");

        Assert.False(result.success);

        result = TO2ParserFunctions.FunctionDeclaration.TryParse("fn demo()->Unit=0");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Helpers.ShouldDeepEqual(
            new FunctionDeclaration(FunctionModifier.Private, true, "demo", "", [],
                BuiltinType.Unit, new LiteralInt(0)), result.value, IgnorePosition);

        result = TO2ParserFunctions.FunctionDeclaration.TryParse("pub  fn _demo23 ( ab : string ) -> int = { 0 }");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Helpers.ShouldDeepEqual(
            new FunctionDeclaration(FunctionModifier.Public, true, "_demo23", "",
                [new("ab", BuiltinType.String, null)], BuiltinType.Int,
                new Block([new LiteralInt(0)])), result.value, IgnorePosition);

        result = TO2ParserFunctions.FunctionDeclaration.TryParse(
            "pub  fn abc34 ( ab : string, _56 : int ) -> int = { 0 }");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Helpers.ShouldDeepEqual(
            new FunctionDeclaration(FunctionModifier.Public, true, "abc34", "",
                [new("ab", BuiltinType.String, null), new("_56", BuiltinType.Int, null)], BuiltinType.Int, new Block([new LiteralInt(0)])), result.value,
            IgnorePosition);
    }
}
