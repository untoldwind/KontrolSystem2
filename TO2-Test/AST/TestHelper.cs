using System;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Parser;
using Xunit;

namespace KontrolSystem.TO2.Test.AST {
    public class TestHelper {
        public static TestBlockContext CompileExpression(string expression) {
            var result = TO2ParserExpressions.Expression.Parse(expression);
            var context = new TestBlockContext();

            result.EmitCode(context, true);

            Assert.False(context.HasErrors, String.Join("\n", context.AllErrors));

            return context;
        }
    }
}
