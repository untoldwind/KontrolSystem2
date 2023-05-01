using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public interface IMethodInvokeEmitter {
        RealizedType ResultType { get; }

        List<RealizedParameter> Parameters { get; }

        bool RequiresPtr { get; }

        bool IsAsync { get; }

        void EmitCode(IBlockContext context);

        REPLValueFuture Eval(Node node, IREPLValue[] targetWithArguments);
    }

    public static class MethodInvokeEmitterExtensions {
        public static int RequiredParameterCount(this IMethodInvokeEmitter method) =>
            method.Parameters.Count(p => !p.HasDefault);
    }

    public interface IMethodInvokeFactory {
        bool IsAsync { get; }

        bool IsConst { get; }

        TypeHint ReturnHint { get; }

        TypeHint ArgumentHint(int argumentIdx);

        string Description { get; }

        TO2Type DeclaredReturn { get; }

        List<FunctionParameter> DeclaredParameters { get; }

        IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node);

        IMethodInvokeFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);
    }

    public delegate REPLValueFuture REPLMethodCall(Node node, IREPLValue[] targetWithArguments);

    public class InlineMethodInvokeFactory : IMethodInvokeFactory {
        private readonly Func<RealizedType> resultType;
        private readonly OpCode[] opCodes;
        private readonly REPLMethodCall methodCall;
        public bool IsConst { get; }
        public string Description { get; }

        public InlineMethodInvokeFactory(string description, Func<RealizedType> returnType, bool isConst,
            REPLMethodCall methodCall, params OpCode[] opCodes) {
            Description = description;
            resultType = returnType;
            IsConst = isConst;
            this.methodCall = methodCall;
            this.opCodes = opCodes;
        }

        public bool IsAsync => false;

        public TypeHint ReturnHint => _ => resultType();

        public TypeHint ArgumentHint(int argumentIdx) => null;

        public TO2Type DeclaredReturn => resultType();

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter>();

        public IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node) =>
            new InlineMethodInvokeEmitter(resultType(), new List<RealizedParameter>(), methodCall, opCodes);

        public IMethodInvokeFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    public class InlineMethodInvokeEmitter : IMethodInvokeEmitter {
        private readonly OpCode[] opCodes;
        private readonly REPLMethodCall methodCall;

        public RealizedType ResultType { get; }

        public List<RealizedParameter> Parameters { get; }

        public InlineMethodInvokeEmitter(RealizedType returnType, List<RealizedParameter> parameters, REPLMethodCall methodCall,
            params OpCode[] opCodes) {
            ResultType = returnType;
            Parameters = parameters;
            this.methodCall = methodCall;
            this.opCodes = opCodes;
        }


        public bool IsAsync => false;

        public bool RequiresPtr => false;

        public void EmitCode(IBlockContext context) {
            foreach (OpCode opCode in opCodes) {
                context.IL.Emit(opCode);
            }
        }

        public REPLValueFuture Eval(Node node, IREPLValue[] targetWithArguments) => methodCall(node, targetWithArguments);
    }

    public class BoundMethodInvokeFactory : IMethodInvokeFactory {
        public bool IsConst { get; }
        public string Description { get; }
        private readonly Func<RealizedType> resultType;
        private readonly Func<List<RealizedParameter>> parameters;
        private readonly bool isAsync;
        private readonly MethodInfo methodInfo;
        private readonly Type methodTarget;
        private readonly Func<ModuleContext, IEnumerable<(string name, RealizedType type)>> targetTypeArguments;
        private readonly bool constrained;

        public BoundMethodInvokeFactory(string description, bool isConst, Func<RealizedType> resultType,
            Func<List<RealizedParameter>> parameters, bool isAsync, Type methodTarget, MethodInfo methodInfo,
            Func<ModuleContext, IEnumerable<(string name, RealizedType type)>> targetTypeArguments = null, bool constrained = false) {
            Description = description;
            IsConst = isConst;
            this.resultType = resultType;
            this.parameters = parameters;
            this.methodInfo = methodInfo ??
                              throw new ArgumentException(
                                  $"MethodInfo is null for {description} in type {methodTarget}");
            this.methodTarget = methodTarget;
            this.isAsync = isAsync;
            this.targetTypeArguments = targetTypeArguments;
            this.constrained = constrained;
        }

        public bool IsAsync => isAsync;

        public TypeHint ReturnHint => _ => resultType();

        public TypeHint ArgumentHint(int argumentIdx) {
            List<RealizedParameter> p = parameters();

            return context => argumentIdx >= 0 && argumentIdx < p.Count ? p[argumentIdx].type : null;
        }

        public TO2Type DeclaredReturn => resultType();

        public List<FunctionParameter> DeclaredParameters =>
            parameters().Select(p => new FunctionParameter(p.name, p.type)).ToList();

        public IMethodInvokeEmitter Create(IBlockContext context, List<TO2Type> arguments, Node node) {
            (MethodInfo genericMethod, RealizedType genericResult, List<RealizedParameter> genericParameters) =
                Helpers.MakeGeneric(context,
                    resultType(), parameters(), methodInfo,
                    null, arguments,
                    targetTypeArguments?.Invoke(context.ModuleContext) ??
                    Enumerable.Empty<(string name, RealizedType type)>(),
                    node);

            if (context.HasErrors) return null;

            return new BoundMethodInvokeEmitter(genericResult, genericParameters, isAsync, methodTarget, genericMethod, constrained);
        }

        public IMethodInvokeFactory FillGenerics(ModuleContext context,
            Dictionary<string, RealizedType> typeArguments) {
            if (methodTarget.IsGenericTypeDefinition) {
                Type[] arguments = methodTarget.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name))
                        throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();
                Type genericTarget = methodTarget.MakeGenericType(arguments);
                List<RealizedParameter> genericParams =
                    parameters().Select(p => p.FillGenerics(context, typeArguments)).ToList();
                MethodInfo genericMethod = genericTarget.GetMethod(methodInfo.Name,
                    genericParams.Select(p => p.type.GeneratedType(context)).ToArray());

                if (genericMethod == null)
                    throw new ArgumentException(
                        $"Unable to relocate method {methodInfo.Name} on {methodTarget} for type arguments {typeArguments}");

                return new BoundMethodInvokeFactory(Description, IsConst,
                    () => resultType().FillGenerics(context, typeArguments), () => genericParams, isAsync,
                    genericTarget, genericMethod);
            }

            return this;
        }
    }

    public class BoundMethodInvokeEmitter : IMethodInvokeEmitter {
        private readonly MethodInfo methodInfo;
        private readonly Type methodTarget;
        private readonly bool constrained;
        public RealizedType ResultType { get; }
        public List<RealizedParameter> Parameters { get; }
        public bool IsAsync { get; }

        public BoundMethodInvokeEmitter(RealizedType resultType, List<RealizedParameter> parameters, bool isAsync,
            Type methodTarget, MethodInfo methodInfo, bool constrained = false) {
            ResultType = resultType;
            Parameters = parameters;
            this.methodInfo = methodInfo;
            this.methodTarget = methodTarget;
            this.IsAsync = isAsync;
            this.constrained = constrained;
        }

        public bool RequiresPtr =>
            methodTarget.IsValueType && (methodInfo.CallingConvention & CallingConventions.HasThis) != 0;

        public void EmitCode(IBlockContext context) {
            if (methodInfo.IsVirtual) {
                if (constrained) context.IL.Emit(OpCodes.Constrained, methodTarget);
                context.IL.EmitCall(OpCodes.Callvirt, methodInfo, Parameters.Count + 1);
            } else context.IL.EmitCall(OpCodes.Call, methodInfo, Parameters.Count + 1);
            if (methodInfo.ReturnType == typeof(void)) context.IL.Emit(OpCodes.Ldnull);
        }

        public REPLValueFuture Eval(Node node, IREPLValue[] targetWithArguments) {
            var result = methodInfo.IsStatic ? methodInfo.Invoke(null, targetWithArguments.Select(a => a.Value).ToArray()) :
                methodInfo.Invoke(targetWithArguments[0].Value, targetWithArguments.Skip(1).Select(a => a.Value).ToArray());

            return IsAsync ? REPLValueFuture.Wrap(ResultType, result as IAnyFuture) : REPLValueFuture.Success(ResultType.REPLCast(result));
        }
    }
}
