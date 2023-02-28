using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2 {
    public readonly struct RealizedParameter {
        public readonly string name;
        public readonly RealizedType type;
        public readonly IDefaultValue defaultValue;

        public RealizedParameter(string name, RealizedType type, IDefaultValue defaultValue = null) {
            this.name = name;
            this.type = type;
            this.defaultValue = defaultValue;
        }

        public RealizedParameter(IBlockContext context, FunctionParameter parameter) {
            name = parameter.name;
            type = parameter.type.UnderlyingType(context.ModuleContext);
            defaultValue = DefaultValue.ForParameter(context, parameter);
        }

        public bool HasDefault => defaultValue != null;

        public RealizedParameter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            new RealizedParameter(name, type.FillGenerics(context, typeArguments), defaultValue);
    }

    public interface IKontrolFunction {
        IKontrolModule Module { get; }

        string Name { get; }

        string Description { get; }

        List<RealizedParameter> Parameters { get; }

        RealizedType ReturnType { get; }

        MethodInfo RuntimeMethod { get; }

        bool IsCompiled { get; }

        bool IsAsync { get; }

        object Invoke(IContext context, params object[] args);
    }

    public static class KontrolFunctionExtensions {
        public static RealizedType DelegateType(this IKontrolFunction function) {
            return new FunctionType(function.IsAsync, function.Parameters.Select(p => p.type as TO2Type).ToList(),
                function.ReturnType);
        }

        public static int RequiredParameterCount(this IKontrolFunction function) =>
            function.Parameters.Count(p => !p.HasDefault);
    }

    public class CompiledKontrolFunction : IKontrolFunction {
        public IKontrolModule Module { get; internal set; }
        public string Name { get; }
        public string Description { get; }
        public List<RealizedParameter> Parameters { get; }
        public RealizedType ReturnType { get; }
        public MethodInfo RuntimeMethod { get; }
        public bool IsAsync { get; }

        public CompiledKontrolFunction(string name, string description, bool isAsync,
            List<RealizedParameter> parameters, RealizedType returnType, MethodInfo runtimeMethod) {
            Name = name;
            Description = description;
            IsAsync = isAsync;
            Parameters = parameters;
            ReturnType = returnType;
            RuntimeMethod = runtimeMethod;
        }

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
        private readonly DeclaredKontrolModule module;
        public List<RealizedParameter> Parameters { get; }
        public RealizedType ReturnType { get; }
        public readonly IBlockContext methodContext;
        public readonly FunctionDeclaration to2Function;

        public DeclaredKontrolFunction(DeclaredKontrolModule module, IBlockContext methodContext,
            FunctionDeclaration to2Function) {
            this.module = module;
            Parameters = to2Function.parameters.Select(p => new RealizedParameter(methodContext, p)).ToList();
            ReturnType = to2Function.declaredReturn.UnderlyingType(methodContext.ModuleContext);
            this.methodContext = methodContext;
            this.to2Function = to2Function;
        }

        public IKontrolModule Module => module;

        public string Name => to2Function.name;

        public string Description => to2Function.description;

        public MethodInfo RuntimeMethod => methodContext.MethodBuilder;

        public bool IsCompiled => false;

        public bool IsAsync => to2Function.isAsync;

        public object Invoke(IContext context, params object[] args) =>
            throw new NotImplementedException($"Function {to2Function.name} not yet compiled");
    }

    public class DeclaredKontrolStructConstructor : IKontrolFunction {
        private readonly DeclaredKontrolModule module;
        public List<RealizedParameter> Parameters { get; }
        public RealizedType ReturnType { get; }
        public readonly IBlockContext methodContext;
        public readonly StructDeclaration to2Struct;

        public DeclaredKontrolStructConstructor(DeclaredKontrolModule module, IBlockContext methodContext,
            StructDeclaration to2Struct) {
            this.module = module;
            Parameters = to2Struct.constructorParameters.Select(p => new RealizedParameter(methodContext, p)).ToList();
            ReturnType = to2Struct.typeDelegate.UnderlyingType(methodContext.ModuleContext);
            this.methodContext = methodContext;
            this.to2Struct = to2Struct;
        }

        public IKontrolModule Module => module;

        public string Name => to2Struct.name;

        public string Description => to2Struct.description;

        public MethodInfo RuntimeMethod => methodContext.MethodBuilder;

        public bool IsCompiled => false;

        public bool IsAsync => false;

        public object Invoke(IContext context, params object[] args) =>
            throw new NotImplementedException($"Function {to2Struct.name} not yet compiled");
    }
}
