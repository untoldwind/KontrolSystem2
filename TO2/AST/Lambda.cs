using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class Lambda : Expression, IVariableContainer {
    private readonly Expression expression;
    private readonly List<FunctionParameter> parameters;
    private LambdaClass? lambdaClass;

    private FunctionType? resolvedType;

    private TypeHint? typeHint;

    public Lambda(List<FunctionParameter> parameters, Expression expression, Position start = new(),
        Position end = new()) : base(start, end) {
        this.parameters = parameters;
        this.expression = expression.CollapseFinalReturn();
        this.expression.VariableContainer = this;
    }

    public override IVariableContainer? VariableContainer {
        set {
            ParentContainer = value;
            expression.VariableContainer = this;
        }
    }

    public override TypeHint? TypeHint {
        set => typeHint = value;
    }

    public IVariableContainer? ParentContainer { get; private set; }

    public TO2Type? FindVariableLocal(IBlockContext context, string name) {
        var idx = parameters.FindIndex(p => p.name == name);

        if (idx < 0 || idx >= parameters.Count) return null;

        var parameterType = parameters[idx].type;
        if (parameterType != null) return parameterType;
        if (resolvedType == null || idx >= resolvedType.parameterTypes.Count) return null;

        return resolvedType.parameterTypes[idx];
    }

    public override void Prepare(IBlockContext context) {
    }

    public override TO2Type ResultType(IBlockContext context) {
        if (resolvedType != null) return resolvedType;
        // Make an assumption ...
        if (parameters.All(p => p.type != null))
            resolvedType = new FunctionType(false, parameters.Select(p => p.type!).ToList(), BuiltinType.Unit);
        else resolvedType = typeHint?.Invoke(context) as FunctionType;
        if (resolvedType != null) {
            // ... so that it is possible to determine the return type
            var returnType = expression.ResultType(context);
            // ... so that the assumption can be replaced with the (hopefully) real thing
            resolvedType = new FunctionType(false, resolvedType.parameterTypes, returnType);
        }

        return resolvedType ?? BuiltinType.Unit;
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (ResultType(context) is not FunctionType lambdaType) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                "Unable to infer type of lambda. Please add some type hint",
                Start,
                End
            ));
            return;
        }

        if (lambdaType.parameterTypes.Count != parameters.Count)
            context.AddError(new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Expected lambda to have {lambdaType.parameterTypes.Count} parameters, found {parameters.Count}",
                Start,
                End
            ));

        for (var i = 0; i < parameters.Count; i++) {
            if (parameters[i].type == null) continue;
            if (!lambdaType.parameterTypes[i].UnderlyingType(context.ModuleContext)
                    .IsAssignableFrom(context.ModuleContext, parameters[i].type!.UnderlyingType(context.ModuleContext)))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Expected parameter {parameters[i].name} of lambda to have type {lambdaType.parameterTypes[i]}, found {parameters[i].type}",
                    Start,
                    End
                ));
        }

        if (context.HasErrors) return;

        if (dropResult) return;

        lambdaClass ??= LambdaClass.CreateLambdaClass(context, lambdaType, parameters, expression, Start, End);

        foreach (var (sourceName, _) in lambdaClass.Value.clonedVariables) {
            var source = context.FindVariable(sourceName)!;
            source.EmitLoad(context);
        }

        context.IL.EmitNew(OpCodes.Newobj, lambdaClass.Value.constructor, lambdaClass.Value.clonedVariables.Count);
        context.IL.EmitPtr(OpCodes.Ldftn, lambdaClass.Value.lambdaImpl);
        context.IL.EmitNew(OpCodes.Newobj, lambdaType.GeneratedType(context.ModuleContext)
                .GetConstructor([typeof(object), typeof(IntPtr)])!);
    }

}
