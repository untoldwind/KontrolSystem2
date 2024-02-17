using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class BoundType : RealizedType {
    public readonly Dictionary<string, IFieldAccessFactory> allowedFields;
    public readonly Dictionary<string, IMethodInvokeFactory> allowedMethods;
    private readonly OperatorCollection allowedPrefixOperators;
    private readonly OperatorCollection allowedSuffixOperators;
    private readonly string? description;
    public readonly string localName;
    private readonly string? modulePrefix;
    public readonly Type runtimeType;
    private readonly IEnumerable<RealizedType> typeParameters;

    public BoundType(string? modulePrefix, string localName, string? description, Type runtimeType,
        OperatorCollection allowedPrefixOperators,
        OperatorCollection allowedSuffixOperators,
        IEnumerable<(string name, IMethodInvokeFactory invoker)> allowedMethods,
        IEnumerable<(string name, IFieldAccessFactory access)> allowedFields,
        IEnumerable<RealizedType>? typeParameters = null) {
        this.modulePrefix = modulePrefix;
        this.localName = localName;
        this.description = description;
        this.runtimeType = runtimeType;
        this.allowedPrefixOperators = allowedPrefixOperators;
        this.allowedSuffixOperators = allowedSuffixOperators;
        this.allowedMethods = allowedMethods.ToDictionary(m => m.name, m => m.invoker);
        this.allowedFields = allowedFields.ToDictionary(m => m.name, m => m.access);
        this.typeParameters = typeParameters ??
                              runtimeType.GetGenericArguments().Select(t => new GenericParameter(t.Name));
    }

    public override string Name {
        get {
            var builder = new StringBuilder();

            if (modulePrefix != null) {
                builder.Append(modulePrefix);
                builder.Append("::");
            }

            builder.Append(localName);
            if (typeParameters.Any()) {
                builder.Append("<");
                builder.Append(string.Join(",", typeParameters.Select(t => t.Name)));
                builder.Append(">");
            }

            return builder.ToString();
        }
    }

    public override string Description => description ?? "";

    public override string LocalName => localName;

    public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

    public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

    public override TO2Type[] GenericParameters => typeParameters.ToArray<TO2Type>();

    public override bool IsValid(ModuleContext context) {
        return !runtimeType.IsGenericTypeDefinition;
    }

    public override RealizedType UnderlyingType(ModuleContext context) {
        return this;
    }

    public override Type GeneratedType(ModuleContext context) {
        return runtimeType;
    }

    public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) {
        return allowedPrefixOperators;
    }

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) {
        return allowedSuffixOperators;
    }

    public override RealizedType
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType>? typeArguments) {
        if (runtimeType.IsGenericType) {
            var filled = typeParameters.Select(t => t.FillGenerics(context, typeArguments)).ToList();

            if (filled.Any(t => t is GenericParameter))
                return new BoundType(modulePrefix, localName, description, runtimeType,
                    allowedPrefixOperators,
                    allowedSuffixOperators,
                    allowedMethods.Select(m => (m.Key, m.Value)),
                    allowedFields.Select(f => (f.Key, f.Value)),
                    filled);

            var arguments = filled.Select(t => t.GeneratedType(context)).ToArray();
            var originalTypeArguments = runtimeType.GetGenericArguments()
                .Zip(filled, (o, t) => (o.Name, t)).ToDictionary(i => i.Item1, i => i.Item2);

            return new BoundType(modulePrefix, localName, description, runtimeType.MakeGenericType(arguments),
                allowedPrefixOperators.FillGenerics(context, originalTypeArguments),
                allowedSuffixOperators.FillGenerics(context, originalTypeArguments),
                allowedMethods.Select(m => (m.Key, m.Value.FillGenerics(context, originalTypeArguments))),
                allowedFields.Select(f => (f.Key, f.Value.FillGenerics(context, originalTypeArguments))),
                filled);
        }

        return this;
    }

    public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
        RealizedType? concreteType) {
        if (!runtimeType.IsGenericType) return Enumerable.Empty<(string name, RealizedType type)>();

        var otherBoundType = concreteType as BoundType;
        if (otherBoundType == null || otherBoundType.runtimeType.GetGenericTypeDefinition() != runtimeType)
            return Enumerable.Empty<(string name, RealizedType type)>();

        return typeParameters.Zip(otherBoundType.typeParameters, (t, o) => t.InferGenericArgument(context, o))
            .SelectMany(t => t);
    }
}
