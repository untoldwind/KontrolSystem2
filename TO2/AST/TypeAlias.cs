using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class TypeAlias(
    bool exported,
    string name,
    string description,
    TO2Type type,
    Position start = new(),
    Position end = new())
    : Node(start, end), IModuleItem {
    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) {
        if (exported) context.exportedTypes!.Add((name, new TypeAliasDelegate(context, type, description, this)));
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
        context.mappedTypes.Add(name, type);
        return [];
    }

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) => [];

    public override REPLValueFuture Eval(REPLContext context) {
        if (context.replModuleContext.mappedTypes.ContainsKey(name))
            throw new REPLException(this, $"Type with name {name} already defined");
        context.replModuleContext.mappedTypes.Add(name, type);

        return REPLValueFuture.Success(REPLUnit.INSTANCE);
    }
}

public class TypeAliasDelegate : TO2Type {
    public readonly TO2Type aliasedType;
    private readonly ModuleContext declaredModule;
    private readonly Node target;
    private bool lookingUp;

    internal TypeAliasDelegate(ModuleContext declaredModule, TO2Type aliasedType, string description, Node target) {
        this.declaredModule = declaredModule;
        this.aliasedType = aliasedType;
        lookingUp = false;
        Description = description;
        this.target = target;
    }

    public override string Description { get; }

    public override string Name => aliasedType.Name;

    public override IFieldAccessFactory? FindField(ModuleContext context, string fieldName) =>
        aliasedType.FindField(declaredModule, fieldName);

    public override IIndexAccessEmitter? AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) =>
        aliasedType.AllowedIndexAccess(declaredModule, indexSpec);

    public override IMethodInvokeFactory? FindMethod(ModuleContext context, string methodName) =>
        aliasedType.FindMethod(declaredModule, methodName);

    public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) =>
        aliasedType.AllowedPrefixOperators(declaredModule);

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) =>
        aliasedType.AllowedSuffixOperators(declaredModule);

    public override IUnapplyEmitter?
        AllowedUnapplyPatterns(ModuleContext context, string unapplyName, int itemCount) =>
        aliasedType.AllowedUnapplyPatterns(context, unapplyName, itemCount);

    public override Type GeneratedType(ModuleContext context) => aliasedType.GeneratedType(declaredModule);

    public override RealizedType UnderlyingType(ModuleContext context) {
        if (lookingUp)
            throw new CompilationErrorException([
                new(
                    StructuralError.ErrorType.InvalidType,
                    $"Cyclic dependency to {aliasedType.Name}",
                    target.Start,
                    target.End
                )
            ]);
        lookingUp = true;
        var realized = aliasedType.UnderlyingType(declaredModule);
        lookingUp = false;
        return realized;
    }
}
