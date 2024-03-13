using Xunit;

namespace KontrolSystem.Parsing.Test;

using static Parsers;

public class CompleteTests {
    [Fact]
    public void TestChar() {
        var parser = Char('A');
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("abc");

        Assert.False(result.success);

        result = parser.TryParse("ABC");

        Assert.True(result.success);
        Assert.Equal("BC", result.remaining.ToString());
        Assert.Equal('A', result.value);
    }

    [Fact]
    public void TestTag() {
        var parser = Tag("abc");
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ab");

        Assert.False(result.success);

        result = parser.TryParse("abc");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.True(result.value);

        result = parser.TryParse("abcde");

        Assert.True(result.success);
        Assert.Equal("de", result.remaining.ToString());
        Assert.True(result.value);
    }

    [Fact]
    public void TestWhitespaces() {
        var parser = WhiteSpaces0;
        var result = parser.TryParse(" \t\r\n");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(" \t\r\n", result.value);
    }
}
