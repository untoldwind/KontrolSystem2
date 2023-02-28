using Xunit;

namespace KontrolSystem.Parsing.Test {
    using static Parsers;

    public class SequenceTests {
        [Fact]
        public void TestPreceded() {
            var parser = Preceded(Char('a'), Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.Equal("c", result.Remaining.ToString());
            Assert.Equal('B', result.Value);
        }

        [Fact]
        public void TestTerminated() {
            var parser = Terminated(Char('a'), Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.Equal("c", result.Remaining.ToString());
            Assert.Equal('a', result.Value);
        }

        [Fact]
        public void TestPair() {
            var parser = Seq(Char('a'), Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.Equal("c", result.Remaining.ToString());
            Assert.Equal(('a', 'B'), result.Value);
        }

        [Fact]
        public void TestTriple() {
            var parser = Seq(Char('a'), Char('B'), Char('c'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBc");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(('a', 'B', 'c'), result.Value);
        }

        [Fact]
        public void TestQuad() {
            var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABcD");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBcd");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(('a', 'B', 'c', 'd'), result.Value);
        }

        [Fact]
        public void TestQuint() {
            var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'), Char('E'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABcDe");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBcdE");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(('a', 'B', 'c', 'd', 'E'), result.Value);
        }

        [Fact]
        public void TestHexa() {
            var parser = Seq(Char('a'), Char('B'), Char('c'), Char('d'), Char('E'), Char('f'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABcDef");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("a");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("aBcdEf");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(('a', 'B', 'c', 'd', 'E', 'f'), result.Value);
        }
    }
}
