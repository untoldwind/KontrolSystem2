using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator;

public readonly struct AsyncResume {
    internal readonly LabelRef resumeLabel;
    internal readonly LabelRef pollLabel;
    private readonly FieldInfo futureField;
    private readonly ILocalRef futureResultVar;

    internal AsyncResume(LabelRef resumeLabel, LabelRef pollLabel, FieldInfo futureField,
        ILocalRef futureResultVar) {
        this.resumeLabel = resumeLabel;
        this.pollLabel = pollLabel;
        this.futureField = futureField;
        this.futureResultVar = futureResultVar;
    }

    internal void EmitPoll(AsyncBlockContext context) {
        context.IL.MarkLabel(pollLabel);
        context.IL.Emit(OpCodes.Ldarg_0);
        context.IL.Emit(OpCodes.Ldfld, futureField);
        context.IL.EmitCall(OpCodes.Callvirt, futureField.FieldType.GetMethod("PollValue")!, 1);
        futureResultVar.EmitStore(context);
        futureResultVar.EmitLoad(context);
        context.IL.Emit(OpCodes.Ldfld, futureResultVar.LocalType.GetField("ready"));
        context.IL.Emit(OpCodes.Brfalse, context.notReady);
        context.IL.Emit(OpCodes.Br, context.resume);
    }
}

public readonly struct StateRef {
    private readonly ILocalRef localRef;
    private readonly FieldInfo storageField;

    internal StateRef(ILocalRef localRef, FieldInfo storageField) {
        this.localRef = localRef;
        this.storageField = storageField;
    }

    internal void EmitStore(AsyncBlockContext context) {
        context.IL.Emit(OpCodes.Ldarg_0);
        localRef.EmitLoad(context);
        context.IL.Emit(OpCodes.Stfld, storageField);
    }

    internal void EmitRestore(AsyncBlockContext context) {
        context.IL.Emit(OpCodes.Ldarg_0);
        context.IL.Emit(OpCodes.Ldfld, storageField);
        localRef.EmitStore(context);
    }
}

public class AsyncBlockContext : IBlockContext {
    internal readonly List<AsyncResume>? asyncResumes;
    internal readonly LabelRef notReady;
    internal readonly LabelRef resume;
    private readonly Context root;
    internal readonly FieldInfo stateField;
    internal readonly List<StateRef>? stateRefs;
    internal readonly LabelRef storeState;
    private readonly Dictionary<string, IBlockVariable> variables;

    private AsyncBlockContext(AsyncBlockContext parent, (LabelRef start, LabelRef end)? innerLoop) {
        root = parent.root;
        ModuleContext = parent.ModuleContext;
        MethodBuilder = parent.MethodBuilder;
        ExpectedReturn = parent.ExpectedReturn;
        IL = parent.IL;
        variables = parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
        AllErrors = parent.AllErrors;
        InnerLoop = innerLoop;
        asyncResumes = parent.asyncResumes;
        stateRefs = parent.stateRefs;
        stateField = parent.stateField;
        storeState = parent.storeState;
        notReady = parent.notReady;
        resume = parent.resume;
        InferredGenerics = new Dictionary<string, RealizedType>();
    }

    private AsyncBlockContext(AsyncBlockContext parent, IILEmitter il, (LabelRef start, LabelRef end)? innerLoop) {
        root = parent.root;
        ModuleContext = parent.ModuleContext;
        MethodBuilder = parent.MethodBuilder;
        ExpectedReturn = parent.ExpectedReturn;
        IL = il;
        variables = parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
        AllErrors = parent.AllErrors;
        InnerLoop = innerLoop;
        stateField = parent.stateField;
        asyncResumes = null;
        stateRefs = null;
        storeState = parent.storeState;
        notReady = parent.notReady;
        resume = parent.resume;
        InferredGenerics = new Dictionary<string, RealizedType>();
    }

