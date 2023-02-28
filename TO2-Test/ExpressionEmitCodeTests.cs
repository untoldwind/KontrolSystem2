using Xunit;
using System;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Parser;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    public class ExpressionEmitCodeTests {
        static MethodInfo GenerateMethod(Expression expression, TO2Type returnType) {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("DynamicExpression");
            IBlockContext methodContext = moduleContext.CreateMethodContext(FunctionModifier.Public, false, "Exec",
                returnType, Enumerable.Empty<FunctionParameter>());

            expression.EmitCode(methodContext, false);
            Assert.False(methodContext.HasErrors);
            methodContext.IL.EmitReturn(methodContext.MethodBuilder.ReturnType);

            Type dynamicType = moduleContext.CreateType();

            return dynamicType.GetMethod("Exec");
        }

        [Fact]
        public void TestLiteral() {
            MethodInfo method = GenerateMethod(new LiteralInt(1234), BuiltinType.Int);
            var result = method.Invoke(null, new object[0]);

            Assert.Equal(1234L, result);

            method = GenerateMethod(new LiteralFloat(1234.56), BuiltinType.Float);
            result = method.Invoke(null, new object[0]);

            Assert.Equal(1234.56, result);

            method = GenerateMethod(new LiteralBool(true), BuiltinType.Bool);
            result = method.Invoke(null, new object[0]);

            Assert.Equal(true, result);

            method = GenerateMethod(new LiteralString("abcded"), BuiltinType.String);
            result = method.Invoke(null, new object[0]);

            Assert.Equal("abcded", result);
        }

        [Fact]
        public void TestSimpleCalc() {
            MethodInfo method = GenerateMethod(TO2ParserExpressions.Expression.Parse("1234 + 4321"), BuiltinType.Int);
            var result = method.Invoke(null, new object[0]);

            Assert.Equal(5555L, result);
        }
    }
}
