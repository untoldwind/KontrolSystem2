using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class LookupTypeReference : TO2Type {
    private readonly Position end;
    private readonly string? moduleName;
    private readonly string name;
    private readonly Position start;
    private readonly List<TO2Type> typeArguments;

    public LookupTypeReference(List<string> namePath, List<TO2Type> typeArguments, Position start, Position end) {
        if (namePath.Count > 1) {
            moduleName = string.Join("::", namePath.Take(namePath.Count - 1));
            name = namePath.Last();
        } else {
            moduleName = null;
            name = namePath.Last();
        }

        this.typeArguments = typeArguments;
        this.start = start;
        this.end = end;
    }

    public override string Name {
        get {
            var builder = new StringBuilder();

            if (moduleName != null) {
                builder.Append(moduleName);
                builder.Append("::");
            }

            builder.Append(name);
            if (typeArguments.Count > 0) {
                builder.Append("<");
                builder.Append(string.Join(",", typeArguments.Select(t => t.Name)));
                builder.Append(">");
            }

            return builder.ToString();
        }
    }

    public override bool IsValid(ModuleContext context) {
        return ReferencedType(context) != null;
    }

    public override RealizedType UnderlyingType(ModuleContext context) {
        return ReferencedType(context);
    }

    public override Type GeneratedType(ModuleContext context) {
        return ReferencedType(context).GeneratedType(context);
    }

    public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) {
        return ReferencedType(context).AllowedPrefixOperators(context);
    }

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) {
        return ReferencedType(context).AllowedSuffixOperators(context);
    }

    public override IMethodInvokeFactory? FindMethod(ModuleContext context, string methodName) {
        return ReferencedType(context).FindMethod(context, methodName);
    }

    public override IFieldAccessFactory? FindField(ModuleContext context, string fieldName) {
        return ReferencedType(context).FindField(context, fieldName);
    }

    public override IIndexAccessEmitter? AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) {
        return ReferencedType(context).AllowedIndexAccess(context, indexSpec);
    }

    public override IUnapplyEmitter?
        AllowedUnapplyPatterns(ModuleContext context, string unapplyName, int itemCount) {
        return ReferencedType(context).AllowedUnapplyPatterns(context, unapplyName, itemCount);
    }

    public override IForInSource? ForInSource(ModuleContext context, TO2Type? typeHint) {
        return ReferencedType(context).ForInSource(context, typeHint);
    }

    public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
        return ReferencedType(context).IsAssignableFrom(context, otherType);
    }

    public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
        return ReferencedType(context).AssignFrom(context, otherType);
    }

    private RealizedType ReferencedType(ModuleContext context) {
        var realizedType = moduleName != null
            ? context.FindModule(moduleName)?.FindType(name)?.UnderlyingType(context)
            : context.mappedTypes.Get(name)?.UnderlyingType(context);
        if (realizedType == null)
            throw new CompilationErrorException(new List<StructuralError> {
                new(
                    StructuralError.ErrorType.InvalidType,
                    $"Unable to lookup type {Name}",
                    start,
                    end
                )
            });

        var typeParameterNames = realizedType.GenericParameters;
        if (typeParameterNames.Length != typeArguments.Count)
            throw new CompilationErrorException(new List<StructuralError> {
                new(
                    StructuralError.ErrorType.InvalidType,
                    $"Type {realizedType.Name} expects {typeParameterNames.Length} type parameters, only {typeArguments.Count} where given",
                    start,
                    end
                )
            });

        var namedTypeArguments = new Dictionary<string, RealizedType>();
        for (var i = 0; i < typeArguments.Count; i++)
            namedTypeArguments.Add(typeParameterNames[i].Name, typeArguments[i].UnderlyingType(context));

        return realizedType.FillGenerics(context, namedTypeArguments);
    }
}

public class DirectTypeReference : RealizedType {
    private readonly List<TO2Type> declaredTypeArguments;
    private readonly RealizedType referencedType;

    public DirectTypeReference(RealizedType referencedType, List<TO2Type> declaredTypeArguments) {
        this.referencedType = referencedType;
        this.declaredTypeArguments = declaredTypeArguments;
    }


    public override string Name => referencedType.Name;

    public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => referencedType.DeclaredMethods;

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields => referencedType.DeclaredFields;

    public override TO2Type[] GenericParameters => declaredTypeArguments.ToArray();

    public override Type GeneratedType(ModuleContext context) {
        return UnderlyingType(context).GeneratedType(context);
    }

    public override RealizedType UnderlyingType(ModuleContext context) {
        var arguments = referencedType.GenericParameters
            .Zip(declaredTypeArguments, (name, type) => (name, type.UnderlyingType(context)))
            .ToDictionary(i => i.Item1.Name, i => i.Item2);

        return referencedType.FillGenerics(context, arguments);
    }

    public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) {
        return UnderlyingType(context).AllowedPrefixOperators(context);
    }

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) {
        return UnderlyingType(context).AllowedSuffixOperators(context);
    }

    public override IMethodInvokeFactory? FindMethod(ModuleContext context, string methodName) {
        return UnderlyingType(context).FindMethod(context, methodName);
    }

    public override IFieldAccessFactory? FindField(ModuleContext context, string fieldName) {
        return UnderlyingType(context).FindField(context, fieldName);
    }

    public override RealizedType
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType>? typeArguments) {
        return UnderlyingType(context).FillGenerics(context, typeArguments);
    }

    public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
        return UnderlyingType(context).IsAssignableFrom(context, otherType);
    }

    public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
        return UnderlyingType(context).AssignFrom(context, otherType);
    }

    public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
        RealizedType? concreteType) {
        return UnderlyingType(context).InferGenericArgument(context, concreteType);
    }
}
