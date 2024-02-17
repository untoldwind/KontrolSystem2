using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Runtime;
using Xunit;
using Xunit.Sdk;

namespace KontrolSystem.TO2.Test;

public class REPLBasicTests {
    [Fact]
    public void TestSimpleExpressions() {
        Assert.Equal(1234, RunExpression<long>(BuiltinType.Int, "1234"));
        Assert.Equal(-1234, RunExpression<long>(BuiltinType.Int, "-1234"));
        Assert.Equal(3579, RunExpression<long>(BuiltinType.Int, "1234 + 2345"));
        Assert.Equal(-1111, RunExpression<long>(BuiltinType.Int, "1234 - 2345"));
        Assert.Equal(2893730, RunExpression<long>(BuiltinType.Int, "1234 * 2345"));
        Assert.Equal(195, RunExpression<long>(BuiltinType.Int, "2345 / 12"));
        Assert.Equal(5, RunExpression<long>(BuiltinType.Int, "2345 % 12"));
    }

    [Fact]
    public void TestVariables() {
        Assert.Equal(3579, RunExpression<long>(BuiltinType.Int, "const a = 1234\nconst b = 2345\na + b"));
        Assert.Equal(2345, RunExpression<long>(BuiltinType.Int, "let a = 1234\na = 2345\na"));
        Assert.Equal(3579, RunExpression<long>(BuiltinType.Int, "let a = 1234\na += 2345\na"));
    }

    [Fact]
    public void TestArrays() {
        Assert.Equal("[1, 2, 3, 4]", RunExpression<string>(BuiltinType.String, "[1, 2, 3, 4].to_string()"));
        Assert.Equal("[4, 3, 2, 1]", RunExpression<string>(BuiltinType.String, "[1, 2, 3, 4].reverse().to_string()"));
        Assert.Equal(4, RunExpression<long>(BuiltinType.Int, "[1, 2, 3, 4].length"));
        Assert.Equal(3, RunExpression<long>(BuiltinType.Int, "let a = [1, 2, 3, 4] a[2]"));

        Assert.Equal("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]", RunExpression<string>(BuiltinType.String, @"
                let build : ArrayBuilder<int> = ArrayBuilder(100)
                let i : int = 0

                while (i < 10) {
                    i = i + 1
                    build += i
                }

                build.result().to_string()
            "));

        Assert.Equal("|10|9|8|7|6|5|4|3|2|1|", RunExpression<string>(BuiltinType.String, @"
                let out = ""|""

                for(i in [1, 2, 3, 4, 5, 6 , 7, 8, 9, 10].reverse()) {
                    out += i.to_string()
                    out += ""|""
                }

                out
            "));

        Assert.Equal("[1, 2, 9, 4]", RunExpression<string>(BuiltinType.String, @"
                let a = [1, 2, 3, 4]

                a[2] += 6

                a.to_string()
            "));
    }

    [Fact]
    public void TestStrings() {
        Assert.Equal(5, RunExpression<long>(BuiltinType.Int, "\"Hallo\".length"));
    }

    [Fact]
    public void TestRange() {
        Assert.Equal(5, RunExpression<long>(BuiltinType.Int, "(0..5).length"));
        Assert.Equal(6, RunExpression<long>(BuiltinType.Int, "(0...5).length"));

        Assert.Equal("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]", RunExpression<string>(BuiltinType.String, @"
                let build : ArrayBuilder<int> = ArrayBuilder(100)

                for(i in 0..10) {
                    build += i + 1
                }

                build.result().to_string()
            "));
    }

    [Fact]
    public void TestBlock() {
        Assert.Equal(3579, RunExpression<long>(BuiltinType.Int, "const a = { let b = 1234\nb += 2345\nb }\na"));
        var exception = Assert.Throws<REPLException>(() =>
            RunExpression<long>(BuiltinType.Int, "const a = { let b = 1234\nb += 2345\nb }\nb"));
        Assert.Equal("<inline>(4, 1): No local variable, constant or function 'b'", exception.Message);
    }

    [Fact]
    public void TestIf() {
        Assert.Equal(1, RunExpression<long>(BuiltinType.Int, @"
                let a = 0
                let b = 10

                if(b < 20) {
                    a += 1
                }

                a
            "));

        Assert.Equal(1, RunExpression<long>(BuiltinType.Int, @"
                let a = 0
                let b = 10

                if(b < 20) {
                    a += 1
                } else {
                    a -= 1
                }

                a
            "));
        Assert.Equal(-1, RunExpression<long>(BuiltinType.Int, @"
                let a = 0
                let b = 10

                if(b > 20) {
                    a += 1
                } else {
                    a -= 1
                }

                a
            "));
    }

    [Fact]
    public void TestUse() {
        Assert.Equal(0.5, RunExpression<double>(BuiltinType.Float, @"
                use { cos_deg } from core::math

                cos_deg(60)
            "), 5);

        Assert.Equal(1, RunExpression<double>(BuiltinType.Float, @"
                use { log, E } from core::math

                log(E)
            "), 5);
    }

    private T RunExpression<T>(TO2Type to2Type, string expression) {
        var result = TO2ParserREPL.REPLItems.Parse(expression);
        var registry = KontrolRegistry.CreateCore();
        var context = new REPLContext(registry, new TestRunnerContext());
        var pollCount = 0;

        foreach (var item in result.Where(i => !(i is IBlockItem))) {
            var future = item.Eval(context);
            var futureResult = future.PollValue();

            while (!futureResult.IsReady) {
                pollCount++;
                if (pollCount > 100) throw FailException.ForFailure("No result after 100 tries");
                futureResult = future.PollValue();
            }
        }

        var mainBlock = new Block(result.OfType<IBlockItem>().ToList());
        var mainFuture = mainBlock.Eval(context);
        var mainFutureResult = mainFuture.PollValue();

        while (!mainFutureResult.IsReady) {
            pollCount++;
            if (pollCount > 100) throw FailException.ForFailure("No result after 100 tries");
            mainFutureResult = mainFuture.PollValue();
        }


        Assert.Equal(to2Type, mainFutureResult.value!.Type);
        Assert.True(mainFutureResult.value.Value is T);

        return (T)mainFutureResult.value.Value;
    }
}
