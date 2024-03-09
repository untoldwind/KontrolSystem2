using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.Runtime;

public class REPLContext {
    public delegate REPLVariable? VariableResolver(string? name);

    private readonly VariableResolver? externalVariables;
    private readonly Dictionary<string, REPLVariable> localVariables = new();
    public readonly REPLBlockContext replBlockContext;
    public readonly REPLModuleContext replModuleContext;
    private readonly IContext runtimeContext;

    public REPLContext(KontrolRegistry registry, IContext runtimeContext) : this(runtimeContext,
        new REPLModuleContext(new Context(registry))) {
    }

    public REPLContext(IContext runtimeContext, REPLModuleContext replModuleContext,
        VariableResolver? externalVariables = null) {
        this.runtimeContext = runtimeContext;
        this.replModuleContext = replModuleContext;
        replBlockContext = new REPLBlockContext(this.replModuleContext, FindVariable);
        this.externalVariables = externalVariables;
    }

    public REPLVariable? FindVariable(string? name) {
        return localVariables!.Get(name) ?? externalVariables?.Invoke(name);
    }

    public REPLVariable DeclaredVariable(string name, bool isConst, RealizedType declaredType) {
        var variable = new REPLVariable(name, isConst, declaredType);

        localVariables.Add(name, variable);

        return variable;
    }

    public REPLContext CreateChildContext() {
        return new REPLContext(runtimeContext, replModuleContext, FindVariable);
    }

    public class REPLVariable(string name, bool isConst, RealizedType declaredType) : IBlockVariable {
        public readonly RealizedType declaredType = declaredType;
        public readonly bool isConst = isConst;
        public IREPLValue? value;

        public string Name => name;

        public RealizedType Type => declaredType;

        public bool IsConst => isConst;

        public void EmitLoad(IBlockContext context) {
            throw new REPLException("No code generation");
        }

        public void EmitLoadPtr(IBlockContext context) {
            throw new REPLException("No code generation");
        }

        public void EmitStore(IBlockContext context) {
            throw new REPLException("No code generation");
        }
    }
}

public class REPLModuleContext(Context rootContext) : ModuleContext(rootContext, "repl");

public delegate IBlockVariable? VariableLookup(string? name);

public class REPLBlockContext(
    REPLModuleContext moduleContext,
    VariableLookup variableLookup,
    List<StructuralError>? errors = null)
    : IBlockContext {
    public ModuleContext ModuleContext => moduleContext;
    public MethodBuilder MethodBuilder => throw new REPLException("No method builder");
    public IILEmitter IL { get; } = null!;
    public TO2Type ExpectedReturn => BuiltinType.Unit;

    public bool IsAsync => false;

    public void AddError(StructuralError error) {
        AllErrors.Add(error);
    }

    public bool HasErrors => AllErrors.Count > 0;

    public List<StructuralError> AllErrors { get; } = errors ?? [];

    public IBlockContext CreateChildContext() {
        return new REPLBlockContext(moduleContext, variableLookup, AllErrors);
    }

    public IBlockContext CreateLoopContext(LabelRef start, LabelRef end) {
        throw new REPLException("Loop block context");
    }

    public IBlockContext CloneCountingContext() {
        throw new REPLException("No counting context");
    }

    public (LabelRef start, LabelRef end)? InnerLoop => throw new REPLException("No inner loop");

    public IBlockVariable? FindVariable(string? name) {
        return variableLookup(name);
    }

    public ITempBlockVariable MakeTempVariable(RealizedType to2Type) {
        throw new REPLException("No temp block variables");
    }

    public ILocalRef DeclareHiddenLocal(Type rawType) {
        throw new REPLException("No hidden local");
    }

    public IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type) {
        throw new REPLException("No declare block variables");
    }

    public void RegisterAsyncResume(TO2Type returnType) {
        throw new REPLException("No async resume");
    }

    public Dictionary<string, RealizedType> InferredGenerics { get; } = new();
}
