﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType {
    private class TO2Float : BuiltinType {
        private readonly OperatorCollection allowedPrefixOperators;
        private readonly OperatorCollection allowedSuffixOperators;
        private readonly IAssignEmitter intToFloatAssign;

        internal TO2Float() {
            allowedPrefixOperators = new OperatorCollection {
                {
                    Operator.Neg,
                    new DirectOperatorEmitter(() => Unit, () => Float, OpCodes.Neg)
                }, {
                    Operator.Add,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Add)
                }, {
                    Operator.Sub,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Sub)
                }, {
                    Operator.Mul,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Mul)
                }, {
                    Operator.Div,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Div)
                }
            };
            allowedSuffixOperators = new OperatorCollection {
                {
                    Operator.Add,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Add)
                }, {
                    Operator.AddAssign,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Add)
                }, {
                    Operator.Sub,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Sub)
                }, {
                    Operator.SubAssign,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Sub)
                }, {
                    Operator.Mul,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Mul)
                }, {
                    Operator.MulAssign,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Mul)
                }, {
                    Operator.Div,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Div)
                }, {
                    Operator.DivAssign,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Div)
                }, {
                    Operator.Mod,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Rem)
                }, {
                    Operator.ModAssign,
                    new DirectOperatorEmitter(() => Float, () => Float, OpCodes.Rem)
                }, {
                    Operator.Eq,
                    new DirectOperatorEmitter(() => Float, () => Bool, OpCodes.Ceq)
                }, {
                    Operator.NotEq,
                    new DirectOperatorEmitter(() => Float, () => Bool,  OpCodes.Ceq,
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }, {
                    Operator.Gt,
                    new DirectOperatorEmitter(() => Float, () => Bool, OpCodes.Cgt)
                }, {
                    Operator.Lt,
                    new DirectOperatorEmitter(() => Float, () => Bool, OpCodes.Clt)
                }, {
                    Operator.Ge,
                    new DirectOperatorEmitter(() => Float, () => Bool,  OpCodes.Clt,
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }, {
                    Operator.Le,
                    new DirectOperatorEmitter(() => Float, () => Bool,  OpCodes.Cgt,
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }, {
                    Operator.Pow,
                    new StaticMethodOperatorEmitter(() => Float, () => Float, typeof(Math).GetMethod("Pow"))
                }, {
                    Operator.PowAssign,
                    new StaticMethodOperatorEmitter(() => Float, () => Float, typeof(Math).GetMethod("Pow"))
                }
            };
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert the float to string.", true, () => String,
                        () => [], false, typeof(FormatUtils),
                        typeof(FormatUtils).GetMethod("FloatToString"))
                }, {
                    "to_fixed",
                    new BoundMethodInvokeFactory("Convert the float to string with fixed number of `decimals`.",
                        true,
                        () => String,
                        () => [new("decimals", Int, "Number of decimals")],
                        false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("FloatToFixed"))
                }
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {
                    "to_int",
                    new InlineFieldAccessFactory("Value converted to int (will be truncated as necessary)",
                        () => Int, OpCodes.Conv_I8)
                }, {
                    "abs",
                    new BoundPropertyLikeFieldAccessFactory("Absolute value", () => Float, typeof(Math),
                        typeof(Math).GetMethod("Abs", [typeof(double)]), null)
                }, {
                    "sign",
                    new BoundPropertyLikeFieldAccessFactory("Sign of the value (< 0 -> -1, 0 -> 0, > 0 -> 1)",
                        () => Int, typeof(Math),
                        typeof(Math).GetMethod("Sign", [typeof(double)]), null, OpCodes.Conv_I8)
                }, {
                    "is_nan",
                    new BoundPropertyLikeFieldAccessFactory("Check if float is not a number", () => Bool,
                        typeof(double),
                        typeof(double).GetMethod("IsNaN", [typeof(double)]), null)
                }, {
                    "is_infinity",
                    new BoundPropertyLikeFieldAccessFactory("Check if float is infinity", () => Bool,
                        typeof(double),
                        typeof(double).GetMethod("IsInfinity", [typeof(double)]), null)
                }, {
                    "is_finite",
                    new BoundPropertyLikeFieldAccessFactory("Check if float is finite", () => Bool,
                        typeof(double),
                        typeof(double).GetMethod("IsFinite", [typeof(double)]) ??
                        typeof(TO2Float).GetMethod("IsFiniteWrapper", [typeof(double)]), null)
                }
            };
            intToFloatAssign = new IntToFloatAssign();
        }

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public override string Name => "float";

        public override Type GeneratedType(ModuleContext context) => typeof(double);

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) =>
            otherType == Int || otherType == Float || (otherType is BoundValueType bound && IsAssignableFrom(context, bound.elementType));

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) =>
            otherType is BoundValueType bound ?
                new BoundValueAssignEmitter(bound, AssignFrom(context, bound.elementType)) :
                otherType == Int ? intToFloatAssign : DefaultAssignEmitter.Instance;

        // Fallback if framework is (slightly) incompatible
        public static bool IsFiniteWrapper(double d) => !double.IsInfinity(d);
    }
}
