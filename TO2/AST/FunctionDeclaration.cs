using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public enum FunctionModifier {
    Public,
    Private,
    Test
}

public class FunctionParameter(
    string name,
    TO2Type? type,
    string? description,
    Expression? defaultValue = null,
    Position start = new(),
    Position end = new())
    : Node(start, end) {
    public readonly Expression? defaultValue = defaultValue;
    public readonly string? description = description;
    public readonly string name = name;
    public readonly TO2Type? type = type;

    public bool HasDefault => defaultValue != null;

    public override string ToString() => $"{name} : {type}";

    public override REPLValueFuture Eval(REPLContext context) {
        throw new NotSupportedException("Function are not supported in REPL mode");
    }
}

public class FunctionDeclaration : Node, IModuleItem, IVariableContainer {
    public readonly TO2Type declaredReturn;
    public readonly string description;
    private readonly Expression expression;
    public readonly bool isAsync;
    public readonly FunctionModifier modifier;
    public readonly string name;
    public readonly List<FunctionParameter> parameters;
    private AsyncClass? asyncClass;

    public FunctionDeclaration(FunctionModifier modifier, bool isAsync, string name, string description,
        List<FunctionParameter> parameters, TO2Type declaredReturn, Expression expression,
        Position start = new(), Position end = new()) : base(start, end) {
        this.modifier = modifier;
        this.name = name;
        this.description = description;
        this.isAsync = isAsync;
        this.parameters = parameters;
        this.declaredReturn = declaredReturn;
        this.expression = expression.CollapseFinalReturn();
        this.expression.VariableContainer = this;
        this.expression.TypeHint = context => this.declaredReturn.UnderlyingType(context.ModuleContext);
    }

    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
        var errors =
            parameters.Select(p => p.type).Concat([declaredReturn])
                .Where(type => !type!.IsValid(context)).Select(
                    type => new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Invalid type name '{type!.Name}'",
                        Start,
                        End
                    )).ToList();

        return errors;
    }

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => [];

    public IVariableContainer? ParentContainer => null;

    public TO2Type? FindVariableLocal(IBlockContext context, string variableName) {
        return parameters.Find(p => p.name == variableName)?.type;
    }

    public void EmitCode(IBlockContext context) {
        var valueType = expression.ResultType(context);
        if (declaredReturn != BuiltinType.Unit &&
            !declaredReturn.IsAssignableFrom(context.ModuleContext, valueType)) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Function '{name}' returns {valueType} but should return {declaredReturn}",
                Start,
                End
            ));
            return;
        }

        ILChunks.GenerateFunctionEnter(context, name, parameters, Start.sourceName, Start.line);

        if (isAsync) EmitCodeAsync(context);
        else EmitCodeSync(context);
    }

    private void EmitCodeSync(IBlockContext context) {
        expression.EmitCode(context, declaredReturn == BuiltinType.Unit);

        if (!context.HasErrors && declaredReturn != BuiltinType.Unit)
            declaredReturn.AssignFrom(context.ModuleContext, expression.ResultType(context)).EmitConvert(context, false);
        else if (declaredReturn == BuiltinType.Unit) context.IL.Emit(OpCodes.Ldnull);

        ILChunks.GenerateFunctionLeave(context);
        context.IL.EmitReturn(context.MethodBuilder!.ReturnType);
    }

    private void EmitCodeAsync(IBlockContext context) {
        asyncClass ??= AsyncClass.Create(context, name, declaredReturn, parameters, expression);

        for (var idx = 0; idx < parameters.Count; idx++)
            MethodParameter.EmitLoadArg(context.IL, idx);
        context.IL.EmitNew(OpCodes.Newobj, asyncClass.Value.constructor, parameters.Count);
        context.IL.EmitReturn(asyncClass.Value.type);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        throw new NotSupportedException("Function are not supported in REPL mode");
    }
}
