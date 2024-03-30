using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract partial class BuiltinType {
    private class TO2Bool : BuiltinType {
        private readonly OperatorCollection allowedPrefixOperators;
        private readonly OperatorCollection allowedSuffixOperators;

        internal TO2Bool() {
            allowedPrefixOperators = new OperatorCollection {
                {
                    Operator.Not,
                    new DirectOperatorEmitter(() => Unit, () => Bool, REPLBool.Not, OpCodes.Ldc_I4_0,
                        OpCodes.Ceq)
                }
            };
            allowedSuffixOperators = new OperatorCollection {
                {
                    Operator.Eq,
                    new DirectOperatorEmitter(() => Bool, () => Bool, REPLBool.Eq, OpCodes.Ceq)
                }, {
                    Operator.NotEq,
                    new DirectOperatorEmitter(() => Bool, () => Bool, REPLBool.Neq, OpCodes.Ceq, OpCodes.Ldc_I4_0,
                        OpCodes.Ceq)
                }, {
                    Operator.BoolAnd,
                    new DirectOperatorEmitter(() => Bool, () => Bool, REPLBool.And, OpCodes.And)
                }, {
                    Operator.BoolOr,
                    new DirectOperatorEmitter(() => Bool, () => Bool, REPLBool.Or, OpCodes.Or)
                }
            };
            DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert boolean to string", true, () => String,
                        () => [], false, typeof(FormatUtils),
                        typeof(FormatUtils).GetMethod("BoolToString"))
                }
            };
            DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                {
                    "to_int",
                    new InlineFieldAccessFactory("Value converted to integer (false -> 0, true -> 1)",
                        () => Int, REPLBool.ToInt, OpCodes.Conv_I8)
                }, {
                    "to_float",
                    new InlineFieldAccessFactory("Value converted to float (false -> 0.0, true -> 1.0)",
                        () => Float, REPLBool.ToFloat, OpCodes.Conv_R8)
                }
            };
        }

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
        public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

        public override string Name => "bool";

        public override Type GeneratedType(ModuleContext context) => typeof(bool);

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override IREPLValue REPLCast(object? value) => new REPLBool((bool)value!);
    }
}
