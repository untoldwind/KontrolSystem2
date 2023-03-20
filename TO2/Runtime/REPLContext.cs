using System.Collections.Generic;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Runtime {
    public class REPLContext {
        public readonly IContext runtimeContext;
        public readonly REPLModuleContext replModuleContext = new REPLModuleContext();
        public readonly Dictionary<string, REPLVariable> localVariables = new Dictionary<string, REPLVariable>();

        public REPLContext(IContext runtimeContext) {
            this.runtimeContext = runtimeContext;
        }

        public REPLVariable FindVariable(string name) => localVariables.Get(name);

        public REPLVariable DeclaredVariable(string name, bool isConst, TO2Type declaredType) {
            var variable = new REPLVariable(name, isConst, declaredType);
            
            localVariables.Add(name, variable);

            return variable;
        } 
        
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
}
