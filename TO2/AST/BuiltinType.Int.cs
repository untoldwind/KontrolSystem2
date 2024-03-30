using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType {
    private class TO2Int : BuiltinType {
        private readonly OperatorCollection allowedPrefixOperators;
        private readonly OperatorCollection allowedSuffixOperators;

        internal TO2Int() {
            allowedPrefixOperators = new OperatorCollection {
                {
                    Operator.Neg,
                    new DirectOperatorEmitter(() => Unit, () => Int, REPLInt.Neg, OpCodes.Neg)
                }
            };
            allowedSuffixOperators = new OperatorCollection {
                {
                    Operator.Add,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Add, OpCodes.Add)
                }, {
                    Operator.AddAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Add, OpCodes.Add)
                }, {
                    Operator.Sub,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Sub, OpCodes.Sub)
                }, {
                    Operator.SubAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Sub, OpCodes.Sub)
                }, {
                    Operator.Mul,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Mul, OpCodes.Mul)
                }, {
                    Operator.MulAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Mul, OpCodes.Mul)
                }, {
                    Operator.Div,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Div, OpCodes.Div)
                }, {
                    Operator.DivAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Div, OpCodes.Div)
                }, {
                    Operator.Mod,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Rem, OpCodes.Rem)
                }, {
                    Operator.ModAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.Rem, OpCodes.Rem)
                }, {
                    Operator.BitOr,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.BitOr, OpCodes.Or)
                }, {
                    Operator.BitOrAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.BitOr, OpCodes.Or)
                }, {
                    Operator.BitAnd,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.BitAnd, OpCodes.And)
                }, {
                    Operator.BitAndAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.BitAnd, OpCodes.And)
                }, {
                    Operator.BitXor,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.BitXor, OpCodes.Xor)
                }, {
                    Operator.BitXorAssign,
                    new DirectOperatorEmitter(() => Int, () => Int, REPLInt.BitXor, OpCodes.Xor)
                }, {
                    Operator.Pow,
                    new StaticMethodOperatorEmitter(() => Int, () => Int, typeof(DirectBindingMath).GetMethod("IntPow"))
                }, {
                    Operator.PowAssign,
                    new StaticMethodOperatorEmitter(() => Int, () => Int, typeof(DirectBindingMath).GetMethod("IntPow"))
                }, {
                    Operator.Eq,
                    new DirectOperatorEmitter(() => Int, () => Bool, REPLInt.Eq, OpCodes.Ceq)
                }, {
                    Operator.NotEq,
                    new DirectOperatorEmitter(() => Int, () => Bool, REPLInt.Neq, OpCodes.Ceq,
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }, {
                    Operator.Gt,
                    new DirectOperatorEmitter(() => Int, () => Bool, REPLInt.Gt, OpCodes.Cgt)
                }, {
                    Operator.Lt,
                    new DirectOperatorEmitter(() => Int, () => Bool, REPLInt.Lt, OpCodes.Clt)
                }, {
                    Operator.Ge,
                    new DirectOperatorEmitter(() => Int, () => Bool, REPLInt.Geq, OpCodes.Clt,
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }, {
                    Operator.Le,
                    new DirectOperatorEmitter(() => Int, () => Bool, REPLInt.Leq, OpCodes.Cgt,
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }
            };
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert integer to string", true, () => String,
                        () => [], false, typeof(FormatUtils),
                        typeof(FormatUtils).GetMethod("IntToString"))
                }
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {
                    "to_bool",
                    new InlineFieldAccessFactory("Value converted to bool (0 -> false, != 0 -> true)",
                        () => Bool, REPLInt.ToBool, OpCodes.Conv_I4)
                }, {
                    "to_float",
                    new InlineFieldAccessFactory("Value converted to float", () => Float,
                        REPLInt.ToFloat, OpCodes.Conv_R8)
                }, {
                    "abs",
                    new BoundPropertyLikeFieldAccessFactory("Absolute value", () => Int, typeof(Math),
                        typeof(Math).GetMethod("Abs", [typeof(long)]), null)
                }, {
                    "sign",
                    new BoundPropertyLikeFieldAccessFactory("Sign of the value (< 0 -> -1, 0 -> 0, > 0 -> 1)",
                        () => Int, typeof(Math),
                        typeof(Math).GetMethod("Sign", [typeof(long)]), null, OpCodes.Conv_I8)
                }
            };
        }

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public override string Name => "int";

        public override Type GeneratedType(ModuleContext context) => typeof(long);

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override IREPLValue REPLCast(object? value) {
            if (value is long l)
                return new REPLInt(l);
            if (value is int i)
                return new REPLInt(i);

            throw new REPLException(new Position("Intern"), new Position("Intern"),
                $"{value?.GetType()} can not be cast to REPLInt");
        }
    }

    private class IntToFloatAssign : IAssignEmitter {
        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
            bool dropResult) {
            expression.EmitCode(context, false);
            context.IL.Emit(OpCodes.Conv_R8);
            if (!dropResult) context.IL.Emit(OpCodes.Dup);
            variable.EmitStore(context);
        }

        public void EmitConvert(IBlockContext context, bool mutableTarget) {
            context.IL.Emit(OpCodes.Conv_R8);
        }

        public IREPLValue EvalConvert(Node node, IREPLValue value) {
            if (value is REPLInt i) return new REPLFloat(i.intValue);
            throw new REPLException(node, $"Expected int value: {value.Type.Name}");
        }
    }
}
