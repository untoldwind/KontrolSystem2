using Xunit;

namespace KontrolSystem.Parsing.Test;

using static Parsers;

public class SequenceTests {
    [Fact]
    public void TestPreceded() {
        var parser = Preceded(Char('a'), Char('B'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ABc");

        Assert.False(result.success);

        result = parser.TryParse("a");

        Assert.False(result.success);

        result = parser.TryParse("aBc");

        Assert.True(result.success);
        Assert.Equal("c", result.remaining.ToString());
        Assert.Equal('B', result.value);
    }

    [Fact]
    public void TestTerminated() {
        var parser = Terminated(Char('a'), Char('B'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ABc");

        Assert.False(result.success);

        result = parser.TryParse("a");

        Assert.False(result.success);

        result = parser.TryParse("aBc");

        Assert.True(result.success);
        Assert.Equal("c", result.remaining.ToString());
        Assert.Equal('a', result.value);
    }

    [Fact]
    public void TestPair() {
        var parser = Seq(Char('a'), Char('B'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ABc");

        Assert.False(result.success);

        result = parser.TryParse("a");

        Assert.False(result.success);

        result = parser.TryParse("aBc");

        Assert.True(result.success);
        Assert.Equal("c", result.remaining.ToString());
        Assert.Equal(('a', 'B'), result.value);
    }

    [Fact]
    public void TestTriple() {
        var parser = Seq(Char('a'), Char('B'), Char('c'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ABc");

        Assert.False(result.success);

        result = parser.TryParse("a");

        Assert.False(result.success);

        result = parser.TryParse("aBc");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(('a', 'B', 'c'), result.value);
    }

    [Fact]
    public void TestQuad() {
        var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ABcD");

        Assert.False(result.success);

        result = parser.TryParse("a");

        Assert.False(result.success);

        result = parser.TryParse("aBcd");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(('a', 'B', 'c', 'd'), result.value);
    }

    [Fact]
    public void TestQuint() {
        var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'), Char('E'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ABcDe");

        Assert.False(result.success);

        result = parser.TryParse("a");

        Assert.False(result.success);

        result = parser.TryParse("aBcdE");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(('a', 'B', 'c', 'd', 'E'), result.value);
    }

    [Fact]
    public void TestHexa() {
        var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'), Char('E'), Char('f'));
        var result = parser.TryParse("");

        Assert.False(result.success);

        result = parser.TryParse("ABcDef");

        Assert.False(result.success);

        result = parser.TryParse("a");

        Assert.False(result.success);

        result = parser.TryParse("aBcdEf");

        Assert.True(result.success);
        Assert.Equal("", result.remaining.ToString());
        Assert.Equal(('a', 'B', 'c', 'd', 'E', 'f'), result.value);
    }
}
