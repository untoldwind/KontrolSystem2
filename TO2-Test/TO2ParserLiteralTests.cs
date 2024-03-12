using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using Xunit;

namespace KontrolSystem.TO2.Test;

public class TO2ParserLiteralTests {
    [Fact]
    public void TestLiteralString() {
        var result = TO2ParserLiterals.LiteralString.TryParse("");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralString.TryParse("\"\"");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal("", (result.value as LiteralString)!.value);

        result = TO2ParserLiterals.LiteralString.TryParse("\"abcd\"");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal("abcd", (result.value as LiteralString)!.value);
    }

    [Fact]
    public void TestLiteralInt() {
        var result = TO2ParserLiterals.LiteralInt.TryParse("");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralInt.TryParse("abcd");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralInt.TryParse("-abcd");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralInt.TryParse("1234");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(1234L, (result.value as LiteralInt)!.value);

        result = TO2ParserLiterals.LiteralInt.TryParse("-4321");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(-4321L, (result.value as LiteralInt)!.value);

        result = TO2ParserLiterals.LiteralInt.TryParse("0x1234");
        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(0x1234L, (result.value as LiteralInt)!.value);

        result = TO2ParserLiterals.LiteralInt.TryParse("0b1001");
        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(9L, (result.value as LiteralInt)!.value);

        result = TO2ParserLiterals.LiteralInt.TryParse("0o1234");
        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(668L, (result.value as LiteralInt)!.value);

        result = TO2ParserLiterals.LiteralInt.TryParse("1_234_567_890");
        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(1234567890L, (result.value as LiteralInt)!.value);
    }

    [Fact]
    public void TestLiteralFloat() {
        var result = TO2ParserLiterals.LiteralFloat.TryParse("");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralFloat.TryParse("abcd");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralFloat.TryParse("-abcd");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralFloat.TryParse("1234");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralFloat.TryParse(".1234");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(.1234, (result.value as LiteralFloat)!.value);

        result = TO2ParserLiterals.LiteralFloat.TryParse("1.234");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(1.234, (result.value as LiteralFloat)!.value);

        result = TO2ParserLiterals.LiteralFloat.TryParse("-.1234");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(-.1234, (result.value as LiteralFloat)!.value);

        result = TO2ParserLiterals.LiteralFloat.TryParse("-1.234");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(-1.234, (result.value as LiteralFloat)!.value);
    }

    [Fact]
    public void TestLiteralBool() {
        var result = TO2ParserLiterals.LiteralBool.TryParse("");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralBool.TryParse("tru");

        Assert.False(result.success);

        result = TO2ParserLiterals.LiteralBool.TryParse("true");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.True((result.value as LiteralBool)!.value);

        result = TO2ParserLiterals.LiteralBool.TryParse("false");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.False((result.value as LiteralBool)!.value);
    }
}
