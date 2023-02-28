using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator {
    public class SyncBlockContext : IBlockContext {
        private readonly ModuleContext moduleContext;
        private readonly MethodBuilder methodBuilder;
        private readonly TO2Type expectedReturn;
        private readonly IILEmitter il;
        private readonly List<StructuralError> errors;
        private readonly (LabelRef start, LabelRef end)? innerLoop;
        private VariableResolver externalVariables;
        private readonly Dictionary<string, IBlockVariable> variables;

        private SyncBlockContext(SyncBlockContext parent, IILEmitter il, (LabelRef start, LabelRef end)? innerLoop) {
            moduleContext = parent.ModuleContext;
            methodBuilder = parent.methodBuilder;
            expectedReturn = parent.expectedReturn;
            externalVariables = parent.externalVariables;
            this.il = il;
            variables = parent.variables.ToDictionary(entry => entry.Key, entry => entry.Value);
            errors = parent.errors;
            this.innerLoop = innerLoop;
        }

        public SyncBlockContext(ModuleContext moduleContext) {
            this.moduleContext = moduleContext;
            methodBuilder = null;
            expectedReturn = BuiltinType.Unit;
            il = moduleContext.constructorEmitter;
            variables = new Dictionary<string, IBlockVariable>();
            errors = new List<StructuralError>();
            innerLoop = null;
        }

        protected SyncBlockContext(ModuleContext moduleContext, IILEmitter il) {
            this.moduleContext = moduleContext;
            methodBuilder = null;
            expectedReturn = BuiltinType.Unit;
            this.il = il;
            variables = new Dictionary<string, IBlockVariable>();
            errors = new List<StructuralError>();
            innerLoop = null;
        }

        public SyncBlockContext(ModuleContext moduleContext, FunctionModifier modifier, bool isAsync, string methodName,
            TO2Type returnType, List<FunctionParameter> parameters) {
            this.moduleContext = moduleContext;
            methodBuilder = this.moduleContext.typeBuilder.DefineMethod(methodName,
                modifier == FunctionModifier.Private
                    ? MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static
                    : MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static,
                isAsync
                    ? this.moduleContext.FutureTypeOf(returnType).future
                    : returnType.GeneratedType(this.moduleContext),
                parameters.Select(parameter => parameter.type.GeneratedType(this.moduleContext)).ToArray());
            expectedReturn = returnType;
            il = new GeneratorILEmitter(methodBuilder.GetILGenerator());
            variables = parameters.Select<FunctionParameter, IBlockVariable>((p, idx) =>
                new MethodParameter(p.name, p.type.UnderlyingType(this.moduleContext), idx)).ToDictionary(p => p.Name);
            errors = new List<StructuralError>();
            innerLoop = null;
        }

        // LambdaImpl only!!
        public SyncBlockContext(ModuleContext moduleContext, bool isAsync, string methodName, TO2Type returnType,
            List<FunctionParameter> parameters) {
            this.moduleContext = moduleContext;
            methodBuilder = this.moduleContext.typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public | MethodAttributes.HideBySig,
                isAsync
                    ? this.moduleContext.FutureTypeOf(returnType).future
                    : returnType.GeneratedType(this.moduleContext),
                parameters.Select(parameter => parameter.type.GeneratedType(this.moduleContext)).ToArray());
            expectedReturn = returnType;
            il = new GeneratorILEmitter(methodBuilder.GetILGenerator());
            variables = parameters.Select<FunctionParameter, IBlockVariable>((p, idx) =>
                    new MethodParameter(p.name, p.type.UnderlyingType(this.moduleContext), idx + 1))
                .ToDictionary(p => p.Name);
            errors = new List<StructuralError>();
            innerLoop = null;
        }

        // Struct methods only
        public SyncBlockContext(StructTypeAliasDelegate structType, bool isConst, bool isAsync, string methodName,
            TO2Type returnType,
            List<FunctionParameter> parameters) : this(structType.structContext, isAsync, methodName, returnType,
            parameters) {
            variables.Add("self",
                new MethodParameter("self", structType.UnderlyingType(structType.structContext), 0, isConst));
        }

        public ModuleContext ModuleContext => moduleContext;

        public MethodBuilder MethodBuilder => methodBuilder;

        public IILEmitter IL => il;

        public TO2Type ExpectedReturn => expectedReturn;

        public bool IsAsync => false;

        public void AddError(StructuralError error) => errors.Add(error);

        public bool HasErrors => errors.Count > 0;

        public List<StructuralError> AllErrors => errors;

        public (LabelRef start, LabelRef end)? InnerLoop => innerLoop;

        public IBlockContext CreateChildContext() => new SyncBlockContext(this, IL, innerLoop);

        public IBlockContext CreateLoopContext(LabelRef start, LabelRef end) =>
            new SyncBlockContext(this, IL, (start, end));

        public IBlockContext CloneCountingContext() =>
            new SyncBlockContext(this, new CountingILEmitter(IL.LastLocalIndex), innerLoop);

        public ITempBlockVariable MakeTempVariable(RealizedType to2Type) {
            Type type = to2Type.GeneratedType(moduleContext);
            ITempLocalRef localRef = IL.TempLocal(type);

            TempVariable variable = new TempVariable(to2Type, localRef);

            to2Type.EmitInitialize(this, variable);
            return variable;
        }

        public VariableResolver ExternVariables {
            set => externalVariables = value;
        }

        public IBlockVariable FindVariable(string name) => variables.Get(name) ?? externalVariables?.Invoke(name);

        public ILocalRef DeclareHiddenLocal(Type rawType) => il.DeclareLocal(rawType);

        public IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type) {
            ILocalRef localRef = IL.DeclareLocal(to2Type.GeneratedType(moduleContext));
            DeclaredVariable variable = new DeclaredVariable(name, isConst, to2Type, localRef);

            to2Type.EmitInitialize(this, variable);
            variables.Add(name, variable);

            return variable;
        }

        public void RegisterAsyncResume(TO2Type returnType) {
        }
    }
}
