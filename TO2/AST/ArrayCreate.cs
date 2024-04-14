using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ArrayCreate(TO2Type? elementType, List<Expression> elements, Position start = new(),
    Position end = new()) : Expression(start, end) {
    private TypeHint? typeHint;

    private TO2Type? ElementType { get; } = elementType;
    private List<Expression> Elements { get; } = elements;

    public override IVariableContainer? VariableContainer {
        set {
            foreach (var element in Elements) element.VariableContainer = value;
        }
    }

    public override TypeHint? TypeHint {
        set {
            typeHint = value;
            foreach (var element in Elements)
                if (ElementType != null)
                    element.TypeHint = context => ElementType.UnderlyingType(context.ModuleContext);
                else
                    element.TypeHint = context =>
                        (typeHint?.Invoke(context) as ArrayType)?.ElementType.UnderlyingType(context.ModuleContext);
        }
    }

    public override TO2Type ResultType(IBlockContext context) {
        if (ElementType != null) return new ArrayType(ElementType);
        foreach (var element in Elements) {
            var valueType = element.ResultType(context);
            if (valueType != BuiltinType.Unit) return new ArrayType(valueType);
        }

        var arrayHint = typeHint?.Invoke(context) as ArrayType;

        return arrayHint ?? BuiltinType.Unit;
    }

    public override void Prepare(IBlockContext context) {
        foreach (var element in Elements) element.Prepare(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (dropResult) return;

        if (ResultType(context) is not ArrayType arrayType) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                "Unable to infer type of array. Please add some type hint",
                Start,
                End
            ));
            return;
        }

        var elementType = arrayType.ElementType.UnderlyingType(context.ModuleContext);

        context.IL.Emit(OpCodes.Ldc_I4, Elements.Count);
        context.IL.Emit(OpCodes.Newarr, elementType.GeneratedType(context.ModuleContext));

        for (var i = 0; i < Elements.Count; i++) {
            var valueType = Elements[i].ResultType(context).UnderlyingType(context.ModuleContext);
            if (!elementType.IsAssignableFrom(context.ModuleContext, valueType))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Element {i} is of type {valueType}, expected {elementType}",
                    Elements[i].Start,
                    Elements[i].End
                ));
        }

        if (context.HasErrors) return;

        for (var i = 0; i < Elements.Count; i++) {
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4, i);
            Elements[i].EmitCode(context, false);
            if (elementType == BuiltinType.Bool) context.IL.Emit(OpCodes.Stelem_I4);
            else if (elementType == BuiltinType.Int) context.IL.Emit(OpCodes.Stelem_I8);
            else if (elementType == BuiltinType.Float) context.IL.Emit(OpCodes.Stelem_R8);
            else context.IL.Emit(OpCodes.Stelem, elementType.GeneratedType(context.ModuleContext));
        }
    }
}
