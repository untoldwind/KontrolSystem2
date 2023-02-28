using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType {
        private class TO2Bool : BuiltinType {
            private readonly OperatorCollection allowedPrefixOperators;
            private readonly OperatorCollection allowedSuffixOperators;
            public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }
            public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

            internal TO2Bool() {
                allowedPrefixOperators = new OperatorCollection {
                    {
                        Operator.Not,
                        new DirectOperatorEmitter(() => Unit, () => Bool, OpCodes.Ldc_I4_0,
                            OpCodes.Ceq)
                    }
                };
                allowedSuffixOperators = new OperatorCollection {
                    {
                        Operator.Eq,
                        new DirectOperatorEmitter(() => Bool, () => Bool, OpCodes.Ceq)
                    }, {
                        Operator.NotEq,
                        new DirectOperatorEmitter(() => Bool, () => Bool, OpCodes.Ldc_I4_0,
                            OpCodes.Ceq)
                    }, {
                        Operator.BoolAnd,
                        new DirectOperatorEmitter(() => Bool, () => Bool, OpCodes.And)
                    }, {
                        Operator.BoolOr,
                        new DirectOperatorEmitter(() => Bool, () => Bool, OpCodes.Or)
                    }
                };
                DeclaredMethods = new Dictionary<string, IMethodInvokeFactory> {
                    {
                        "to_string",
                        new BoundMethodInvokeFactory("Convert boolean to string", true, () => String,
                            () => new List<RealizedParameter>(), false, typeof(FormatUtils),
                            typeof(FormatUtils).GetMethod("BoolToString"))
                    }
                };
                DeclaredFields = new Dictionary<string, IFieldAccessFactory> {
                    {
                        "to_int",
                        new InlineFieldAccessFactory("Value converted to integer (false -> 0, true -> 1)",
                            () => Int, OpCodes.Conv_I8)
                    }, {
                        "to_float",
                        new InlineFieldAccessFactory("Value converted to float (false -> 0.0, true -> 1.0)",
                            () => Float, OpCodes.Conv_R8)
                    },
                };
            }

            public override string Name => "bool";
            public override Type GeneratedType(ModuleContext context) => typeof(bool);

            public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

            public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;
        }
    }
}
