using System.Collections.Generic;
using Xunit;

namespace KontrolSystem.Parsing.Test;

using static Parsers;

public class MultiTests {
    [Fact]
    public void TextMany0() {
        var parser = Many0(Char('B'));
        var result = parser.TryParse("");

        Assert.True(result.success);
        Assert.Equal([], result.value);

        result = parser.TryParse("abcde");

        Assert.True(result.success);
        Assert.Equal("abcde", result.remaining.ToString());
        Assert.Equal([], result.value);

        result = parser.TryParse("BBBde");

        Assert.True(result.success);
        Assert.Equal("de", result.remaining.ToString());
        Assert.Equal(new List<char>(['B', 'B', 'B']), result.value);
    }

    [Fact]
    public void TestMany1() {
        var parser = Many1(Char('B'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("abcde");

        Assert.False(result.success);

        result = parser.TryParse("Bbcde");

        Assert.True(result.success);
        Assert.Equal("bcde", result.remaining.ToString());
        Assert.Equal(new List<char>(['B']), result.value);

        result = parser.TryParse("BBBde");

        Assert.True(result.success);
        Assert.Equal("de", result.remaining.ToString());
        Assert.Equal(new List<char>(['B', 'B', 'B']), result.value);
    }

    [Fact]
    public void TestDelimited0() {
        var parser = Delimited0(OneOf("abAB"), Char(',').Between(WhiteSpaces0, WhiteSpaces0));
        var result = parser.TryParse("");

        Assert.True(result.success);
        Assert.Empty(result.value);

        result = parser.TryParse("a, e");

        Assert.True(result.success);
        Assert.Equal(", e", result.remaining.ToString());
        Assert.Equal(['a'], result.value);

        result = parser.TryParse("a,");

        Assert.True(result.success);
        Assert.Equal(",", result.remaining.ToString());
        Assert.Equal(['a'], result.value);

        result = parser.TryParse("a , b,a, A ,B");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(['a', 'b', 'a', 'A', 'B'], result.value);
    }
}
