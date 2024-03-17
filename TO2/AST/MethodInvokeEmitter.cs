using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public interface IMethodInvokeEmitter {
    RealizedType ResultType { get; }

    List<RealizedParameter> Parameters { get; }

    bool RequiresPtr { get; }

    bool IsAsync { get; }

    void EmitCode(IBlockContext context);

    REPLValueFuture Eval(Node node, IREPLValue[] targetWithArguments);
}

public static class MethodInvokeEmitterExtensions {
    public static int RequiredParameterCount(this IMethodInvokeEmitter method) {
        return method.Parameters.Count(p => !p.HasDefault);
    }
}

public interface IMethodInvokeFactory {
    bool IsAsync { get; }

    bool IsConst { get; }

    TypeHint? ReturnHint { get; }

    string? Description { get; }

    TO2Type DeclaredReturn { get; }

    List<FunctionParameter> DeclaredParameters { get; }

    TypeHint? ArgumentHint(int argumentIdx);

    IMethodInvokeEmitter? Create(IBlockContext context, List<TO2Type> arguments, Node node);

    IMethodInvokeFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);
}

public delegate REPLValueFuture REPLMethodCall(Node node, IREPLValue[] targetWithArguments);

public class InlineMethodInvokeFactory : IMethodInvokeFactory {
    private readonly REPLMethodCall methodCall;
    private readonly OpCode[] opCodes;
    private readonly Func<RealizedType> resultType;

    public InlineMethodInvokeFactory(string description, Func<RealizedType> returnType, bool isConst,
        REPLMethodCall methodCall, params OpCode[] opCodes) {
        Description = description;
        resultType = returnType;
        IsConst = isConst;
        this.methodCall = methodCall;
        this.opCodes = opCodes;
    }

    public bool IsConst { get; }
    public string Description { get; }

    public bool IsAsync => false;

    public TypeHint ReturnHint => _ => resultType();

    public TypeHint? ArgumentHint(int argumentIdx) {
        return null;
    }

    public TO2Type DeclaredReturn => resultType();

    public List<FunctionParameter> DeclaredParameters => new();

    public IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node) {
        return new InlineMethodInvokeEmitter(resultType(), new List<RealizedParameter>(), methodCall, opCodes);
    }

    public IMethodInvokeFactory
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }
}

public class InlineMethodInvokeEmitter : IMethodInvokeEmitter {
    private readonly REPLMethodCall methodCall;
    private readonly OpCode[] opCodes;

    public InlineMethodInvokeEmitter(RealizedType returnType, List<RealizedParameter> parameters,
        REPLMethodCall methodCall,
        params OpCode[] opCodes) {
        ResultType = returnType;
        Parameters = parameters;
        this.methodCall = methodCall;
        this.opCodes = opCodes;
    }

    public RealizedType ResultType { get; }

    public List<RealizedParameter> Parameters { get; }


    public bool IsAsync => false;

    public bool RequiresPtr => false;

    public void EmitCode(IBlockContext context) {
        foreach (var opCode in opCodes) context.IL.Emit(opCode);
    }

    public REPLValueFuture Eval(Node node, IREPLValue[] targetWithArguments) {
        return methodCall(node, targetWithArguments);
    }
}

public class BoundMethodInvokeFactory : IMethodInvokeFactory {
    private readonly bool constrained;
    private readonly MethodInfo methodInfo;
    private readonly Type methodTarget;
    private readonly Func<List<RealizedParameter>> parameters;
    private readonly Func<RealizedType> resultType;
    private readonly Func<ModuleContext, IEnumerable<(string name, RealizedType type)>>? targetTypeArguments;

    public BoundMethodInvokeFactory(string? description, bool isConst, Func<RealizedType> resultType,
        Func<List<RealizedParameter>> parameters, bool isAsync, Type methodTarget, MethodInfo? methodInfo,
        Func<ModuleContext, IEnumerable<(string name, RealizedType type)>>? targetTypeArguments = null,
        bool constrained = false) {
        Description = description;
        IsConst = isConst;
        this.resultType = resultType;
        this.parameters = parameters;
        this.methodInfo = methodInfo ??
                          throw new ArgumentException(
                              $"MethodInfo is null for {description} in type {methodTarget}");
        this.methodTarget = methodTarget;
        IsAsync = isAsync;
        this.targetTypeArguments = targetTypeArguments;
        this.constrained = constrained;
    }

    public bool IsConst { get; }
    public string? Description { get; }

