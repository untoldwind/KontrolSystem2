using Xunit;

namespace KontrolSystem.Parsing.Test {
    using static Parsers;

    public class CompleteTests {
        [Fact]
        public void TestChar() {
            var parser = Char('A');
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("abc");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ABC");

            Assert.True(result.WasSuccessful);
            Assert.Equal("BC", result.Remaining.ToString());
            Assert.Equal('A', result.Value);
        }

        [Fact]
        public void TestTag() {
            var parser = Tag("abc");
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("ab");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("abc");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal("abc", result.Value);

            result = parser.TryParse("abcde");

            Assert.True(result.WasSuccessful);
            Assert.Equal("de", result.Remaining.ToString());
            Assert.Equal("abc", result.Value);
        }

        [Fact]
        public void TestWhitespaces() {
            var parser = WhiteSpaces0;
            var result = parser.TryParse(" \t\r\n");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(" \t\r\n", result.Value);
        }
    }
}
