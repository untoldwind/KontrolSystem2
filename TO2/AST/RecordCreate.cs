using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class RecordCreate : Expression {
        private readonly Dictionary<string, Expression> items;
        private readonly TO2Type declaredResult;
        private RecordType resultType;

        private TypeHint typeHint;

        public RecordCreate(TO2Type declaredResult, IEnumerable<(string, Expression)> items, Position start,
            Position end) : base(start, end) {
            this.declaredResult = declaredResult;
            this.items = items.ToDictionary(kv => kv.Item1, kv => kv.Item2);
        }

        public override IVariableContainer VariableContainer {
            set {
                foreach (var kv in items) kv.Value.VariableContainer = value;
            }
        }

        public override TypeHint TypeHint {
            set {
                typeHint = value;
                foreach (var kv in items) {
                    string itemName = kv.Key;
                    kv.Value.TypeHint = context => {
                        RecordType expectedRecord = declaredResult as RecordType ??
                                                    typeHint?.Invoke(context) as RecordType ??
                                                    (typeHint?.Invoke(context) as ResultType)
                                                    ?.successType as RecordType;
                        SortedDictionary<string, TO2Type> itemTypes = expectedRecord?.ItemTypes;

                        return itemTypes.Get(itemName)?.UnderlyingType(context.ModuleContext);
                    };
                }
            }
        }

        public override TO2Type ResultType(IBlockContext context) => DeriveType(context);

        public override void Prepare(IBlockContext context) {
            foreach (Expression item in items.Values) item.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (dropResult) return;

            RecordType recordHint = ResultType(context) as RecordType;

            using ITempBlockVariable tempVariable = context.MakeTempVariable(recordHint ?? DeriveType(context));
            EmitStore(context, tempVariable, false);
        }

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            RecordType recordType = ResultType(context) as RecordType;

            if (recordType == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"{variable.Type} is not a record",
                    Start,
                    End
                ));
                return;
            }

            foreach (var kv in items) {
                if (!recordType.ItemTypes.ContainsKey(kv.Key))
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"{recordType} does not have a field {kv.Key}",
                        Start,
                        End
                    ));
                else {
                    TO2Type valueType = kv.Value.ResultType(context);
                    if (!recordType.ItemTypes[kv.Key].IsAssignableFrom(context.ModuleContext, valueType)) {
                        context.AddError(new StructuralError(
                            StructuralError.ErrorType.IncompatibleTypes,
                            $"Expected item {kv.Key} of {recordType} to be a {recordType.ItemTypes[kv.Key]}, found {valueType}",
                            Start,
                            End
                        ));
                    }
                }
            }

            foreach (string name in recordType.ItemTypes.Keys)
                if (!items.ContainsKey(name))
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"Missing {name} for of {recordType}",
                        Start,
                        End
                    ));

            if (context.HasErrors) return;

            foreach (Expression item in items.Values) {
                item.Prepare(context);
            }

            Type type = recordType.GeneratedType(context.ModuleContext);

            switch (recordType) {
            case RecordStructType recordStruct:
                if (recordStruct.runtimeType.IsValueType) variable.EmitLoadPtr(context);
                else variable.EmitLoad(context);

                foreach (var kv in recordStruct.fields) {
                    context.IL.Emit(OpCodes.Dup);
                    items[kv.Key].EmitCode(context, false);
                    recordType.ItemTypes[kv.Key].AssignFrom(context.ModuleContext, items[kv.Key].ResultType(context))
                        .EmitConvert(context);
                    context.IL.Emit(OpCodes.Stfld, kv.Value);
                }

                context.IL.Emit(OpCodes.Pop);
                break;
            default:
                variable.EmitLoadPtr(context);

                int i = 0;
                foreach (var kv in recordType.ItemTypes) {
                    if (i > 0 && i % 7 == 0) {
                        context.IL.Emit(OpCodes.Ldflda, type.GetField("Rest"));
                        type = type.GetGenericArguments()[7];
                    }

                    if (i < items.Count - 1) context.IL.Emit(OpCodes.Dup);
                    items[kv.Key].EmitCode(context, false);
                    recordType.ItemTypes[kv.Key].AssignFrom(context.ModuleContext, items[kv.Key].ResultType(context))
                        .EmitConvert(context);
                    context.IL.Emit(OpCodes.Stfld, type.GetField($"Item{i % 7 + 1}"));
                    i++;
                }

                break;
            }

            if (context.HasErrors) return;

            if (!dropResult) variable.EmitLoad(context);
        }

        private RecordType DeriveType(IBlockContext context) {
            if (declaredResult != null) {
                resultType = declaredResult.UnderlyingType(context.ModuleContext) as RecordType;
                if (resultType == null) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"{declaredResult} is not a record type",
                        Start,
                        End
                    ));
                }
            }

            if (resultType == null) {
                RecordType hinted = typeHint?.Invoke(context) as RecordType;
                resultType = new RecordTupleType(items.Select(item =>
                    (item.Key, hinted?.ItemTypes.Get(item.Key) ?? item.Value.ResultType(context))));
            }

            return resultType;
        }
    }
}
