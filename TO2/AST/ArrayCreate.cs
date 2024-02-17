using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ArrayCreate : Expression {
    private TypeHint? typeHint;

    public ArrayCreate(TO2Type? elementType, List<Expression> elements, Position start = new(),
        Position end = new()) : base(start, end) {
        ElementType = elementType;
        Elements = elements;
    }

    private TO2Type? ElementType { get; }
    private List<Expression> Elements { get; }

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

        var arrayType = ResultType(context) as ArrayType;

        if (arrayType == null) {
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
            var valueType = Elements[i].ResultType(context);
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

    public override REPLValueFuture Eval(REPLContext context) {
        var expressionFutures = Elements.Select(p => p.Eval(context)).ToArray();
        var elementType = ElementType ?? expressionFutures.FirstOrDefault(e => e.Type != BuiltinType.Unit)?.Type;

        if (elementType == null)
            throw new REPLException(this, "Unable to infer type of array. Please add some type hint");

        for (var i = 0; i < expressionFutures.Length; i++)
            if (!elementType.IsAssignableFrom(context.replModuleContext, expressionFutures[i].Type))
                throw new REPLException(this,
                    $"Element {i} is of type {expressionFutures[i].Type}, expected {elementType}");

        var resultType = new ArrayType(elementType);
        var generatedElementType = elementType.GeneratedType(context.replModuleContext);

        return REPLValueFuture.ChainN(resultType, expressionFutures,
            values => {
                var destinationArray = Array.CreateInstance(generatedElementType, values.Length);
                for (var i = 0; i < values.Length; i++)
                    destinationArray.SetValue(values[i].Value, i);
                return REPLValueFuture.Success(new REPLArray(resultType, destinationArray));
            });
    }
}
