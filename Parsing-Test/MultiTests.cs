using System.Collections.Generic;
using Xunit;

namespace KontrolSystem.Parsing.Test {
    using static Parsers;

    public class MultiTests {
        [Fact]
        public void TextMany0() {
            var parser = Many0(Char('B'));
            var result = parser.TryParse("");

            Assert.True(result.WasSuccessful);
            Assert.Equal(new List<char>(), result.Value);

            result = parser.TryParse("abcde");

            Assert.True(result.WasSuccessful);
            Assert.Equal("abcde", result.Remaining.ToString());
            Assert.Equal(new List<char>(), result.Value);

            result = parser.TryParse("BBBde");

            Assert.True(result.WasSuccessful);
            Assert.Equal("de", result.Remaining.ToString());
            Assert.Equal(new List<char>(new[] { 'B', 'B', 'B' }), result.Value);
        }

        [Fact]
        public void TestMany1() {
            var parser = Many1(Char('B'));
            var result = parser.TryParse("");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("abcde");

            Assert.False(result.WasSuccessful);

            result = parser.TryParse("Bbcde");

            Assert.True(result.WasSuccessful);
            Assert.Equal("bcde", result.Remaining.ToString());
            Assert.Equal(new List<char>(new[] { 'B' }), result.Value);

            result = parser.TryParse("BBBde");

            Assert.True(result.WasSuccessful);
            Assert.Equal("de", result.Remaining.ToString());
            Assert.Equal(new List<char>(new[] { 'B', 'B', 'B' }), result.Value);
        }

        [Fact]
        public void TestDelimited0() {
            var parser = Delimited0(OneOf("abAB"), Char(',').Between(WhiteSpaces0, WhiteSpaces0));
            var result = parser.TryParse("");

            Assert.True(result.WasSuccessful);
            Assert.Empty(result.Value);

            result = parser.TryParse("a, e");

            Assert.True(result.WasSuccessful);
            Assert.Equal(", e", result.Remaining.ToString());
            Assert.Equal(new List<char> { 'a' }, result.Value);

            result = parser.TryParse("a,");

            Assert.True(result.WasSuccessful);
            Assert.Equal(",", result.Remaining.ToString());
            Assert.Equal(new List<char> { 'a' }, result.Value);

            result = parser.TryParse("a , b,a, A ,B");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(new List<char> { 'a', 'b', 'a', 'A', 'B' }, result.Value);
        }
    }
}
