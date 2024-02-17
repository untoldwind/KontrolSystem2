using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class Call : Expression {
    private readonly List<Expression> arguments;
    private readonly string functionName;
    private readonly string? moduleName;
    private ILocalRef? preparedResult;
    private TypeHint? typeHint;
    private IVariableContainer? variableContainer;

    public Call(List<string> namePath, List<Expression> arguments, Position start, Position end) :
        base(start, end) {
        if (namePath.Count > 1) {
            moduleName = string.Join("::", namePath.Take(namePath.Count - 1));
            functionName = namePath.Last();
        } else {
            moduleName = null;
            functionName = namePath.Last();
        }

        this.arguments = arguments;
        for (var i = 0; i < this.arguments.Count; i++) this.arguments[i].TypeHint = ArgumentTypeHint(i);
    }

    public override IVariableContainer? VariableContainer {
        set {
            variableContainer = value;
            foreach (var argument in arguments) argument.VariableContainer = value;
        }
    }

    public override TypeHint? TypeHint {
        set => typeHint = value;
    }

    private string FullName => moduleName != null ? $"{moduleName}::{functionName}" : functionName;

    public override TO2Type ResultType(IBlockContext context) {
        var constant = ReferencedConstant(context.ModuleContext);
        if (constant != null) return (constant.Type as FunctionType)?.returnType ?? BuiltinType.Unit;

        TO2Type? variable = ReferencedVariable(context)?.UnderlyingType(context.ModuleContext);
        if (variable != null) return (variable as FunctionType)?.returnType ?? BuiltinType.Unit;

        var function = ReferencedFunction(context.ModuleContext);
        if (function == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchFunction,
                $"Function '{FullName}' not found",
                Start,
                End
            ));
            return BuiltinType.Unit;
        }

        var (_, genericResult, _) = Helpers.MakeGeneric(context,
            function.ReturnType, function.Parameters, function.RuntimeMethod!,
            typeHint?.Invoke(context), arguments.Select(e => e.ResultType(context)),
            Enumerable.Empty<(string name, RealizedType type)>(),
            this);

        return genericResult;
    }

    public override void Prepare(IBlockContext context) {
        if (preparedResult != null) return;
        if (ReferencedVariable(context) != null) return;

        var function = ReferencedFunction(context.ModuleContext);

        if (function == null || !function.IsAsync || !context.IsAsync) return;

        EmitCodeFunction(context, false);

        if (context.HasErrors) return;

        preparedResult = context.DeclareHiddenLocal(function.ReturnType.GeneratedType(context.ModuleContext));
        preparedResult.EmitStore(context);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        if (preparedResult != null) {
            if (!dropResult) preparedResult.EmitLoad(context);
            preparedResult = null;
            return;
        }

        var constant = ReferencedConstant(context.ModuleContext);

        if (constant != null) {
            if (!(constant.Type is FunctionType)) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Constant {constant.Name} with type {constant.Type} cannot be called as a function",
                        Start,
                        End
                    )
                );
                return;
            }

            constant.EmitLoad(context);

            EmitCodeDelegate((FunctionType)constant.Type, context, dropResult);
            return;
        }

        TO2Type? variable = ReferencedVariable(context)?.UnderlyingType(context.ModuleContext);

        if (variable != null) {
            if (!(variable is FunctionType)) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Variable {functionName} with type {variable} cannot be called as a function",
                        Start,
                        End
                    )
                );
                return;
            }

            var blockVariable = context.FindVariable(functionName);

            if (blockVariable == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchVariable,
                    $"No local variable '{functionName}'",
                    Start,
                    End
                ));
                return;
            }

            blockVariable.EmitLoad(context);

            EmitCodeDelegate((FunctionType)variable, context, dropResult);
        } else {
            EmitCodeFunction(context, dropResult);
        }
    }

    private void EmitCodeDelegate(FunctionType functionType, IBlockContext context, bool dropResult) {
        if (functionType.isAsync && !context.IsAsync) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchFunction,
                $"Cannot call async function or variable '{functionName}' from a sync context",
                Start,
                End
            ));
            return;
        }

        if (functionType.parameterTypes.Count != arguments.Count) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.ArgumentMismatch,
                $"Call to variable '{functionName}' requires {functionType.parameterTypes.Count} arguments",
                Start,
                End
            ));
            return;
        }

        for (var i = 0; i < arguments.Count; i++) {
            arguments[i].EmitCode(context, false);
            if (!context.HasErrors)
                functionType.parameterTypes[i].AssignFrom(context.ModuleContext, arguments[i].ResultType(context))
                    .EmitConvert(context);
        }

        if (context.HasErrors) return;

        for (var i = 0; i < arguments.Count; i++) {
            var argumentType = arguments[i].ResultType(context);
            if (!functionType.parameterTypes[i].IsAssignableFrom(context.ModuleContext, argumentType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.ArgumentMismatch,
                    $"Argument {i + 1} of variable '{functionName}' has to be a {functionType.parameterTypes[i]}, but {argumentType} was given",
                    Start,
                    End
                ));
                return;
            }
        }

        var invokeMethod = functionType.GeneratedType(context.ModuleContext).GetMethod("Invoke") ??
                           throw new ArgumentException($"No Invoke method in generated ${functionType}");

        context.IL.EmitCall(OpCodes.Callvirt, invokeMethod, arguments.Count + 1);
        if (functionType.isAsync) context.RegisterAsyncResume(functionType.returnType);
        if (dropResult && invokeMethod.ReturnType != typeof(void)) context.IL.Emit(OpCodes.Pop);
        if (!dropResult && invokeMethod.ReturnType == typeof(void)) context.IL.Emit(OpCodes.Ldnull);
    }

    private void EmitCodeFunction(IBlockContext context, bool dropResult) {
        var function = ReferencedFunction(context.ModuleContext);

        if (function == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchFunction,
                $"Function '{FullName}' not found",
                Start,
                End
            ));
            return;
        }

        if (function.IsAsync && !context.IsAsync) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchFunction,
                $"Cannot call async function '{FullName}' from a sync context",
                Start,
                End
            ));
            return;
        }

        if (arguments.Count > function.Parameters.Count) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.ArgumentMismatch,
                $"Function '{FullName}' only allows {function.Parameters.Count} arguments, {arguments.Count} where given",
                Start,
                End
            ));
            return;
        }

        if (function.RequiredParameterCount() > arguments.Count) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.ArgumentMismatch,
                $"Function '{FullName}' requires at least {function.RequiredParameterCount()} arguments, {arguments.Count} where given",
                Start,
                End
            ));
            return;
        }

        (var genericMethod, var genericResult, var genericParameters) =
            Helpers.MakeGeneric(context,
                function.ReturnType, function.Parameters, function.RuntimeMethod!,
                typeHint?.Invoke(context), arguments.Select(e => e.ResultType(context)),
                Enumerable.Empty<(string name, RealizedType type)>(),
                this);

        int i;
        for (i = 0; i < arguments.Count; i++) {
            var argumentType = arguments[i].ResultType(context);
            if (!genericParameters[i].type.IsAssignableFrom(context.ModuleContext, argumentType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.ArgumentMismatch,
                    $"Argument {function.Parameters[i].name} of '{FullName}' has to be a {genericParameters[i].type}, but {argumentType} was given",
                    Start,
                    End
                ));
                return;
            }
        }

        foreach (var argument in arguments) argument.Prepare(context);

        if (context.HasErrors) return;

        for (i = 0; i < arguments.Count; i++) {
            arguments[i].EmitCode(context, false);
            if (!context.HasErrors)
                genericParameters[i].type.AssignFrom(context.ModuleContext, arguments[i].ResultType(context))
                    .EmitConvert(context);
        }

        if (!context.HasErrors)
            for (; i < function.Parameters.Count; i++)
                function.Parameters[i].defaultValue!.EmitCode(context);

        if (context.HasErrors) return;

        context.IL.EmitCall(OpCodes.Call, genericMethod, function.Parameters.Count);
        if (function.IsAsync) context.RegisterAsyncResume(genericResult);
        if (dropResult && genericMethod.ReturnType != typeof(void)) context.IL.Emit(OpCodes.Pop);
        if (!dropResult && genericMethod.ReturnType == typeof(void)) context.IL.Emit(OpCodes.Ldnull);
    }

    private IKontrolConstant? ReferencedConstant(ModuleContext context) {
        return moduleName != null
            ? context.FindModule(moduleName)?.FindConstant(functionName)
            : context.mappedConstants.Get(functionName);
    }

    private TO2Type? ReferencedVariable(IBlockContext context) {
        return moduleName != null ? null : variableContainer.FindVariable(context, functionName);
    }

    private IKontrolFunction? ReferencedFunction(ModuleContext context) {
        return moduleName != null
            ? context.FindModule(moduleName)?.FindFunction(functionName)
            : BuiltinFunctions.ByName.Get(functionName) ?? context.mappedFunctions.Get(functionName);
    }

    private TypeHint ArgumentTypeHint(int argumentIdx) {
        return context => {
            RealizedType returnType;
            List<RealizedType> parameterTypes;
            var variable = ReferencedVariable(context.CloneCountingContext());
            if (variable != null) {
                var functionVariable = variable as FunctionType;
                if (functionVariable == null) return null;

                returnType = functionVariable.returnType.UnderlyingType(context.ModuleContext);
                parameterTypes = functionVariable.parameterTypes
                    .Select(t => t.UnderlyingType(context.ModuleContext)).ToList();
            } else {
                var function = ReferencedFunction(context.ModuleContext);
                if (function == null) return null;

                returnType = function.ReturnType;
                parameterTypes = function.Parameters.Select(p => p.type.UnderlyingType(context.ModuleContext))
                    .ToList();
            }

            if (argumentIdx >= parameterTypes.Count) return null;

            // Try to infer as many types as possible
            var expectedReturn = typeHint?.Invoke(context);
            var inferred = new Dictionary<string, RealizedType>();
            if (expectedReturn != null && returnType.GenericParameters.Length > 0)
                foreach (var (name, type) in returnType.InferGenericArgument(context.ModuleContext, expectedReturn)
                        )
                    if (!inferred.ContainsKey(name))
                        inferred.Add(name, type);

            for (var i = 0; i < argumentIdx; i++) {
                var child = context.CreateChildContext();
                var argumentType = arguments[i].ResultType(child);
                if (child.HasErrors) continue;
                foreach (var (name, type) in parameterTypes[i].InferGenericArgument(context.ModuleContext,
                             argumentType.UnderlyingType(context.ModuleContext)))
                    if (!inferred.ContainsKey(name))
                        inferred.Add(name, type);
            }

            if (inferred.Count == 0) return parameterTypes[argumentIdx];

            return parameterTypes[argumentIdx].FillGenerics(context.ModuleContext, inferred);
        };
    }

    public override REPLValueFuture Eval(REPLContext context) {
        var argumentFutures = arguments.Select(p => p.Eval(context)).ToList();

        var function = ReferencedFunction(context.replModuleContext);

        if (function == null) throw new REPLException(this, $"Function '{FullName}' not found");

        if (function.IsAsync && !context.replBlockContext.IsAsync)
            throw new REPLException(this, $"Cannot call async function '{FullName}' from a sync context");

        if (arguments.Count > function.Parameters.Count)
            throw new REPLException(this,
                $"Function '{FullName}' only allows {function.Parameters.Count} arguments, {arguments.Count} where given");

        if (function.RequiredParameterCount() > arguments.Count)
            throw new REPLException(this,
                $"Function '{FullName}' requires at least {function.RequiredParameterCount()} arguments, {arguments.Count} where given");

        (var genericMethod, var genericResult, var genericParameters) =
            Helpers.MakeGeneric(context.replBlockContext,
                function.ReturnType, function.Parameters, function.RuntimeMethod!,
                typeHint?.Invoke(context.replBlockContext),
                arguments.Select(e => e.ResultType(context.replBlockContext)),
                Enumerable.Empty<(string name, RealizedType type)>(),
                this);

        for (var i = 0; i < arguments.Count; i++) {
            var argumentType = argumentFutures[i].Type;
            if (!genericParameters[i].type.IsAssignableFrom(context.replModuleContext, argumentType))
                throw new REPLException(this,
                    $"Argument {function.Parameters[i].name} of '{FullName}' has to be a {genericParameters[i].type}, but {argumentType} was given");
        }

        return REPLValueFuture.ChainN(genericResult, argumentFutures.ToArray(),
            arguments => {
                var result = genericMethod.Invoke(null, arguments.Select(a => a.Value).ToArray());

                return function.IsAsync
                    ? REPLValueFuture.Wrap(genericResult, (result as IAnyFuture)!)
                    : REPLValueFuture.Success(genericResult.REPLCast(result));
            });
    }
}
