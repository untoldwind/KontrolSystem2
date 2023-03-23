using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.Runtime {
    public class REPLContext {
        public readonly IContext runtimeContext;
        public readonly REPLModuleContext replModuleContext;
        public readonly REPLBlockContext replBlockContext;
        public readonly Dictionary<string, REPLVariable> localVariables = new Dictionary<string, REPLVariable>();
        public readonly VariableResolver externalVariables;

        public REPLContext(IContext runtimeContext, REPLModuleContext replModuleContext = null, VariableResolver externalVariables = null) {
            this.runtimeContext = runtimeContext;
            this.replModuleContext = replModuleContext ?? new REPLModuleContext();
            replBlockContext = new REPLBlockContext(this.replModuleContext);
            this.externalVariables = externalVariables;
        }

        public REPLVariable FindVariable(string name) => localVariables.Get(name) ?? externalVariables?.Invoke(name);

        public REPLVariable DeclaredVariable(string name, bool isConst, TO2Type declaredType) {
            var variable = new REPLVariable(name, isConst, declaredType);

            localVariables.Add(name, variable);

            return variable;
        }

        public REPLContext CreateChildContext() {
            return new REPLContext(runtimeContext, replModuleContext, FindVariable);
        }
        
        public delegate REPLVariable VariableResolver(string name);
        
        public class REPLVariable {
            public readonly string name;
            public readonly bool isConst;
            public readonly TO2Type declaredType;
            public IREPLValue value;

            public REPLVariable(string name, bool isConst, TO2Type declaredType) {
                this.name = name;
                this.isConst = isConst;
                this.declaredType = declaredType;
            }
        }
    }
    
    public class REPLModuleContext : ModuleContext {

    }

    public class REPLBlockContext : IBlockContext {
        private readonly REPLModuleContext moduleContext;
        private readonly List<StructuralError> errors;

        public REPLBlockContext(REPLModuleContext moduleContext) {
            this.moduleContext = moduleContext;
            errors = new List<StructuralError>();
        }

        public ModuleContext ModuleContext => moduleContext;
        public MethodBuilder MethodBuilder => throw new REPLException( "No method builder");
        public IILEmitter IL { get; }
        public TO2Type ExpectedReturn => BuiltinType.Unit;

        public bool IsAsync => false;

        public void AddError(StructuralError error) => errors.Add(error);

        public bool HasErrors => errors.Count > 0;

        public List<StructuralError> AllErrors => errors;
        
        public IBlockContext CreateChildContext() => throw new REPLException( "Child block context");

        public IBlockContext CreateLoopContext(LabelRef start, LabelRef end) =>  throw new REPLException( "Loop block context");

        public IBlockContext CloneCountingContext() => throw new REPLException( "No counting context");

        public (LabelRef start, LabelRef end)? InnerLoop => throw new REPLException( "No inner loop");
        
        public IBlockVariable FindVariable(string name) => throw new REPLException( "No block variables");

        public ITempBlockVariable MakeTempVariable(RealizedType to2Type) => throw new REPLException("No temp block variables");

        public ILocalRef DeclareHiddenLocal(Type rawType) => throw new REPLException("No hidden local");

        public IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type) => throw new REPLException("No declare block variables");

        public void RegisterAsyncResume(TO2Type returnType) => throw new REPLException( "No async resume");
    }
}

