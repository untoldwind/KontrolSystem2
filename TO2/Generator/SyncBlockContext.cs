using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator;

public class SyncBlockContext : IBlockContext {
    private readonly Dictionary<string, IBlockVariable> variables;
    private VariableResolver? externalVariables;

    private SyncBlockContext(SyncBlockContext parent, IILEmitter il, (LabelRef start, LabelRef end)? innerLoop) {
        ModuleContext = parent.ModuleContext;
        MethodBuilder = parent.MethodBuilder;
        ExpectedReturn = parent.ExpectedReturn;
        externalVariables = parent.externalVariables;
        this.IL = il;
        variables = parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
        AllErrors = parent.AllErrors;
        this.InnerLoop = innerLoop;
        InferredGenerics = new Dictionary<string, RealizedType>();
    }

    public SyncBlockContext(ModuleContext moduleContext) {
        this.ModuleContext = moduleContext;
        MethodBuilder = null;
        ExpectedReturn = BuiltinType.Unit;
        IL = moduleContext.constructorEmitter!;
        variables = new Dictionary<string, IBlockVariable>();
        AllErrors = new List<StructuralError>();
        InnerLoop = null;
        InferredGenerics = new Dictionary<string, RealizedType>();
    }

    protected SyncBlockContext(ModuleContext moduleContext, IILEmitter il) {
        this.ModuleContext = moduleContext;
        MethodBuilder = null;
        ExpectedReturn = BuiltinType.Unit;
        this.IL = il;
        variables = new Dictionary<string, IBlockVariable>();
        AllErrors = new List<StructuralError>();
        InnerLoop = null;
        InferredGenerics = new Dictionary<string, RealizedType>();
    }

    public SyncBlockContext(ModuleContext moduleContext, FunctionModifier modifier, bool isAsync, string methodName,
        TO2Type returnType, List<FunctionParameter> parameters) {
        this.ModuleContext = moduleContext;
        MethodBuilder = this.ModuleContext.typeBuilder.DefineMethod(methodName,
            modifier == FunctionModifier.Private
                ? MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static
                : MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static,
            isAsync
                ? this.ModuleContext.FutureTypeOf(returnType).future
                : returnType.GeneratedType(this.ModuleContext),
            parameters.Select(parameter => parameter.type!.GeneratedType(this.ModuleContext)).ToArray());
        ExpectedReturn = returnType;
        IL = new GeneratorILEmitter(MethodBuilder.GetILGenerator());
        variables = parameters.Select<FunctionParameter, IBlockVariable>((p, idx) =>
            new MethodParameter(p.name, p.type!.UnderlyingType(this.ModuleContext), idx)).ToDictionary(p => p.Name);
        AllErrors = new List<StructuralError>();
        InnerLoop = null;
    }

    // LambdaImpl only!!
    public SyncBlockContext(ModuleContext moduleContext, bool isAsync, string methodName, TO2Type returnType,
        List<FunctionParameter> parameters) {
        this.ModuleContext = moduleContext;
        MethodBuilder = this.ModuleContext.typeBuilder.DefineMethod(methodName,
            MethodAttributes.Public | MethodAttributes.HideBySig,
            isAsync
                ? this.ModuleContext.FutureTypeOf(returnType).future
                : returnType.GeneratedType(this.ModuleContext),
            parameters.Select(parameter => parameter.type!.GeneratedType(this.ModuleContext)).ToArray());
        ExpectedReturn = returnType;
        IL = new GeneratorILEmitter(MethodBuilder.GetILGenerator());
        variables = parameters.Select<FunctionParameter, IBlockVariable>((p, idx) =>
                new MethodParameter(p.name, p.type!.UnderlyingType(this.ModuleContext), idx + 1))
            .ToDictionary(p => p.Name);
        AllErrors = new List<StructuralError>();
        InnerLoop = null;
    }

    // Struct methods only
    public SyncBlockContext(StructTypeAliasDelegate structType, bool isAsync, string methodName,
        TO2Type returnType,
        List<FunctionParameter> parameters) : this(structType.declaredModule, isAsync, methodName, returnType,
        parameters) {
        variables.Add("self",
            new MethodParameter("self", structType.UnderlyingType(structType.structContext), 0, false));
    }

    public VariableResolver ExternVariables {
        set => externalVariables = value;
    }

    public ModuleContext ModuleContext { get; }

    public MethodBuilder? MethodBuilder { get; }

    public IILEmitter IL { get; }

    public TO2Type ExpectedReturn { get; }

    public bool IsAsync => false;

    public void AddError(StructuralError error) {
        AllErrors.Add(error);
    }

    public bool HasErrors => AllErrors.Count > 0;

    public List<StructuralError> AllErrors { get; }

    public (LabelRef start, LabelRef end)? InnerLoop { get; }

    public IBlockContext CreateChildContext() {
        return new SyncBlockContext(this, IL, InnerLoop);
    }

    public IBlockContext CreateLoopContext(LabelRef start, LabelRef end) {
        return new SyncBlockContext(this, IL, (start, end));
    }

    public IBlockContext CloneCountingContext() {
        return new SyncBlockContext(this, new CountingILEmitter(IL.LastLocalIndex), InnerLoop);
    }

    public ITempBlockVariable MakeTempVariable(RealizedType to2Type) {
        var type = to2Type.GeneratedType(ModuleContext);
        var localRef = IL.TempLocal(type);

        var variable = new TempVariable(to2Type, localRef);

        to2Type.EmitInitialize(this, variable);
        return variable;
    }

    public IBlockVariable? FindVariable(string name) {
        return variables.Get(name) ?? externalVariables?.Invoke(name);
    }

    public ILocalRef DeclareHiddenLocal(Type rawType) {
        return IL.DeclareLocal(rawType);
    }

    public IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type) {
        var localRef = IL.DeclareLocal(to2Type.GeneratedType(ModuleContext));
        var variable = new DeclaredVariable(name, isConst, to2Type, localRef);

        to2Type.EmitInitialize(this, variable);
        variables.Add(name, variable);

        return variable;
    }

    public Dictionary<string, RealizedType>? InferredGenerics { get; }

    public void RegisterAsyncResume(TO2Type returnType) {
    }
}
