using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class RecordTupleType : RecordType {
    private readonly SortedDictionary<string, TO2Type> itemTypes;
    private Type? generatedType;

    public RecordTupleType(IEnumerable<(string name, TO2Type type)> itemTypes) : base(BuiltinType.NoOperators) {
        this.itemTypes = [];
        foreach (var (name, type) in itemTypes) this.itemTypes.Add(name, type);
        DeclaredFields = this.itemTypes.Keys
            .Select((name, idx) => (name,
                new TupleFieldAccessFactory(this, [.. this.itemTypes.Values], idx) as IFieldAccessFactory))
            .ToDictionary(item => item.name, item => item.Item2);
    }

    public override SortedDictionary<string, TO2Type> ItemTypes => itemTypes;

    public override string Name => $"({string.Join(", ", itemTypes.Select(kv => $"{kv.Key} : {kv.Value}"))})";

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

    public override bool IsValid(ModuleContext context) => itemTypes.Count > 0 && itemTypes.Values.All(t => t.IsValid(context));

    public override RealizedType UnderlyingType(ModuleContext context) => new RecordTupleType(itemTypes.Select(kv => (kv.Key, kv.Value.UnderlyingType(context) as TO2Type)));

    public override Type GeneratedType(ModuleContext context) =>
        generatedType ??=
            TupleType.DeriveTupleType(itemTypes.Values.Select(t => t.GeneratedType(context)).ToList());

    public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
        var newGeneratedType = GeneratedType(context);
        var generatedOther = otherType.GeneratedType(context);

        return otherType is RecordType otherRecordType && newGeneratedType != generatedOther
            ? new AssignRecordTuple(this, otherRecordType)
            : DefaultAssignEmitter.Instance;
    }

    internal override IOperatorEmitter CombineFrom(RecordType otherType) => new AssignRecordTuple(this, otherType);
}

internal class AssignRecordTuple : RecordTypeAssignEmitter<RecordTupleType> {
    internal AssignRecordTuple(RecordTupleType targetType, RecordType sourceType) : base(targetType, sourceType) {
    }

    protected override void EmitAssignToPtr(IBlockContext context, IBlockVariable tempSource) {
        var type = targetType.GeneratedType(context.ModuleContext);

        var i = 0;
        foreach (var kv in targetType.ItemTypes) {
            if (i > 0 && i % 7 == 0) {
                context.IL.Emit(OpCodes.Ldflda, type.GetField("Rest"));
                type = type.GetGenericArguments()[7];
            }

            var sourceFieldFactory = sourceType.FindField(context.ModuleContext, kv.Key);
            if (sourceFieldFactory != null) {
                var sourceField = sourceFieldFactory.Create(context.ModuleContext);

                context.IL.Emit(OpCodes.Dup);
                if (sourceField.RequiresPtr) tempSource.EmitLoadPtr(context);
                else tempSource.EmitLoad(context);
                sourceField.EmitLoad(context);
                targetType.ItemTypes[kv.Key].AssignFrom(context.ModuleContext, sourceType.ItemTypes[kv.Key])
                    .EmitConvert(context, !tempSource.IsConst);
                context.IL.Emit(OpCodes.Stfld, type.GetField($"Item{i % 7 + 1}"));
            }

            i++;
        }

        context.IL.Emit(OpCodes.Pop);
    }
}
