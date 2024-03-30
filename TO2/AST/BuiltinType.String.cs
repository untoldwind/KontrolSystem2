using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType {
    private class TO2SString : BuiltinType {
        private readonly OperatorCollection allowedOperators;
        private readonly IAssignEmitter errorToStringAssign;

        internal TO2SString() {
#pragma warning disable IDE0300 // Simplify collection initialization
            allowedOperators = new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => String, () => String,
                        typeof(string).GetMethod("Concat", [typeof(string), typeof(string)]))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => String, () => String,
                        typeof(string).GetMethod("Concat", [typeof(string), typeof(string)]))
                }, {
                    Operator.Eq,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Equals", [typeof(string), typeof(string)]))
                }, {
                    Operator.NotEq,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Equals", [typeof(string), typeof(string)]),
                        null, OpCodes.Ldc_I4_0, OpCodes.Ceq)
                }, {
                    Operator.Gt,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", [typeof(string), typeof(string)]),
                        null, OpCodes.Ldc_I4_0, OpCodes.Cgt)
                }, {
                    Operator.Ge,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", [typeof(string), typeof(string)]),
                        null, OpCodes.Ldc_I4_M1, OpCodes.Cgt)
                }, {
                    Operator.Lt,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", [typeof(string), typeof(string)]),
                        null, OpCodes.Ldc_I4_0, OpCodes.Clt)
                }, {
                    Operator.Le,
                    new StaticMethodOperatorEmitter(() => String, () => Bool,
                        typeof(string).GetMethod("Compare", [typeof(string), typeof(string)]),
                        null, OpCodes.Ldc_I4_1, OpCodes.Clt)
                }
            };
#pragma warning restore IDE0300 // Simplify collection initialization
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {
                    "repeat",
                    new BoundMethodInvokeFactory("Repeat the string `count` number of time", true,
                        () => String,
                        () => [new("count", Int, "Number of times string should be repeated.")],
                        false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringRepeat"))
                }, {
                    "pad_left",
                    new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the left side",
                        true,
                        () => String,
                        () => [new("length", Int, "Desired length of the string")],
                        false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadLeft"))
                }, {
                    "pad_right",
                    new BoundMethodInvokeFactory("Pad the string to `length` by filling spaces from the right side",
                        true,
                        () => String,
                        () => [new("length", Int, "Desired length of the string")],
                        false, typeof(FormatUtils), typeof(FormatUtils).GetMethod("StringPadRight"))
                }, {
                    "contains",
                    new BoundMethodInvokeFactory("Check if the string contains a sub string `other`", true,
                        () => Bool,
                        () => [new("other", String, "Search string")],
                        false, typeof(string), typeof(string).GetMethod("Contains", [typeof(string)]))
                }, {
                    "starts_with",
                    new BoundMethodInvokeFactory("Check if the string starts with `other`", true,
                        () => Bool,
                        () => [new("other", String, "Search string")],
                        false, typeof(string), typeof(string).GetMethod("StartsWith", [typeof(string)]))
                }, {
                    "ends_with",
                    new BoundMethodInvokeFactory("Check if the string ends with `other`", true,
                        () => Bool,
                        () => [new("other", String, "Search string")],
                        false, typeof(string), typeof(string).GetMethod("EndsWith", [typeof(string)]))
                }, {
                    "to_lower",
                    new BoundMethodInvokeFactory("Convert string to lower case", true,
                        () => String,
                        () => [],
                        false, typeof(string), typeof(string).GetMethod("ToLowerInvariant"))
                }, {
                    "to_upper",
                    new BoundMethodInvokeFactory("Convert string to upper case", true,
                        () => String,
                        () => [],
                        false, typeof(string), typeof(string).GetMethod("ToUpperInvariant"))
                }, {
                    "index_of",
                    new BoundMethodInvokeFactory("Find index of a sub string, will return -1 if not found", true,
                        () => Int,
                        () => [
                            new("other", String, "Search string"),
                            new("startIndex", Int, "Start index", new IntDefaultValue(0))
                        ],
                        false, typeof(StringMethods), typeof(StringMethods).GetMethod("IndexOf"))
                }, {
                    "slice",
                    new BoundMethodInvokeFactory("Get a sub string/slice for the string defined by start and end index",
                        true,
                        () => String,
                        () => [
                            new("startIndex", Int, "Start index of the slice (inclusive)"),
                            new("endIndex", Int, "End index of the slice (exclusive)", new IntDefaultValue(-1))
                        ],
                        false, typeof(StringMethods), typeof(StringMethods).GetMethod("Slice"))
                }, {
                    "replace",
                    new BoundMethodInvokeFactory("Replace sub string with another sub string", true,
                        () => String,
                        () => [
                            new("oldString", String, "Search string"),
                            new("newString", String, "Replacement")
                        ],
                        false, typeof(string),
                        typeof(string).GetMethod("Replace", [typeof(string), typeof(string)]))
                }, {
                    "split",
                    new BoundMethodInvokeFactory("Split string into substrings by separator", true,
                        () => new ArrayType(String),
                        () => [new("separator", String, "Search string (separator)")],
                        false, typeof(StringMethods), typeof(StringMethods).GetMethod("Split"))
                }, {
                    "ellipsis",
                    new BoundMethodInvokeFactory("Truncate string with ellipsis if necessary", true,
                        () => String,
                        () => [new("max_length", Int, "Maximum length of result string")],
                        false, typeof(StringMethods), typeof(StringMethods).GetMethod("Ellipsis"))
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
            errorToStringAssign = new ErrorToStringAssign();
        }

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public override string Name => "string";

        public override Type GeneratedType(ModuleContext context) => typeof(string);

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedOperators;

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) =>
            otherType == String || otherType.GeneratedType(context) == typeof(CoreError.Error);

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) =>
            otherType.GeneratedType(context) == typeof(CoreError.Error)
                ? errorToStringAssign
                : DefaultAssignEmitter.Instance;

        public override IREPLValue REPLCast(object? value) => new REPLString((string)value!);
    }

    private class ErrorToStringAssign : IAssignEmitter {
        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
            bool dropResult) {
            expression.EmitCode(context, false);
            context.IL.Emit(OpCodes.Ldfld, typeof(CoreError.Error).GetField("message"));
            if (!dropResult) context.IL.Emit(OpCodes.Dup);
            variable.EmitStore(context);
        }

        public void EmitConvert(IBlockContext context, bool mutableTarget) {
            context.IL.Emit(OpCodes.Ldfld, typeof(CoreError.Error).GetField("message"));
        }

        public IREPLValue EvalConvert(Node node, IREPLValue value) {
            if (value is REPLAny a) return new REPLString(a.ToString());
            throw new REPLException(node, $"Expected error value: {value.Type.Name}");
        }
    }
}
