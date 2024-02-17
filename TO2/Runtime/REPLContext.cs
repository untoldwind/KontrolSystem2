using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.Runtime;

public class REPLContext {
    public delegate REPLVariable? VariableResolver(string? name);

    public readonly VariableResolver? externalVariables;
    public readonly Dictionary<string, REPLVariable> localVariables = new();
    public readonly REPLBlockContext replBlockContext;
    public readonly REPLModuleContext replModuleContext;
    public readonly IContext runtimeContext;

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

    public class REPLVariable : IBlockVariable {
        public readonly RealizedType declaredType;
        public readonly bool isConst;
        public readonly string name;
        public IREPLValue? value;

        public REPLVariable(string name, bool isConst, RealizedType declaredType) {
            this.name = name;
            this.isConst = isConst;
            this.declaredType = declaredType;
        }

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

public class REPLModuleContext : ModuleContext {
    public REPLModuleContext(Context rootContext) : base(rootContext, "repl") {
    }
}

public delegate IBlockVariable? VariableLookup(string? name);

public class REPLBlockContext : IBlockContext {
    private readonly REPLModuleContext moduleContext;
    private readonly VariableLookup variableLookup;

    public REPLBlockContext(REPLModuleContext moduleContext, VariableLookup variableLookup,
        List<StructuralError>? errors = null) {
        this.moduleContext = moduleContext;
        this.variableLookup = variableLookup;
        this.AllErrors = errors ?? new List<StructuralError>();
        InferredGenerics = new Dictionary<string, RealizedType>();
        IL = null!;
    }

    public ModuleContext ModuleContext => moduleContext;
    public MethodBuilder MethodBuilder => throw new REPLException("No method builder");
    public IILEmitter IL { get; }
    public TO2Type ExpectedReturn => BuiltinType.Unit;

    public bool IsAsync => false;

    public void AddError(StructuralError error) {
        AllErrors.Add(error);
    }

    public bool HasErrors => AllErrors.Count > 0;

    public List<StructuralError> AllErrors { get; }

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

    public Dictionary<string, RealizedType> InferredGenerics { get; }
}
