using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public readonly struct StructField(
    string name,
    TO2Type type,
    string description,
    Expression initializer,
    Position start = new(),
    Position end = new()) {
    public readonly string name = name;
    public readonly TO2Type type = type;
    public readonly string description = description;
    public readonly Expression initializer = initializer;
    public readonly Position start = start;
    public readonly Position end = end;
}

public class StructDeclaration : Node, IModuleItem, IVariableContainer {
    public readonly List<FunctionParameter> constructorParameters;
    public readonly string description;
    public readonly bool exported;
    private readonly List<IEither<LineComment, StructField>> fields;
    public readonly string name;
    public StructTypeAliasDelegate? typeDelegate;

    public StructDeclaration(bool exported, string name, string description,
        List<FunctionParameter> constructorParameters, List<IEither<LineComment, StructField>> fields,
        Position start = new(), Position end = new()) : base(start, end) {
        this.exported = exported;
        this.name = name;
        this.description = description;
        this.constructorParameters = constructorParameters;
        this.fields = fields;
        foreach (var field in fields.Where(e => e.IsRight).Select(e => e.Right)) {
            field.initializer.VariableContainer = this;
            field.initializer.TypeHint = context => field.type.UnderlyingType(context.ModuleContext);
        }
    }

    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) {
        typeDelegate = new StructTypeAliasDelegate(context, name, description,
            fields.Where(e => e.IsRight).Select(e => e.Right).ToList());
        if (exported) context.exportedTypes!.Add((name, typeDelegate));
        return [];
    }

    public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
        if (context.mappedTypes.ContainsKey(name))
            return new StructuralError(
                StructuralError.ErrorType.DuplicateTypeName,
                $"Type with name {name} already defined",
                Start,
                End
            ).Yield();
        context.mappedTypes.Add(name, typeDelegate!);
        return [];
    }

    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
        typeDelegate!.EnsureFields();
        return [];
    }

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => [];

    public IVariableContainer? ParentContainer => null;

    public TO2Type? FindVariableLocal(IBlockContext context, string variableName) =>
        constructorParameters.Find(p => p.name == variableName)?.type;

    public void EmitConstructor(IBlockContext context) {
        foreach (var field in fields.Where(e => e.IsRight).Select(e => e.Right)) {
            var initializerType = field.initializer.ResultType(context);
            if (!field.type.IsAssignableFrom(context.ModuleContext, initializerType))
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Expected item {field.name} of {name} to be a {field.type}, found {initializerType}",
                    Start,
                    End
                ));
        }

        // Note: Somewhat crude way to ensure that all field initializes do not have structural errors
        foreach (var field in fields.Where(e => e.IsRight).Select(e => e.Right))
            field.initializer.GetILCount(context, false);

        if (context.HasErrors) return;

        typeDelegate!.EnsureFields();
        typeDelegate.CreateStructType();

        var type = typeDelegate.GeneratedType(context.ModuleContext);
        var variable =
            context.DeclaredVariable("instance", false, typeDelegate.UnderlyingType(context.ModuleContext));

        variable.EmitLoad(context);

        foreach (var field in fields.Where(e => e.IsRight).Select(e => e.Right)) {
            context.IL.Emit(OpCodes.Dup);
            field.initializer.EmitCode(context, false);
            field.type.AssignFrom(context.ModuleContext, field.initializer.ResultType(context))
                .EmitConvert(context, false);
            context.IL.Emit(OpCodes.Stfld, type.GetField(field.name));
        }

        context.IL.EmitReturn(type);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        throw new NotSupportedException("Structs are not supported in REPL mode");
    }
}

public class StructTypeAliasDelegate : TO2Type {
    public readonly ModuleContext declaredModule;
    private readonly List<StructField> fields;
    private readonly RecordStructType realizedType;
    public readonly ModuleContext structContext;
    private bool creating;
    private bool fieldsCreated;

    internal StructTypeAliasDelegate(ModuleContext declaredModule, string name, string description,
        List<StructField> fields) {
        this.declaredModule = declaredModule;
        Name = name;
        Description = description;
        this.fields = fields;
        structContext = declaredModule.DefineSiblingContext(name, typeof(object));

        var constructorBuilder = structContext.typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

        realizedType = new RecordStructType(declaredModule.moduleName, name, description, structContext.typeBuilder,
            [],
            [],
            [],
            [],
            [],
            constructorBuilder, GeneratedType);
    }

    public override string Name { get; }
    public override string Description { get; }

    public override RealizedType UnderlyingType(ModuleContext context) {
        return realizedType;
    }

    public override Type GeneratedType(ModuleContext context) {
        EnsureFields();
        CreateStructType();
        return realizedType.runtimeType;
    }

    public override IMethodInvokeFactory? FindMethod(ModuleContext context, string methodName) {
        EnsureFields();
        return realizedType.FindMethod(context, methodName);
    }

    public override IFieldAccessFactory? FindField(ModuleContext context, string fieldName) {
        EnsureFields();
        return realizedType.FindField(context, fieldName);
    }

    public void AddMethod(string name, IMethodInvokeFactory methodInvokeFactory) {
        realizedType.DeclaredMethods.Add(name, methodInvokeFactory);
    }

    internal void EnsureFields() {
        if (fieldsCreated || creating) return;

        creating = true;
        foreach (var field in fields) {
            var fieldTO2Type = field.type.UnderlyingType(declaredModule);
            var fieldType = fieldTO2Type.GeneratedType(declaredModule);

            FieldInfo fieldInfo =
                structContext.typeBuilder.DefineField(field.name, fieldType, FieldAttributes.Public);

            realizedType.AddField(new RecordStructField(field.name, field.description, fieldTO2Type,
                fieldInfo));
        }

        creating = false;
        fieldsCreated = true;
    }

    internal void CreateStructType() {
        if (creating) return;

        realizedType.runtimeType = structContext.typeBuilder.CreateType();
    }
}
