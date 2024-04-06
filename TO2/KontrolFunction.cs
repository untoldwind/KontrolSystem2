﻿using System;
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

public class KontrolFunctionSelector(IKontrolFunction? async = null, IKontrolFunction? sync = null) {
    private readonly IKontrolFunction? async = async;
    private readonly IKontrolFunction? sync = sync;

    public IKontrolFunction? PreferAsync => async ?? sync;

    public IKontrolFunction? PreferSync => sync ?? async;

    public IKontrolFunction? ForContext(IBlockContext context) => context.IsAsync ? PreferAsync : PreferSync;

    public static KontrolFunctionSelector From(IKontrolFunction function) =>
        function.IsAsync
            ? new KontrolFunctionSelector(function, null)
            : new KontrolFunctionSelector(null, function);

    public static KontrolFunctionSelector operator +(KontrolFunctionSelector a, IKontrolFunction function) {
        if (function.IsAsync) {
            if (a.async != null) throw new Exception($"Duplicate async function registration {function.Name}");
            return new KontrolFunctionSelector(function, a.sync);
        } else {
            if (a.sync != null) throw new Exception($"Duplicate sync function registration {function.Name}");
            return new KontrolFunctionSelector(a.async, function);
        }
    }
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

public class CompiledKontrolFunction(string name, string? description, bool isAsync,
    List<RealizedParameter> parameters, RealizedType returnType, MethodInfo? runtimeMethod) : IKontrolFunction {
    public IKontrolModule? Module { get; internal set; }
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public List<RealizedParameter> Parameters { get; } = parameters;
    public RealizedType ReturnType { get; } = returnType;
    public MethodInfo RuntimeMethod { get; } = runtimeMethod ?? throw new ArgumentException($"Method is null for {name} : {returnType}");

    public bool IsAsync { get; } = isAsync;

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

public class DeclaredKontrolFunction(DeclaredKontrolModule module, IBlockContext methodContext,
    FunctionDeclaration to2Function) : IKontrolFunction {
    public readonly IBlockContext methodContext = methodContext;
    private readonly DeclaredKontrolModule module = module;
    public readonly FunctionDeclaration to2Function = to2Function;

    public List<RealizedParameter> Parameters { get; } = to2Function.parameters.Select(p => new RealizedParameter(methodContext, p)).ToList();
    public RealizedType ReturnType { get; } = to2Function.declaredReturn.UnderlyingType(methodContext.ModuleContext);

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

public class DeclaredKontrolStructConstructor(DeclaredKontrolModule module, IBlockContext methodContext,
    StructDeclaration to2Struct) : IKontrolFunction {
    public readonly IBlockContext methodContext = methodContext;
    private readonly DeclaredKontrolModule module = module;
    public readonly StructDeclaration to2Struct = to2Struct;

    public List<RealizedParameter> Parameters { get; } = to2Struct.constructorParameters.Select(p => new RealizedParameter(methodContext, p)).ToList();
    public RealizedType ReturnType { get; } = to2Struct.typeDelegate!.UnderlyingType(methodContext.ModuleContext);

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