    public bool IsAsync { get; }

    public TypeHint ReturnHint => _ => resultType();

    public TypeHint ArgumentHint(int argumentIdx) {
        var p = parameters();

        return context => argumentIdx >= 0 && argumentIdx < p.Count
            ? p[argumentIdx].type.FillGenerics(context.ModuleContext, context.InferredGenerics)
            : null;
    }

    public TO2Type DeclaredReturn => resultType();

    public List<FunctionParameter> DeclaredParameters =>
        parameters().Select(p =>
            new FunctionParameter(p.name, p.type, p.description, p.HasDefault ? new LiteralBool(true) : null)).ToList();

    public IMethodInvokeEmitter? Create(IBlockContext context, List<TO2Type> arguments, Node node) {
        var (genericMethod, genericResult, genericParameters) =
            Helpers.MakeGeneric(context,
                resultType(), parameters(), methodInfo,
                null, arguments,
                targetTypeArguments?.Invoke(context.ModuleContext) ??
                Enumerable.Empty<(string name, RealizedType type)>(),
                node);

        if (context.HasErrors) return null;

        return new BoundMethodInvokeEmitter(genericResult, genericParameters, IsAsync, methodTarget, genericMethod,
            constrained);
    }

    public IMethodInvokeFactory FillGenerics(ModuleContext context,
        Dictionary<string, RealizedType> typeArguments) {
        if (methodTarget.IsGenericTypeDefinition) {
            var arguments = methodTarget.GetGenericArguments().Select(t => {
                if (!typeArguments.ContainsKey(t.Name))
                    throw new ArgumentException($"Generic parameter {t.Name} not found");
                return typeArguments[t.Name].GeneratedType(context);
            }).ToArray();
            var genericTarget = methodTarget.MakeGenericType(arguments);
            var genericParams =
                parameters().Select(p => p.FillGenerics(context, typeArguments)).ToList();
            var genericMethod = genericTarget.GetMethod(methodInfo.Name,
                genericParams.Select(p => p.type.GeneratedType(context)).ToArray());

            if (genericMethod == null)
                throw new ArgumentException(
                    $"Unable to relocate method {methodInfo.Name} on {methodTarget} for type arguments {typeArguments}");

            return new BoundMethodInvokeFactory(Description, IsConst,
                () => resultType().FillGenerics(context, typeArguments), () => genericParams, IsAsync,
                genericTarget, genericMethod);
        }

        return this;
    }
}

public class BoundMethodInvokeEmitter : IMethodInvokeEmitter {
    private readonly bool constrained;
    private readonly MethodInfo methodInfo;
    private readonly Type methodTarget;

    public BoundMethodInvokeEmitter(RealizedType resultType, List<RealizedParameter> parameters, bool isAsync,
        Type methodTarget, MethodInfo methodInfo, bool constrained = false) {
        ResultType = resultType;
        Parameters = parameters;
        this.methodInfo = methodInfo;
        this.methodTarget = methodTarget;
        IsAsync = isAsync;
        this.constrained = constrained;
    }

    public RealizedType ResultType { get; }
    public List<RealizedParameter> Parameters { get; }
    public bool IsAsync { get; }

    public bool RequiresPtr =>
        methodTarget.IsValueType && (methodInfo.CallingConvention & CallingConventions.HasThis) != 0;

    public void EmitCode(IBlockContext context) {
        if (methodInfo.IsVirtual) {
            if (constrained) context.IL.Emit(OpCodes.Constrained, methodTarget);
            context.IL.EmitCall(OpCodes.Callvirt, methodInfo, Parameters.Count + 1);
        } else {
            context.IL.EmitCall(OpCodes.Call, methodInfo, Parameters.Count + 1);
        }

        if (methodInfo.ReturnType == typeof(void)) context.IL.Emit(OpCodes.Ldnull);
    }

    public REPLValueFuture Eval(Node node, IREPLValue[] targetWithArguments) {
        var result = methodInfo.IsStatic
            ? methodInfo.Invoke(null, targetWithArguments.Select(a => a.Value).ToArray())
            : methodInfo.Invoke(targetWithArguments[0].Value,
                targetWithArguments.Skip(1).Select(a => a.Value).ToArray());

        return IsAsync
            ? REPLValueFuture.Wrap(ResultType, (result as IAnyFuture)!)
            : REPLValueFuture.Success(ResultType.REPLCast(result));
    }
}
