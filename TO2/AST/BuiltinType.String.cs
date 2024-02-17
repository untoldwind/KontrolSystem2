using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType {
    private class TO2SString : BuiltinType {
        private readonly OperatorCollection allowedOperators;

        internal TO2SString() {
            allowedOperators = new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => String, () => String,
                        typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => String, () => String,
                        typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }))
                }, {
                    Operator.Eq,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) }))
                }, {
                    Operator.NotEq,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) }),
                        null, OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }, {
                    Operator.Gt,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) }),
                        null, OpCodes.Ldc_I4_0, OpCodes.Cgt)
                }, {
                    Operator.Ge,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) }),
                        null, OpCodes.Ldc_I4_M1, OpCodes.Cgt)
                }, {
                    Operator.Lt,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) }),
                        null, OpCodes.Ldc_I4_0, OpCodes.Clt)
                }, {
                    Operator.Le,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) }),
                        null, OpCodes.Ldc_I4_1, OpCodes.Clt)
                }
            };
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {
                    "repeat",
                    new BoundMethodInvokeFactory("Repeat the string `count` number of time", true,
                        () => String,
                        () => new List<RealizedParameter> {
                            new("count", Int, "Number of times string should be repeated.")
                        },
                        false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringRepeat"))
                }, {
                    "pad_left",
                    new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the left side",
                        true,
                        () => String,
                        () => new List<RealizedParameter> {
                            new("length", Int, "Desired length of the string")
                        },
                        false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadLeft"))
                }, {
                    "pad_right",
                    new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the right side",
                        true,
                        () => String,
                        () => new List<RealizedParameter> {
                            new("length", Int, "Desired length of the string")
                        },
                        false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadRight"))
                }, {
                    "contains",
                    new BoundMethodInvokeFactory("Check if the string contains a sub string `other`", true,
                        () => Bool,
                        () => new List<RealizedParameter> {
                            new("other", String, "Search string")
                        },
                        false, typeof(string), typeof(string).GetMethod("Contains", new[] { typeof(string) }))
                }, {
                    "starts_with",
                    new BoundMethodInvokeFactory("Check if the string starts with `other`", true,
                        () => Bool,
                        () => new List<RealizedParameter> {
                            new("other", String, "Search string")
                        },
                        false, typeof(string), typeof(string).GetMethod("StartsWith", new[] { typeof(string) }))
                }, {
                    "ends_with",
                    new BoundMethodInvokeFactory("Check if the string ends with `other`", true,
                        () => Bool,
                        () => new List<RealizedParameter> {
                            new("other", String, "Search string")
                        },
                        false, typeof(string), typeof(string).GetMethod("EndsWith", new[] { typeof(string) }))
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
                }, {
                    "index_of",
                    new BoundMethodInvokeFactory("Find index of a sub string, will return -1 if not found", true,
                        () => Int,
                        () => new List<RealizedParameter> {
                            new("other", String, "Search string"),
                            new("startIndex", Int, "Start index", new IntDefaultValue(0))
                        },
                        false, typeof(StringMethods), typeof(StringMethods).GetMethod("IndexOf"))
                }, {
                    "slice",
                    new BoundMethodInvokeFactory("Get a sub string/slice for the string defined by start and end index",
                        true,
                        () => String,
                        () => new List<RealizedParameter> {
                            new("startIndex", Int, "Start index of the slice (inclusive)"),
                            new("endIndex", Int, "End index of the slice (exclusive)", new IntDefaultValue(-1))
                        },
                        false, typeof(StringMethods), typeof(StringMethods).GetMethod("Slice"))
                }, {
                    "replace",
                    new BoundMethodInvokeFactory("Replace sub string with another sub string", true,
                        () => String,
                        () => new List<RealizedParameter> {
                            new("oldString", String, "Search string"),
                            new("newString", String, "Replacement")
                        },
                        false, typeof(string),
                        typeof(string).GetMethod("Replace", new[] { typeof(string), typeof(string) }))
                }, {
                    "split",
                    new BoundMethodInvokeFactory("Split string into substrings by separator", true,
                        () => new ArrayType(String),
                        () => new List<RealizedParameter> {
                            new("separator", String, "Search string (separator)")
                        },
                        false, typeof(StringMethods), typeof(StringMethods).GetMethod("Split"))
                }
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {
                    "length",
                    new BoundPropertyLikeFieldAccessFactory(
                        "Length of the string, i.e. number of characters in the string", () => Int,
                        typeof(string), typeof(string).GetProperty("Length"), OpCodes.Conv_I8)
                }
            };
        }

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public override string Name => "string";

        public override Type GeneratedType(ModuleContext context) {
            return typeof(string);
        }

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) {
            return allowedOperators;
        }

        public override IREPLValue REPLCast(object? value) {
            return new REPLString((string)value!);
        }
    }
}
