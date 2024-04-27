﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

internal readonly struct AsyncClass {
    internal readonly TypeBuilder type;
    internal readonly ConstructorInfo constructor;

    private AsyncClass(TypeBuilder type, ConstructorInfo constructor) {
        this.type = type;
        this.constructor = constructor;
    }

    internal static AsyncClass Create(IBlockContext parent, string name, TO2Type declaredReturn,
        List<FunctionParameter> parameters, Expression expression) {
        var returnType = declaredReturn.GeneratedType(parent.ModuleContext);
        var typeParameter = returnType == typeof(void) ? typeof(object) : returnType;

        var asyncModuleContext = parent.ModuleContext.DefineSubContext($"AsyncFunction_{name}",
            typeof(Future<>).MakeGenericType(typeParameter));

        var clonedParameters = new List<ClonedFieldVariable>();

        foreach (var parameter in parameters) {
            var field = asyncModuleContext.typeBuilder.DefineField(parameter.name,
                parameter.type!.GeneratedType(parent.ModuleContext), FieldAttributes.Private);
            clonedParameters.Add(
                new ClonedFieldVariable(parameter.type.UnderlyingType(parent.ModuleContext), field));
        }

        // ------------- PollValue -------------
        var asyncContext = new AsyncBlockContext(asyncModuleContext, FunctionModifier.Public,
            "PollValue", declaredReturn, typeof(FutureResult<>).MakeGenericType(typeParameter), clonedParameters);

        var applyState = asyncContext.IL.DefineLabel(false);
        var initialState = asyncContext.IL.DefineLabel(false);

        asyncContext.IL.Emit(OpCodes.Br, applyState);
        asyncContext.IL.MarkLabel(initialState);

        expression.EmitCode(asyncContext, false);
        if (!asyncContext.HasErrors)
            declaredReturn.AssignFrom(asyncContext.ModuleContext, expression.ResultType(asyncContext))
                .EmitConvert(asyncContext, false);
        else
            // At this point the IL code is most likely messed up beyond repair
            throw new CompilationErrorException(asyncContext.AllErrors);

        asyncContext.IL.EmitNew(OpCodes.Newobj,
            asyncContext.MethodBuilder.ReturnType.GetConstructor([typeParameter])!);
        ILChunks.GenerateFunctionLeave(asyncContext);
        asyncContext.IL.EmitReturn(asyncContext.MethodBuilder.ReturnType);

        // Apply state
        asyncContext.IL.MarkLabel(applyState);
        asyncContext.IL.Emit(OpCodes.Ldarg_0);
        asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
        asyncContext.IL.Emit(OpCodes.Switch,
            Enumerable.Concat([initialState], asyncContext.asyncResumes!.Select(ar => ar.pollLabel)));
        asyncContext.IL.Emit(OpCodes.Ldarg_0);
        asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
        asyncContext.IL.EmitNew(OpCodes.Newobj,
            typeof(InvalidAsyncStateException).GetConstructor([typeof(int)])!, 1);
        asyncContext.IL.Emit(OpCodes.Throw);

        foreach (var asyncResume in asyncContext.asyncResumes!) asyncResume.EmitPoll(asyncContext);

        // Restore state
        asyncContext.IL.MarkLabel(asyncContext.resume);
        foreach (var stateRef in asyncContext.stateRefs!) stateRef.EmitRestore(asyncContext);
        asyncContext.IL.Emit(OpCodes.Ldarg_0);
        asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
        asyncContext.IL.Emit(OpCodes.Switch,
            Enumerable.Concat([initialState], asyncContext.asyncResumes.Select(ar => ar.resumeLabel)));
        asyncContext.IL.Emit(OpCodes.Ldarg_0);
        asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
        asyncContext.IL.EmitNew(OpCodes.Newobj,
            typeof(InvalidAsyncStateException).GetConstructor([typeof(int)])!, 1);
        asyncContext.IL.Emit(OpCodes.Throw);

        // Store state
        asyncContext.IL.MarkLabel(asyncContext.storeState);
        foreach (var stateRef in asyncContext.stateRefs) stateRef.EmitStore(asyncContext);

        asyncContext.IL.MarkLabel(asyncContext.notReady);
        using (var notReady = asyncContext.IL.TempLocal(asyncContext.MethodBuilder.ReturnType)) {
            notReady.EmitLoadPtr(asyncContext);
            asyncContext.IL.Emit(OpCodes.Initobj, asyncContext.MethodBuilder.ReturnType, 1, 0);
            notReady.EmitLoad(asyncContext);
            asyncContext.IL.EmitReturn(asyncContext.MethodBuilder.ReturnType);
        }

        foreach (var error in asyncContext.AllErrors) parent.AddError(error);

        // ------------- Constructor -------------
        var parameterFields = clonedParameters.Select(c => c.valueField).ToList();
        var constructorBuilder = asyncModuleContext.typeBuilder.DefineConstructor(
            MethodAttributes.Public, CallingConventions.Standard,
            parameterFields.Select(f => f.FieldType).ToArray());
        IILEmitter constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());

        var argIndex = 1;
        foreach (var field in parameterFields) {
            constructorEmitter.Emit(OpCodes.Ldarg_0);
            MethodParameter.EmitLoadArg(constructorEmitter, argIndex++);
            constructorEmitter.Emit(OpCodes.Stfld, field);
        }

        constructorEmitter.Emit(OpCodes.Ldarg_0);
        constructorEmitter.Emit(OpCodes.Ldc_I4_0);
        constructorEmitter.Emit(OpCodes.Stfld, asyncContext.stateField);
        constructorEmitter.EmitReturn(typeof(void));

        return new AsyncClass(asyncModuleContext.typeBuilder, constructorBuilder);
    }
}
