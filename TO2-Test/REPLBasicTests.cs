using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Runtime;
using Xunit;
using Xunit.Sdk;

namespace KontrolSystem.TO2.Test {
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
        }

        private T RunExpression<T>(TO2Type to2Type, string expression) {
            var result = TO2ParserREPL.REPLItems.TryParse(expression);

            Assert.True(result.WasSuccessful);

            var context = new REPLContext(new TestRunnerContext());
            var pollCount = 0;
            IREPLValue lastValue = REPLUnit.INSTANCE;

            foreach (var item in result.Value) {
                var future = item.Eval(context);
                var futureResult = future.PollValue();

                while (!futureResult.IsReady) {
                    pollCount++;
                    if(pollCount > 100) throw new FailException("No result after 100 tries");
                    futureResult = future.PollValue();
                }

                lastValue = futureResult.value;
            }
            
            Assert.Equal(to2Type, lastValue.Type);
            Assert.True(lastValue.Value is T);

            return (T)lastValue.Value;
        }
    }
}
