using System.Diagnostics;
using System.IO;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace KontrolSystem.TO2.Test;

public class REPLCompiledTests {
    private readonly KontrolRegistry registry;
    private readonly ITestOutputHelper output;

    public REPLCompiledTests(ITestOutputHelper output) {
        this.output = output;
        try {
            registry = KontrolRegistry.CreateCore();

            registry.RegisterModule(BindingGenerator.BindModule(typeof(TestModule)));

            //            var context = registry.AddDirectory(Path.Combine(".", "to2Core"));

            //            context.Save("demo_repl.dll");
        } catch (CompilationErrorException e) {
            foreach (var error in e.errors) output.WriteLine(error.ToString());

            throw new XunitException(e.Message);
        }
    }

    [Fact]
    public void TestSimpleExpressions() {

        Assert.Equal(1234, RunExpression<long>(registry, "1234"));
        Assert.Equal(-1234, RunExpression<long>(registry, "-1234"));
        Assert.Equal(3579, RunExpression<long>(registry, "1234 + 2345"));
        Assert.Equal(-1111, RunExpression<long>(registry, "1234 - 2345"));
        Assert.Equal(2893730, RunExpression<long>(registry, "1234 * 2345"));
        Assert.Equal(195, RunExpression<long>(registry, "2345 / 12"));
        Assert.Equal(5, RunExpression<long>(registry, "2345 % 12"));
    }

    [Fact]
    public void TestVariables() {
        Assert.Equal(3579, RunExpression<long>(registry, "const a = 1234\nconst b = 2345\na + b"));
        Assert.Equal(2345, RunExpression<long>(registry, "let a = 1234\na = 2345\na"));
        Assert.Equal(3579, RunExpression<long>(registry, "let a = 1234\na += 2345\na"));
    }

    [Fact]
    public void TestArrays() {
        Assert.Equal("[1, 2, 3, 4]", RunExpression<string>(registry, "[1, 2, 3, 4].to_string()"));
        Assert.Equal("[4, 3, 2, 1]", RunExpression<string>(registry, "[1, 2, 3, 4].reverse().to_string()"));
        Assert.Equal(4, RunExpression<long>(registry, "[1, 2, 3, 4].length"));
        Assert.Equal(3, RunExpression<long>(registry, "let a = [1, 2, 3, 4] a[2]"));

        Assert.Equal("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]", RunExpression<string>(registry, @"
                let build : ArrayBuilder<int> = ArrayBuilder(100)
                let i : int = 0

                while (i < 10) {
                    i = i + 1
                    build += i
                }

                build.result().to_string()
            "));

        Assert.Equal("|10|9|8|7|6|5|4|3|2|1|", RunExpression<string>(registry, @"
                let out = ""|""

                for(i in [1, 2, 3, 4, 5, 6 , 7, 8, 9, 10].reverse()) {
                    out += i.to_string()
                    out += ""|""
                }

                out
            "));

        Assert.Equal("[1, 2, 9, 4]", RunExpression<string>(registry, @"
                let a = [1, 2, 3, 4]

                a[2] += 6

                a.to_string()
            "));
    }

    [Fact]
    public void TestStrings() {
        Assert.Equal(5, RunExpression<long>(registry, "\"Hallo\".length"));
    }

    [Fact]
    public void TestRange() {
        Assert.Equal(5, RunExpression<long>(registry, "(0..5).length"));
        Assert.Equal(6, RunExpression<long>(registry, "(0...5).length"));

        Assert.Equal("[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]", RunExpression<string>(registry, @"
                let build : ArrayBuilder<int> = ArrayBuilder(100)

                for(i in 0..10) {
                    build += i + 1
                }

                build.result().to_string()
            "));
    }

    [Fact]
    public void TestBlock() {
        Assert.Equal(3579, RunExpression<long>(registry, "const a = { let b = 1234\nb += 2345\nb }\na"));
        var exception = Assert.Throws<CompilationErrorException>(() =>
            RunExpression<long>(registry, "const a = { let b = 1234\nb += 2345\nb }\nb"));
        Assert.Equal("<inline>(4, 1): ERROR NoSuchVariable: No local variable, constant or function 'b'", exception.errors.First().ToString());
    }

    [Fact]
    public void TestIf() {
        Assert.Equal(1, RunExpression<long>(registry, @"
                let a = 0
                let b = 10

                if(b < 20) {
                    a += 1
                }

                a
            "));

        Assert.Equal(1, RunExpression<long>(registry, @"
                let a = 0
                let b = 10

                if(b < 20) {
                    a += 1
                } else {
                    a -= 1
                }

                a
            "));
        Assert.Equal(-1, RunExpression<long>(registry, @"
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
    public void TestIssue160() {
        Assert.Equal("34", RunExpression<string>(registry, @"
                (if (1 < 0) 12 else 34).to_string()
            "));
    }

    [Fact]
    public void TestUse() {
        Assert.Equal(0.5, RunExpression<double>(registry, @"
                use { cos_deg } from core::math

                cos_deg(60)
            "), 5);

        Assert.Equal(1, RunExpression<double>(registry, @"
                use { log, E } from core::math

                log(E)
            "), 5);
    }

    private T RunExpression<T>(KontrolRegistry registry, string expression) {
        var context = new TestRunnerContext();

        var future = registry.RunREPL(context, expression);
        ContextHolder.CurrentContext.Value = context;
        var futureResult = future.Poll();
        var pollCount = 0;

        while (!futureResult.IsReady) {
            pollCount++;
            if (pollCount > 100) throw FailException.ForFailure("No result after 100 tries");
            futureResult = future.Poll();
        }

        var result = futureResult.ValueObject;

        Assert.IsType<T>(result);

        return (T)result;
    }
}
