using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType {
        private class TO2SString : BuiltinType {
            private readonly OperatorCollection allowedOperators;
            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
            public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

            internal TO2SString() {
                allowedOperators = new OperatorCollection {
                    {
                        Operator.Add,
                        new StaticMethodOperatorEmitter(() => String, () => String,
                            typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)}))
                    }, {
                        Operator.AddAssign,
                        new StaticMethodOperatorEmitter(() => String, () => String,
                            typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)}))
                    }, {
                        Operator.Eq,
                        new StaticMethodOperatorEmitter(() => String, () => Bool,
                            typeof(string).GetMethod("Equals", new[] {typeof(string), typeof(string)}))
                    }, {
                        Operator.NotEq,
                        new StaticMethodOperatorEmitter(() => String, () => Bool,
                            typeof(string).GetMethod("Equals", new[] {typeof(string), typeof(string)}),
                            OpCodes.Ldc_I4_0, OpCodes.Ceq)
                    }, {
                        Operator.Gt,
                        new StaticMethodOperatorEmitter(() => String, () => Bool,
                            typeof(string).GetMethod("Compare", new[] {typeof(string), typeof(string)}),
                            OpCodes.Ldc_I4_0, OpCodes.Cgt)
                    }, {
                        Operator.Ge,
                        new StaticMethodOperatorEmitter(() => String, () => Bool,
                            typeof(string).GetMethod("Compare", new[] {typeof(string), typeof(string)}),
                            OpCodes.Ldc_I4_M1, OpCodes.Cgt)
                    }, {
                        Operator.Lt,
                        new StaticMethodOperatorEmitter(() => String, () => Bool,
                            typeof(string).GetMethod("Compare", new[] {typeof(string), typeof(string)}),
                            OpCodes.Ldc_I4_0, OpCodes.Clt)
                    }, {
                        Operator.Le,
                        new StaticMethodOperatorEmitter(() => String, () => Bool,
                            typeof(string).GetMethod("Compare", new[] {typeof(string), typeof(string)}),
                            OpCodes.Ldc_I4_1, OpCodes.Clt)
                    },
                };
                DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {
                        "repeat",
                        new BoundMethodInvokeFactory("Repeat the string `count` number of time", true,
                            () => String,
                            () => new List<RealizedParameter> {new RealizedParameter("count", BuiltinType.Int)},
                            false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringRepeat"))
                    }, {
                        "pad_left",
                        new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the left side",
                            true,
                            () => String,
                            () => new List<RealizedParameter> {new RealizedParameter("length", BuiltinType.Int)},
                            false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadLeft"))
                    }, {
                        "pad_right",
                        new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the right side",
                            true,
                            () => String,
                            () => new List<RealizedParameter> {new RealizedParameter("length", BuiltinType.Int)},
                            false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadRight"))
                    }, {
                        "contains",
                        new BoundMethodInvokeFactory("Check if the string contains a sub string `other`", true,
                            () => Bool,
                            () => new List<RealizedParameter> {new RealizedParameter("other", BuiltinType.String)},
                            false, typeof(string), typeof(string).GetMethod("Contains", new[] {typeof(string)}))
                    }, {
                        "starts_with",
                        new BoundMethodInvokeFactory("Check if the string starts with `other`", true,
                            () => Bool,
                            () => new List<RealizedParameter> {new RealizedParameter("other", BuiltinType.String)},
                            false, typeof(string), typeof(string).GetMethod("StartsWith", new[] {typeof(string)}))
                    }, {
                        "ends_with",
                        new BoundMethodInvokeFactory("Check if the string ends with `other`", true,
                            () => Bool,
                            () => new List<RealizedParameter> {new RealizedParameter("other", BuiltinType.String)},
                            false, typeof(string), typeof(string).GetMethod("EndsWith", new[] {typeof(string)}))
                    }, {
                        "to_lower",
                        new BoundMethodInvokeFactory("Convert string to lower case", true,
                            () => String,
                            () => new List<RealizedParameter>(),
                            false, typeof(string), typeof(string).GetMethod("ToLowerInvariant"))
                    }, {
                        "to_upper",
                        new BoundMethodInvokeFactory("Convert string to upper case", true,
                            () => String,
                            () => new List<RealizedParameter>(),
                            false, typeof(string), typeof(string).GetMethod("ToUpperInvariant"))
                    },
                };
                DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                    {
                        "length",
                        new BoundPropertyLikeFieldAccessFactory(
                            "Length of the string, i.e. number of characters in the string", () => Int,
                            typeof(String), typeof(String).GetProperty("Length"), OpCodes.Conv_I8)
                    },
                };
            }

            public override string Name => "string";

            public override Type GeneratedType(ModuleContext context) => typeof(string);

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedOperators;
        }
    }
}
