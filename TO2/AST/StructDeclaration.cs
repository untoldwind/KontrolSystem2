using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public readonly struct StructField {
        public readonly string name;
        public readonly TO2Type type;
        public readonly string description;
        public readonly Expression initializer;
        public readonly Position start;
        public readonly Position end;

        public StructField(string name, TO2Type type, string description, Expression initializer,
            Position start = new Position(), Position end = new Position()) {
            this.name = name;
            this.type = type;
            this.description = description;
            this.initializer = initializer;
            this.start = start;
            this.end = end;
        }
    }

    public class StructDeclaration : Node, IModuleItem, IVariableContainer {
        public readonly bool exported;
        public readonly string name;
        public readonly string description;
        public readonly List<FunctionParameter> constructorParameters;
        private readonly List<IEither<LineComment, StructField>> fields;
        public StructTypeAliasDelegate typeDelegate;

        public StructDeclaration(bool exported, string name, string description,
            List<FunctionParameter> constructorParameters, List<IEither<LineComment, StructField>> fields,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.exported = exported;
            this.name = name;
            this.description = description;
            this.constructorParameters = constructorParameters;
            this.fields = fields;
            foreach (var field in fields.Where(e => e.IsRight).Select(e => e.Right))
                field.initializer.VariableContainer = this;
        }

        public IVariableContainer ParentContainer => null;

        public TO2Type FindVariableLocal(IBlockContext context, string variableName) =>
            constructorParameters.Find(p => p.name == variableName)?.type;

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) {
            typeDelegate = new StructTypeAliasDelegate(context, name, description, fields.Where(e => e.IsRight).Select(e => e.Right).ToList());
            if (exported) context.exportedTypes.Add((name, typeDelegate));
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
            if (context.mappedTypes.ContainsKey(name))
                return new StructuralError(
                    StructuralError.ErrorType.DuplicateTypeName,
                    $"Type with name {name} already defined",
                    Start,
                    End
                ).Yield();
            context.mappedTypes.Add(name, typeDelegate);
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
            typeDelegate.EnsureFields();
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public void EmitConstructor(IBlockContext context) {
            foreach (StructField field in fields.Where(e => e.IsRight).Select(e => e.Right)) {
                TO2Type initializerType = field.initializer.ResultType(context);
                if (!field.type.IsAssignableFrom(context.ModuleContext, initializerType)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"Expected item {field.name} of {name} to be a {field.type}, found {initializerType}",
                        Start,
                        End
                    ));
                }
            }

            if (context.HasErrors) return;

            Type type = typeDelegate.GeneratedType(context.ModuleContext);
            IBlockVariable variable =
                context.DeclaredVariable("instance", false, typeDelegate.UnderlyingType(context.ModuleContext));

            variable.EmitLoad(context);

            foreach (StructField field in fields.Where(e => e.IsRight).Select(e => e.Right)) {
                context.IL.Emit(OpCodes.Dup);
                field.initializer.EmitCode(context, false);
                field.type.AssignFrom(context.ModuleContext, field.initializer.ResultType(context))
                    .EmitConvert(context);
                context.IL.Emit(OpCodes.Stfld, type.GetField(field.name));
            }

            context.IL.EmitReturn(type);

            typeDelegate.CreateStructType();
        }
    }

    public class StructTypeAliasDelegate : TO2Type {
        private readonly ModuleContext declaredModule;
        public readonly ModuleContext structContext;
        private readonly List<StructField> fields;
        public override string Name { get; }
        public override string Description { get; }
        private readonly RecordStructType realizedType;
        private bool initialized;

        internal StructTypeAliasDelegate(ModuleContext declaredModule, string name, string description,
            List<StructField> fields) {
            this.declaredModule = declaredModule;
            Name = name;
            Description = description;
            this.fields = fields;
            structContext = declaredModule.DefineSubContext(name, typeof(object));

            ConstructorBuilder constructorBuilder = structContext.typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            IILEmitter constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());
            constructorEmitter.EmitReturn(typeof(void));

            realizedType = new RecordStructType(declaredModule.moduleName, name, description, structContext.typeBuilder,
                Enumerable.Empty<RecordStructField>(),
                new OperatorCollection(),
                new OperatorCollection(),
                new Dictionary<string, IMethodInvokeFactory>(),
                new Dictionary<string, IFieldAccessFactory>(),
                constructorBuilder);
        }

        public override RealizedType UnderlyingType(ModuleContext context) => realizedType;

        public override Type GeneratedType(ModuleContext context) => realizedType.GeneratedType(context);

        public override IMethodInvokeFactory FindMethod(ModuleContext context, string methodName) {
            EnsureFields();
            return realizedType.FindMethod(context, methodName);
        }

        public override IFieldAccessFactory FindField(ModuleContext context, string fieldName) {
            EnsureFields();
            return realizedType.FindField(context, fieldName);
        }

        public void AddMethod(string name, IMethodInvokeFactory methodInvokeFactory) =>
            realizedType.DeclaredMethods.Add(name, methodInvokeFactory);

        internal void EnsureFields() {
            if (initialized) return;

            foreach (var field in fields) {
                RealizedType fieldTO2Type = field.type.UnderlyingType(declaredModule);
                Type fieldType = fieldTO2Type.GeneratedType(declaredModule);

                FieldInfo fieldInfo =
                    structContext.typeBuilder.DefineField(field.name, fieldType, FieldAttributes.Public);

                realizedType.AddField(new RecordStructField(field.name, field.description, fieldTO2Type,
                    fieldInfo));
            }

            initialized = true;
        }

        internal void CreateStructType() {
            realizedType.runtimeType = structContext.typeBuilder.CreateType();
        }
    }
}
