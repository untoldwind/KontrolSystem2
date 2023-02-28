using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public static class Helpers {
        public static V Get<K, V>(this IDictionary<K, V> dictionary, K key) {
            return dictionary.TryGetValue(key, out var value) ? value : default;
        }

        public static V GetOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue) {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static (MethodInfo genericMethod, RealizedType genericResult, List<RealizedParameter> genericParameters)
            MakeGeneric(
                IBlockContext context,
                RealizedType declaredResult, List<RealizedParameter> parameters, MethodInfo methodInfo,
                RealizedType desiredResult, IEnumerable<TO2Type> arguments,
                IEnumerable<(string name, RealizedType type)> targetTypeArguments, Node node) {
            if (methodInfo.IsGenericMethod) {
                string[] genericNames = methodInfo.GetGenericArguments().Select(t => t.Name).ToArray();
                IEnumerable<(string name, RealizedType type)> inferred =
                    (declaredResult != null
                        ? declaredResult.InferGenericArgument(context.ModuleContext, desiredResult)
                        : Enumerable.Empty<(string name, RealizedType type)>()).Concat(targetTypeArguments.Concat(
                        parameters.Zip(arguments,
                            (parameter, argument) => parameter.type.InferGenericArgument(context.ModuleContext,
                                argument.UnderlyingType(context.ModuleContext))).SelectMany(t => t)));
                Dictionary<string, RealizedType> inferredDict = new Dictionary<string, RealizedType>();
                foreach (var kv in inferred) {
                    if (inferredDict.ContainsKey(kv.name)) {
                        if (!inferredDict[kv.name].IsAssignableFrom(context.ModuleContext, kv.type))
                            context.AddError(new StructuralError(
                                StructuralError.ErrorType.InvalidType,
                                $"Conflicting types for parameter {kv.name}, found {inferredDict[kv.name]} != {kv.type}",
                                node.Start,
                                node.End
                            ));
                    } else {
                        inferredDict.Add(kv.name, kv.type);
                    }
                }

                foreach (string name in genericNames)
                    if (!inferredDict.ContainsKey(name))
                        context.AddError(new StructuralError(
                            StructuralError.ErrorType.InvalidType,
                            $"Unable to infer generic argument {name} of {methodInfo}",
                            node.Start,
                            node.End
                        ));

                if (context.HasErrors) return (methodInfo, declaredResult, parameters);

                Type[] typeArguments = genericNames
                    .Select(name => inferredDict[name].GeneratedType(context.ModuleContext)).ToArray();
                List<RealizedParameter> genericParams =
                    parameters.Select(p => p.FillGenerics(context.ModuleContext, inferredDict)).ToList();
                RealizedType genericResult = declaredResult?.FillGenerics(context.ModuleContext, inferredDict) ??
                                             throw new ArgumentException($"No declared result for {methodInfo.Name}");

                return (methodInfo.MakeGenericMethod(typeArguments), genericResult, genericParams);
            } else {
                return (methodInfo, declaredResult, parameters);
            }
        }
    }
}
