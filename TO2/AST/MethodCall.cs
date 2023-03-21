﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class MethodCall : Expression {
        private readonly Expression target;
        private readonly string methodName;
        private readonly List<Expression> arguments;
        private ILocalRef preparedResult;

        public MethodCall(Expression target, string methodName, List<Expression> arguments,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.target = target;
            this.methodName = methodName;
            this.arguments = arguments;
            for (int j = 0; j < this.arguments.Count; j++) {
                int i = j; // Copy for lambda
                this.arguments[i].TypeHint = context => {
                    TO2Type targetType = this.target.ResultType(context);
                    IMethodInvokeFactory methodInvoker = targetType.FindMethod(context.ModuleContext, this.methodName);

                    return methodInvoker?.ArgumentHint(i)?.Invoke(context);
                };
            }
        }

        public override IVariableContainer VariableContainer {
            set {
                target.VariableContainer = value;
                foreach (Expression argument in arguments) argument.VariableContainer = value;
            }
        }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type targetType = target.ResultType(context);
            IMethodInvokeFactory method = targetType.FindMethod(context.ModuleContext, methodName);

            if (method != null) {
                IMethodInvokeEmitter methodInvoker = method.Create(context,
                    arguments.Select(arg => arg.ResultType(context)).ToList(), this);

                if (methodInvoker != null) return methodInvoker.ResultType;
            }

            IFieldAccessFactory field = targetType.FindField(context.ModuleContext, methodName);

            if (field != null) {
                IFieldAccessEmitter fieldAccess = field.Create(context.ModuleContext);
                FunctionType functionType = fieldAccess.FieldType as FunctionType;

                if (functionType == null) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.NoSuchMethod,
                        $"Field '{methodName}' of type '{targetType.Name}' is neither a method or a function",
                        Start,
                        End
                    ));
                    return BuiltinType.Unit;
                } else {
                    return functionType.returnType;
                }
            }

            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchMethod,
                $"Type '{targetType.Name}' does not have a method or field '{methodName}'",
                Start,
                End
            ));
            return BuiltinType.Unit;
        }

        public override void Prepare(IBlockContext context) {
            if (preparedResult != null) return;

            TO2Type targetType = target.ResultType(context);
            IMethodInvokeEmitter methodInvoker = targetType.FindMethod(context.ModuleContext, methodName)
                ?.Create(context, arguments.Select(arg => arg.ResultType(context)).ToList(), this);

            target.Prepare(context);

            if (methodInvoker == null || !methodInvoker.IsAsync || !context.IsAsync) return;

            EmitCode(context, false);
            preparedResult = context.DeclareHiddenLocal(methodInvoker.ResultType.GeneratedType(context.ModuleContext));
            preparedResult.EmitStore(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (preparedResult != null) {
                if (!dropResult) preparedResult.EmitLoad(context);
                preparedResult = null;
                return;
            }

            TO2Type targetType = target.ResultType(context);
            IMethodInvokeFactory method = targetType.FindMethod(context.ModuleContext, methodName);

            if (method != null) {
                EmitCodeMethodCall(context, targetType, method, dropResult);
                return;
            }

            IFieldAccessFactory field = targetType.FindField(context.ModuleContext, methodName);

            if (field != null) {
                EmitCodeDelegateCall(context, targetType, field, dropResult);
                return;
            }

            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchMethod,
                $"Type '{targetType.Name}' does not have a method or field '{methodName}'",
                Start,
                End
            ));
        }

        private void EmitCodeMethodCall(IBlockContext context, TO2Type targetType, IMethodInvokeFactory method,
            bool dropResult) {
            if (target is IAssignContext assignContext) {
                if (assignContext.IsConst(context) && !method.IsConst) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.NoSuchMethod,
                        $"Method '{methodName}' will mutate const variable.",
                        Start,
                        End
                    ));
                }
            }

            List<TO2Type> argumentTypes = arguments.Select(arg => arg.ResultType(context)).ToList();
            IMethodInvokeEmitter methodInvoker = method.Create(context, argumentTypes, this);

            if (methodInvoker == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchMethod,
                    $"Type '{targetType.Name}' does not have a method '{methodName}' matching arguments ({string.Join(", ", argumentTypes)})",
                    Start,
                    End
                ));
                return;
            }

            if (methodInvoker.IsAsync && !context.IsAsync) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchFunction,
                    $"Cannot call async method of variable '{targetType.Name}.{methodName}' from a sync context",
                    Start,
                    End
                ));
                return;
            }

            if (arguments.Count > methodInvoker.Parameters.Count) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.ArgumentMismatch,
                    $"Function '{targetType.Name}.{methodName}' only allows {methodInvoker.Parameters.Count} arguments, {arguments.Count} where given",
                    Start,
                    End
                ));
                return;
            }

            if (methodInvoker.RequiredParameterCount() > arguments.Count) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.ArgumentMismatch,
                    $"Method '{targetType.Name}.{methodName}' requires {methodInvoker.RequiredParameterCount()} arguments, {arguments.Count} where given",
                    Start,
                    End
                ));
                return;
            }

            int i;
            for (i = 0; i < arguments.Count; i++) {
                TO2Type argumentType = arguments[i].ResultType(context);
                if (!methodInvoker.Parameters[i].type.IsAssignableFrom(context.ModuleContext, argumentType)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.ArgumentMismatch,
                        $"Argument {methodInvoker.Parameters[i].name} of '{targetType.Name}.{methodName}' has to be a {methodInvoker.Parameters[i].type}, but got {argumentType}",
                        Start,
                        End
                    ));
                    return;
                }
            }

            foreach (Expression argument in arguments) {
                argument.Prepare(context);
            }

            if (methodInvoker.RequiresPtr)
                target.EmitPtr(context);
            else
                target.EmitCode(context, false);
            for (i = 0; i < arguments.Count; i++) {
                arguments[i].EmitCode(context, false);
                if (!context.HasErrors)
                    methodInvoker.Parameters[i].type.AssignFrom(context.ModuleContext, arguments[i].ResultType(context))
                        .EmitConvert(context);
            }

            if (!context.HasErrors) {
                for (; i < methodInvoker.Parameters.Count; i++) {
                    methodInvoker.Parameters[i].defaultValue.EmitCode(context);
                }
            }

            if (context.HasErrors) return;

            methodInvoker.EmitCode(context);
            if (methodInvoker.IsAsync) context.RegisterAsyncResume(methodInvoker.ResultType);
            if (dropResult) context.IL.Emit(OpCodes.Pop);
        }

        private void EmitCodeDelegateCall(IBlockContext context, TO2Type targetType, IFieldAccessFactory field,
            bool dropResult) {
            IFieldAccessEmitter fieldAccess = field.Create(context.ModuleContext);
            FunctionType functionType = fieldAccess.FieldType as FunctionType;

            if (functionType == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchMethod,
                    $"Field '{methodName}' of type '{targetType.Name}' is neither a method or a function",
                    Start,
                    End
                ));
                return;
            }

            if (functionType.isAsync && !context.IsAsync) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchFunction,
                    $"Cannot call async function of variable {methodName}' of type '{targetType.Name} from a sync context",
                    Start,
                    End
                ));
                return;
            }

            if (functionType.parameterTypes.Count != arguments.Count) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.ArgumentMismatch,
                    $"Call to '{methodName}' of type '{targetType.Name}' requires {functionType.parameterTypes.Count} arguments",
                    Start,
                    End
                ));
                return;
            }

            if (fieldAccess.RequiresPtr)
                target.EmitPtr(context);
            else
                target.EmitCode(context, false);
            fieldAccess.EmitLoad(context);

            for (int i = 0; i < arguments.Count; i++) {
                arguments[i].EmitCode(context, false);
                if (!context.HasErrors)
                    functionType.parameterTypes[i].AssignFrom(context.ModuleContext, arguments[i].ResultType(context))
                        .EmitConvert(context);
            }

            if (context.HasErrors) return;

            for (int i = 0; i < arguments.Count; i++) {
                TO2Type argumentType = arguments[i].ResultType(context);
                if (!functionType.parameterTypes[i].IsAssignableFrom(context.ModuleContext, argumentType)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.ArgumentMismatch,
                        $"Argument {i + 1} of '{methodName}' of type '{targetType.Name}' has to be a {functionType.parameterTypes[i]}, but {argumentType} was given",
                        Start,
                        End
                    ));
                    return;
                }
            }

            MethodInfo invokeMethod = functionType.GeneratedType(context.ModuleContext).GetMethod("Invoke") ??
                                      throw new ArgumentException($"No Invoke method in generated ${functionType}");

            context.IL.EmitCall(OpCodes.Callvirt, invokeMethod, arguments.Count + 1);
            if (functionType.isAsync) context.RegisterAsyncResume(functionType.returnType);
            if (dropResult && invokeMethod.ReturnType != typeof(void)) context.IL.Emit(OpCodes.Pop);
            if (!dropResult && invokeMethod.ReturnType == typeof(void)) context.IL.Emit(OpCodes.Ldnull);
        }

        public override REPLValueFuture Eval(REPLContext context) {
            var targetFuture = target.Eval(context);
            var argumentFutures = arguments.Select(p => p.Eval(context)).ToList();
            
            IMethodInvokeFactory method = targetFuture.Type.FindMethod(context.replModuleContext, methodName);

            if (method != null) {
                IMethodInvokeEmitter methodInvoker = method.Create(context.replBlockContext, argumentFutures.Select(a => targetFuture.Type).ToList(), this);

                if (context.replBlockContext.HasErrors) {
                    throw new REPLException(this, context.replBlockContext.AllErrors.First().message);
                }
                
                if (arguments.Count > methodInvoker.Parameters.Count) {
                    throw new REPLException(this, $"Function '{targetFuture.Type.Name}.{methodName}' only allows {methodInvoker.Parameters.Count} arguments, {arguments.Count} where given");
                }

                if (methodInvoker.RequiredParameterCount() > arguments.Count) {
                    throw new REPLException(this, $"Method '{targetFuture.Type.Name}.{methodName}' requires {methodInvoker.RequiredParameterCount()} arguments, {arguments.Count} where given");
                }
                
                for (int i = 0; i < arguments.Count; i++) {
                    TO2Type argumentType = argumentFutures[i].Type;
                    if (!methodInvoker.Parameters[i].type.IsAssignableFrom(context.replModuleContext, argumentType)) {
                        throw new REPLException(this,$"Argument {methodInvoker.Parameters[i].name} of '{targetFuture.Type.Name}.{methodName}' has to be a {methodInvoker.Parameters[i].type}, but got {argumentType}");
                    }
                }
                
                argumentFutures.Insert(0, targetFuture);

                return REPLValueFuture.ChainN(methodInvoker.ResultType, argumentFutures.ToArray(),
                    targetWithArguments => methodInvoker.Eval(this, targetWithArguments));
            }

            throw new REPLException(this, $"Type '{targetFuture.Type.Name}' does not have a method or field '{methodName}'");
        }
    }
}
