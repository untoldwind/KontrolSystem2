using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class BoundType(string? modulePrefix, string localName, string? description, Type runtimeType,
    OperatorCollection allowedPrefixOperators,
    OperatorCollection allowedSuffixOperators,
    IEnumerable<(string name, IMethodInvokeFactory invoker)> allowedMethods,
    IEnumerable<(string name, IFieldAccessFactory access)> allowedFields,
    IEnumerable<RealizedType>? typeParameters = null,
    Func<IndexSpec, IIndexAccessEmitter?>? indexAccessEmitterFactory = null,
    IForInSource? forInSource = null) : RealizedType {
    public readonly Dictionary<string, IFieldAccessFactory> allowedFields = allowedFields.ToDictionary(m => m.name, m => m.access);
    public readonly Dictionary<string, IMethodInvokeFactory> allowedMethods = allowedMethods.ToDictionary(m => m.name, m => m.invoker);
    public readonly string localName = localName;
    public readonly Type runtimeType = runtimeType;
    private readonly IEnumerable<RealizedType> typeParameters = typeParameters ??
                              runtimeType.GetGenericArguments().Select(t => new GenericParameter(t.Name));

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

    public override bool IsValid(ModuleContext context) => !runtimeType.IsGenericTypeDefinition;

    public override RealizedType UnderlyingType(ModuleContext context) => this;

    public override Type GeneratedType(ModuleContext context) => runtimeType;

    public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

    public override IIndexAccessEmitter? AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) =>
        indexAccessEmitterFactory?.Invoke(indexSpec);

    public override IForInSource? ForInSource(ModuleContext context, TO2Type? typeHint) => forInSource;

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
                .Zip(filled, (o, t) => (o.Name, t)).ToDictionary(i => i.Name, i => i.t);

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
        if (!runtimeType.IsGenericType) return [];

        var otherBoundType = concreteType as BoundType;
        if (otherBoundType == null || otherBoundType.runtimeType.GetGenericTypeDefinition() != runtimeType)
            return [];

        return typeParameters.Zip(otherBoundType.typeParameters, (t, o) => t.InferGenericArgument(context, o))
            .SelectMany(t => t);
    }
}
