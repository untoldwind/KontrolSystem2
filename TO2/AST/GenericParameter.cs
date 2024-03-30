using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class GenericParameter(string name) : RealizedType {
    public override string Name => name;

    public override Type GeneratedType(ModuleContext context) => typeof(object);

    public override RealizedType UnderlyingType(ModuleContext context) => this;

    public override RealizedType FillGenerics(ModuleContext context,
        Dictionary<string, RealizedType>? typeArguments) {
        return typeArguments?.Get(name) ?? this;
    }

    public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
        RealizedType? concreteType) {
        return concreteType != null
            ? (name, concreteType).Yield()
            : [];
    }
}
