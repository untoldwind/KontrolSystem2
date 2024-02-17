using KontrolSystem.Parsing;
using KontrolSystem.TO2.Parser;
using Xunit;

namespace KontrolSystem.TO2.Test;

public class TO2ParserCommonTests {
    [Fact]
    public void TestIdentifier() {
        var result = TO2ParserCommon.Identifier.TryParse("");

        Assert.False(result.WasSuccessful);

        result = TO2ParserCommon.Identifier.TryParse("12ab");

        Assert.False(result.WasSuccessful);

        result = TO2ParserCommon.Identifier.TryParse("ab12_");

        Assert.True(result.WasSuccessful);
        Assert.Equal("", result.Remaining.ToString());
        Assert.Equal("ab12_", result.Value);

        result = TO2ParserCommon.Identifier.TryParse("_12ab");

        Assert.True(result.WasSuccessful);
        Assert.Equal("", result.Remaining.ToString());
        Assert.Equal("_12ab", result.Value);
    }

    [Fact]
    public void TestRecordDecl() {
        var result = TO2ParserCommon.RecordType.TryParse("(a: int, b: float)");

        Assert.True(result.WasSuccessful);

        result = TO2ParserCommon.RecordType.TryParse(@"(  // First line comment
                a: int, b: float)");

        Assert.True(result.WasSuccessful);

        result = TO2ParserCommon.RecordType.TryParse(@"(  // First line comment
                a: int,  // second line comment 
                b: float)");

        Assert.True(result.WasSuccessful);

        result = TO2ParserCommon.RecordType.TryParse(@"(  // First line comment
                a: int,  // second line comment 
                b: float // last line comment
            )");

        Assert.True(result.WasSuccessful);
    }
}