    public AsyncBlockContext(ModuleContext moduleContext, FunctionModifier modifier, string methodName,
        TO2Type expectedReturn, Type generatedReturn, IEnumerable<IBlockVariable> parameters) {
        ModuleContext = moduleContext;
        root = ModuleContext.root;
        MethodBuilder = ModuleContext.typeBuilder.DefineMethod(methodName,
            modifier == FunctionModifier.Private
                ? MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual
                : MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
            generatedReturn,
            new Type[0]);
        ExpectedReturn = expectedReturn;
        IL = new GeneratorILEmitter(MethodBuilder.GetILGenerator());
        variables = parameters.ToDictionary(p => p.Name);
        AllErrors = new List<StructuralError>();
        InnerLoop = null;
        stateField =
            ModuleContext.typeBuilder.DefineField("<async>_state", typeof(int), FieldAttributes.Private);
        asyncResumes = new List<AsyncResume>();
        stateRefs = new List<StateRef>();
        storeState = IL.DefineLabel(false);
        notReady = IL.DefineLabel(false);
        resume = IL.DefineLabel(false);
        InferredGenerics = new Dictionary<string, RealizedType>();
    }

    public ModuleContext ModuleContext { get; }

    public MethodBuilder MethodBuilder { get; }

    public IILEmitter IL { get; }

    public TO2Type ExpectedReturn { get; }

    public bool IsAsync => true;

    public void AddError(StructuralError error) {
        AllErrors.Add(error);
    }

    public bool HasErrors => AllErrors.Count > 0;

    public List<StructuralError> AllErrors { get; }

    public (LabelRef start, LabelRef end)? InnerLoop { get; }

    public IBlockContext CreateChildContext() {
        return new AsyncBlockContext(this, InnerLoop);
    }

    public IBlockContext CreateLoopContext(LabelRef start, LabelRef end) {
        return new AsyncBlockContext(this, (start, end));
    }

    public IBlockContext CloneCountingContext() {
        return new AsyncBlockContext(this, new CountingILEmitter(IL.LastLocalIndex), InnerLoop);
    }

    public ITempBlockVariable MakeTempVariable(RealizedType to2Type) {
        var type = to2Type.GeneratedType(ModuleContext);
        using var localRef = IL.TempLocal(type);
        var variable = new TempVariable(to2Type, localRef);

        to2Type.EmitInitialize(this, variable);
        return variable;
    }

    public IBlockVariable? FindVariable(string? name) {
        return variables!.Get(name);
    }

    public ILocalRef DeclareHiddenLocal(Type rawType) {
        var localRef = IL.DeclareLocal(rawType);

        if (stateRefs != null) {
            var storeField = ModuleContext.typeBuilder.DefineField($"<async>_store_{stateRefs.Count}",
                rawType, FieldAttributes.Private);
            stateRefs.Add(new StateRef(localRef, storeField));
        }

        return localRef;
    }

    public IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type) {
        var type = to2Type.GeneratedType(ModuleContext);
        var localRef = IL.DeclareLocal(type);
        var variable = new DeclaredVariable(name, isConst, to2Type, localRef);

        variables.Add(name, variable);
        to2Type.EmitInitialize(this, variable);

        if (stateRefs != null) {
            var storeField = ModuleContext.typeBuilder.DefineField($"<async>_store_{stateRefs.Count}",
                type, FieldAttributes.Private);
            stateRefs.Add(new StateRef(localRef, storeField));
        }

        return variable;
    }

    public void RegisterAsyncResume(TO2Type returnType) {
        var state = (asyncResumes?.Count ?? 0) + 1;

        (var futureType, var futureResultType) = ModuleContext.FutureTypeOf(returnType);
        FieldInfo futureField = ModuleContext.typeBuilder.DefineField($"<async>_future_{state}", futureType, FieldAttributes.Private);
        var futureResultVar = IL.DeclareLocal(futureResultType);
        using (var futureTemp = IL.TempLocal(futureType)) {
            futureTemp.EmitStore(this);
            IL.Emit(OpCodes.Ldarg_0);
            futureTemp.EmitLoad(this);
        }

        IL.Emit(OpCodes.Stfld, futureField);

        IL.Emit(OpCodes.Ldarg_0);
        IL.Emit(OpCodes.Ldc_I4, state);
        IL.Emit(OpCodes.Stfld, stateField);
        IL.Emit(OpCodes.Br, storeState);

        var resumeLabel = IL.DefineLabel(false);
        IL.MarkLabel(resumeLabel);
        futureResultVar.EmitLoad(this);
        IL.Emit(OpCodes.Ldfld, futureResultVar.LocalType.GetField("value"));

        asyncResumes?.Add(new AsyncResume(resumeLabel, IL.DefineLabel(false), futureField, futureResultVar));
    }

    public Dictionary<string, RealizedType> InferredGenerics { get; }
}
