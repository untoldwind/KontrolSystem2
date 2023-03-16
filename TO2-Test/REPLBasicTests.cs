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
            Assert.Equal(1234,  RunExpression<long>(BuiltinType.Int, "1234"));
            Assert.Equal(1234,  RunExpression<long>(BuiltinType.Int, "1234"));
        }
        
        private T RunExpression<T>(TO2Type to2Type, string expression) {
            var result = TO2ParserExpressions.Expression.TryParse(expression);

            Assert.True(result.WasSuccessful);

            var context = new TestRunnerContext();
            var future = result.Value.Eval(context);
            
            for (int i = 0; i < 100; i++) {
                var futureResult = future.PollValue();

                if (futureResult.IsReady) {
                    var replValue = futureResult.value;
                    
                    Assert.Equal(to2Type, replValue.Type);
                    Assert.True(replValue.Value is T);

                    return (T)replValue.Value;
                }
            }

            throw new FailException("No result after 100 tries");
        }
    }
}
