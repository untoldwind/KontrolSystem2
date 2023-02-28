using Xunit;
using KontrolSystem.TO2.Parser;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    public class TO2ParserLiteralTests {
        [Fact]
        public void TestLiteralString() {
            var result = TO2ParserLiterals.LiteralString.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralString.TryParse("\"\"");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal("", result.Value.value);

            result = TO2ParserLiterals.LiteralString.TryParse("\"abcd\"");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal("abcd", result.Value.value);
        }

        [Fact]
        public void TestLiteralInt() {
            var result = TO2ParserLiterals.LiteralInt.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralInt.TryParse("abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralInt.TryParse("-abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralInt.TryParse("1234");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(1234L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("-4321");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(-4321L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("0x1234");
            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(0x1234L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("0b1001");
            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(9L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("0o1234");
            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(668L, result.Value.value);

            result = TO2ParserLiterals.LiteralInt.TryParse("1_234_567_890");
            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(1234567890L, result.Value.value);
        }

        [Fact]
        public void TestLiteralFloat() {
            var result = TO2ParserLiterals.LiteralFloat.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse("abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse("-abcd");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse("1234");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralFloat.TryParse(".1234");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(.1234, result.Value.value);

            result = TO2ParserLiterals.LiteralFloat.TryParse("1.234");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(1.234, result.Value.value);

            result = TO2ParserLiterals.LiteralFloat.TryParse("-.1234");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(-.1234, result.Value.value);

            result = TO2ParserLiterals.LiteralFloat.TryParse("-1.234");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.Equal(-1.234, result.Value.value);
        }

        [Fact]
        public void TestLiteralBool() {
            var result = TO2ParserLiterals.LiteralBool.TryParse("");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralBool.TryParse("tru");

            Assert.False(result.WasSuccessful);

            result = TO2ParserLiterals.LiteralBool.TryParse("true");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.True(result.Value.value);

            result = TO2ParserLiterals.LiteralBool.TryParse("false");

            Assert.True(result.WasSuccessful);
            Assert.Equal("", result.Remaining.ToString());
            Assert.False(result.Value.value);
        }
    }
}
