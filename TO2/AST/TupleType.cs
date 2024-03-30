using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class TupleType : RealizedType {
    public readonly List<TO2Type> itemTypes;
    private Type? generatedType;

    public TupleType(List<TO2Type> itemTypes) {
        this.itemTypes = itemTypes;
        DeclaredFields = this.itemTypes
            .Select((_, idx) => ($"_{idx + 1}",
                new TupleFieldAccessFactory(this, this.itemTypes, idx) as IFieldAccessFactory))
            .ToDictionary(item => item.Item1, item => item.Item2);
    }

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields { get; }

    public override string Name => $"({string.Join(", ", itemTypes)})";

    public override bool IsValid(ModuleContext context) =>
        itemTypes.Count > 0 && itemTypes.All(t => t.IsValid(context));

    public override RealizedType UnderlyingType(ModuleContext context) =>
        new TupleType(itemTypes.Select(p => p.UnderlyingType(context) as TO2Type).ToList());

    public override Type GeneratedType(ModuleContext context) =>
        generatedType ??=
            DeriveTupleType(itemTypes.Select(t => t.GeneratedType(context)).ToList());

    public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
        if (!(otherType.UnderlyingType(context) is TupleType)) return false;
        return GeneratedType(context).IsAssignableFrom(otherType.GeneratedType(context));
    }

    internal static Type DeriveTupleType(List<Type> itemTypes) {
        if (itemTypes.Count > 7) {
            var rest = DeriveTupleType(itemTypes.Skip(7).ToList());
            return Type.GetType("System.ValueTuple`8")!.MakeGenericType(
                itemTypes.Take(7).Concat(rest.Yield()).ToArray());
        }

        return Type.GetType($"System.ValueTuple`{itemTypes.Count}")!.MakeGenericType([.. itemTypes]);
    }
}

internal class TupleFieldAccessFactory : IFieldAccessFactory {
    private readonly int index;
    private readonly List<TO2Type> itemTypes;
    private readonly RealizedType tupleType;

    internal TupleFieldAccessFactory(RealizedType tupleType, List<TO2Type> itemTypes, int index) {
        this.tupleType = tupleType;
        this.itemTypes = itemTypes;
        this.index = index;
    }

    public TO2Type DeclaredType => itemTypes[index];

    public string Description => itemTypes[index].Description;

    public bool CanStore => true;

    public IFieldAccessEmitter Create(ModuleContext context) {
        var generateType = tupleType.GeneratedType(context);
        var fieldInfos = new List<FieldInfo>();
        var currentType = generateType;
        var currentIdx = index;

        while (currentIdx >= 7) {
            currentIdx -= 7;
            var rest = currentType.GetField("Rest");
            fieldInfos.Add(rest);
            currentType = rest.FieldType;
        }

        fieldInfos.Add(currentType.GetField($"Item{currentIdx + 1}"));

        return new BoundFieldAccessEmitter(itemTypes[index].UnderlyingType(context), generateType, fieldInfos);
    }

    public IFieldAccessFactory
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
}
