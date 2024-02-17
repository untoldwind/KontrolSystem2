using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2;

public interface IKontrolFunction {
    IKontrolModule? Module { get; }

    string Name { get; }

    string? Description { get; }

    List<RealizedParameter> Parameters { get; }

    RealizedType ReturnType { get; }

    MethodInfo? RuntimeMethod { get; }

    bool IsCompiled { get; }

    bool IsAsync { get; }

    object Invoke(IContext context, params object[] args);
}

public static class KontrolFunctionExtensions {
    public static RealizedType DelegateType(this IKontrolFunction function) {
        return new FunctionType(function.IsAsync, function.Parameters.Select(p => p.type as TO2Type).ToList(),
            function.ReturnType);
    }

    public static int RequiredParameterCount(this IKontrolFunction function) {
        return function.Parameters.Count(p => !p.HasDefault);
    }
}

public class CompiledKontrolFunction : IKontrolFunction {
    public CompiledKontrolFunction(string name, string? description, bool isAsync,
        List<RealizedParameter> parameters, RealizedType returnType, MethodInfo? runtimeMethod) {
        Name = name;
        Description = description;
        IsAsync = isAsync;
        Parameters = parameters;
        ReturnType = returnType;
        RuntimeMethod = runtimeMethod ?? throw new ArgumentException($"Method is null for {name} : {returnType}");
    }

    public IKontrolModule? Module { get; internal set; }
    public string Name { get; }
    public string? Description { get; }
    public List<RealizedParameter> Parameters { get; }
    public RealizedType ReturnType { get; }
    public MethodInfo RuntimeMethod { get; }
    public bool IsAsync { get; }

    public bool IsCompiled => true;

    public object Invoke(IContext context, params object[] args) {
        try {
            ContextHolder.CurrentContext.Value = context;
            return RuntimeMethod.Invoke(null, args);
        } catch (TargetInvocationException e) {
            throw e.InnerException ?? e;
        } finally {
            ContextHolder.CurrentContext.Value = null;
        }
    }
}

public class DeclaredKontrolFunction : IKontrolFunction {
    public readonly IBlockContext methodContext;
    private readonly DeclaredKontrolModule module;
    public readonly FunctionDeclaration to2Function;

    public DeclaredKontrolFunction(DeclaredKontrolModule module, IBlockContext methodContext,
        FunctionDeclaration to2Function) {
        this.module = module;
        Parameters = to2Function.parameters.Select(p => new RealizedParameter(methodContext, p)).ToList();
        ReturnType = to2Function.declaredReturn.UnderlyingType(methodContext.ModuleContext);
        this.methodContext = methodContext;
        this.to2Function = to2Function;
    }

    public List<RealizedParameter> Parameters { get; }
    public RealizedType ReturnType { get; }

    public IKontrolModule Module => module;

    public string Name => to2Function.name;

    public string Description => to2Function.description;

    public MethodInfo? RuntimeMethod => methodContext.MethodBuilder;

    public bool IsCompiled => false;

    public bool IsAsync => to2Function.isAsync;

    public object Invoke(IContext context, params object[] args) {
        throw new NotSupportedException($"Function {to2Function.name} not yet compiled");
    }
}

public class DeclaredKontrolStructConstructor : IKontrolFunction {
    public readonly IBlockContext methodContext;
    private readonly DeclaredKontrolModule module;
    public readonly StructDeclaration to2Struct;

    public DeclaredKontrolStructConstructor(DeclaredKontrolModule module, IBlockContext methodContext,
        StructDeclaration to2Struct) {
        this.module = module;
        Parameters = to2Struct.constructorParameters.Select(p => new RealizedParameter(methodContext, p)).ToList();
        ReturnType = to2Struct.typeDelegate!.UnderlyingType(methodContext.ModuleContext);
        this.methodContext = methodContext;
        this.to2Struct = to2Struct;
    }

    public List<RealizedParameter> Parameters { get; }
    public RealizedType ReturnType { get; }

    public IKontrolModule Module => module;

    public string Name => to2Struct.name;

    public string Description => to2Struct.description;

    public MethodInfo? RuntimeMethod => methodContext.MethodBuilder;

    public bool IsCompiled => false;

    public bool IsAsync => false;

    public object Invoke(IContext context, params object[] args) {
        throw new NotSupportedException($"Function {to2Struct.name} not yet compiled");
    }
}
