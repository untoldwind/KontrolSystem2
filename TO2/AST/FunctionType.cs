using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class FunctionType : RealizedType {
        public readonly bool isAsync;
        public readonly List<TO2Type> parameterTypes;
        public readonly TO2Type returnType;
        private Type generatedType;

        public FunctionType(bool isAsync, List<TO2Type> parameterTypes, TO2Type returnType) {
            this.isAsync = isAsync;
            this.parameterTypes = parameterTypes;
            this.returnType = returnType;
        }

        public override string Name => $"fn({String.Join(", ", parameterTypes)}) -> {returnType}";

        public override bool IsValid(ModuleContext context) =>
            returnType.IsValid(context) && parameterTypes.All(t => t.IsValid(context));

        public override RealizedType UnderlyingType(ModuleContext context) => new FunctionType(isAsync,
            parameterTypes.Select(p => p.UnderlyingType(context) as TO2Type).ToList(),
            returnType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) {
            if (generatedType == null) {
                generatedType =
                    (Type.GetType($"System.Func`{parameterTypes.Count + 1}") ??
                     throw new ArgumentException($"No type System.Func`{parameterTypes.Count + 1}")).MakeGenericType(
                        parameterTypes
                            .Concat(returnType.Yield()).Select(p => p.GeneratedType(context)).ToArray());
            }

            return generatedType;
        }

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuiltinType.NoOperators;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => BuiltinType.NoOperators;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => BuiltinType.NoMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => BuiltinType.NoFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        public override RealizedType
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            new FunctionType(isAsync,
                parameterTypes.Select(t => t.UnderlyingType(context).FillGenerics(context, typeArguments) as TO2Type)
                    .ToList(), returnType.UnderlyingType(context).FillGenerics(context, typeArguments));

        public override IEnumerable<(string name, RealizedType type)> InferGenericArgument(ModuleContext context,
            RealizedType concreteType) {
            FunctionType concreteFunction = concreteType as FunctionType;
            if (concreteFunction == null) return Enumerable.Empty<(string name, RealizedType type)>();
            return returnType.InferGenericArgument(context, concreteFunction.returnType.UnderlyingType(context)).Concat(
                parameterTypes.Zip(concreteFunction.parameterTypes,
                        (p, concreteP) => p.InferGenericArgument(context, concreteP.UnderlyingType(context)))
                    .SelectMany(p => p));
        }
    }
}
