﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class MethodDeclaration : Node, IVariableContainer {
    private readonly TO2Type declaredReturn;
    private readonly string description;
    private readonly Expression expression;
    private readonly bool isAsync;

    public readonly string name;

    //        private readonly bool isConst;
    private readonly List<FunctionParameter> parameters;
    private AsyncClass? asyncClass;
    private BoundMethodInvokeFactory? invokeFactory;
    private StructTypeAliasDelegate? structType;
    private SyncBlockContext? syncBlockContext;

    public MethodDeclaration(bool isAsync, string name, string description,
        List<FunctionParameter> parameters,
        TO2Type declaredReturn, Expression expression, Position start = new(),
        Position end = new()) : base(start, end) {
        this.name = name;
        this.description = description;
        this.isAsync = isAsync;
        this.parameters = parameters;
        this.declaredReturn = declaredReturn;
        this.expression = expression.CollapseFinalReturn();
        this.expression.VariableContainer = this;
        this.expression.TypeHint = context => this.declaredReturn.UnderlyingType(context.ModuleContext);
    }

    public StructTypeAliasDelegate StructType {
        set => structType = value;
    }

    public IVariableContainer? ParentContainer => null;

    public TO2Type? FindVariableLocal(IBlockContext context, string variableName) {
        return variableName == "self" ? structType : parameters.Find(p => p.name == variableName)?.type;
    }

    public IMethodInvokeFactory CreateInvokeFactory() {
        syncBlockContext ??= new SyncBlockContext(structType!, isAsync, structType!.Name.ToUpper() + "_" + name,
            declaredReturn, parameters);

        if (invokeFactory != null) return invokeFactory;

        var realizedParameters = parameters.Select(p => new RealizedParameter(syncBlockContext, p)).ToList();

        invokeFactory = new BoundMethodInvokeFactory(
            description,
            true,
            () => declaredReturn.UnderlyingType(structType!.structContext),
            () => realizedParameters,
            isAsync,
            structType!.structContext.typeBuilder,
            syncBlockContext.MethodBuilder
        );

        return invokeFactory;
    }

    public IEnumerable<StructuralError> EmitCode() {
        var valueType = expression.ResultType(syncBlockContext!);
        if (declaredReturn != BuiltinType.Unit &&
            !declaredReturn.IsAssignableFrom(syncBlockContext!.ModuleContext, valueType)) {
            syncBlockContext.AddError(new StructuralError(
                StructuralError.ErrorType.IncompatibleTypes,
                $"Function '{name}' returns {valueType} but should return {declaredReturn}",
                Start,
                End
            ));
            return syncBlockContext.AllErrors;
        }

        var effectiveParameters =
            new List<FunctionParameter> { new("self", structType!, "Reference to self") };
        effectiveParameters.AddRange(parameters);

        ILChunks.GenerateFunctionEnter(syncBlockContext!, structType!.Name + "." + name, effectiveParameters, Start.sourceName, Start.line);

        if (isAsync) {
            asyncClass ??= AsyncClass.Create(syncBlockContext!, structType.Name.ToUpper() + "_" + name, declaredReturn,
                effectiveParameters,
                expression);

            for (var idx = 0; idx < effectiveParameters.Count; idx++)
                MethodParameter.EmitLoadArg(syncBlockContext!.IL, idx);
            syncBlockContext!.IL.EmitNew(OpCodes.Newobj, asyncClass.Value.constructor, effectiveParameters.Count);
            syncBlockContext.IL.EmitReturn(asyncClass.Value.type);

            return [];
        }

        expression.EmitCode(syncBlockContext!, declaredReturn == BuiltinType.Unit);

        if (!syncBlockContext!.HasErrors && declaredReturn != BuiltinType.Unit)
            declaredReturn.AssignFrom(syncBlockContext.ModuleContext, expression.ResultType(syncBlockContext))
                .EmitConvert(syncBlockContext, false);
        else if (declaredReturn == BuiltinType.Unit) syncBlockContext.IL.Emit(OpCodes.Ldnull);

        ILChunks.GenerateFunctionLeave(syncBlockContext);
        syncBlockContext.IL.EmitReturn(syncBlockContext.MethodBuilder!.ReturnType);

        return syncBlockContext.AllErrors;
    }
}
