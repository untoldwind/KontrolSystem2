using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Binding;

public static class BindingGenerator {
    private static readonly Dictionary<Type, RealizedType> TypeMappings = new() {
        { typeof(bool), BuiltinType.Bool },
        { typeof(long), BuiltinType.Int },
        { typeof(double), BuiltinType.Float },
        { typeof(string), BuiltinType.String },
        { typeof(void), BuiltinType.Unit },
        { typeof(Cell<>), BuiltinType.Cell }
    };

    private static readonly Dictionary<Type, CompiledKontrolModule> BoundModules = [];

    public static CompiledKontrolModule BindModule(Type moduleType, IEnumerable<RealizedType>? additionalTypes = null,
        IEnumerable<IKontrolConstant>? additionConstants = null) {
        lock (BoundModules) {
            if (BoundModules.TryGetValue(moduleType, out var bindModule)) return bindModule;

            var ksModule = moduleType.GetCustomAttribute<KSModule>();

            if (ksModule == null) throw new ArgumentException($"Type {moduleType} must have a kSClass attribute");

            var runtimeType = moduleType;
            var functions = new List<CompiledKontrolFunction>();
            var types = new List<RealizedType>(additionalTypes ?? []);
            var constants =
                new List<IKontrolConstant>(additionConstants ?? Enumerable.Empty<CompiledKontrolConstant>());

            while (runtimeType != null && runtimeType != typeof(object)) {
                foreach (var nested in runtimeType.GetNestedTypes(BindingFlags.Public))
                    if (nested.GetCustomAttribute<KSClass>() != null)
                        types.Add(BindType(ksModule.Name, nested));

                foreach (var type in types)
                    if (type is BoundType boundType)
                        LinkType(boundType);

                foreach (var field in runtimeType.GetFields(BindingFlags.Public | BindingFlags.Static)) {
                    var ksConstant = field.GetCustomAttribute<KSConstant>();
                    if (ksConstant == null) continue;

                    TO2Type to2Type = MapNativeType(field.FieldType);

                    constants.Add(new CompiledKontrolConstant(
                        ksConstant.Name ?? ToSnakeCase(field.Name).ToUpperInvariant(),
                        NormalizeDescription(ksConstant.Description), to2Type, field));
                }

                foreach (var method in runtimeType.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                    var ksFunction = method.GetCustomAttribute<KSFunction>();
                    if (ksFunction == null) continue;

                    var parameters = method.GetParameters().Select(p => {
                        var ksParameter = p.GetCustomAttribute<KSParameter>();
                        return new RealizedParameter(p.Name, MapNativeType(p.ParameterType),
                            ksParameter?.Description, BoundDefaultValue.DefaultValueFor(p));
                    }).ToList();
                    if (method.ReturnType.IsGenericType &&
                        method.ReturnType.GetGenericTypeDefinition() == typeof(Future<>)) {
                        var typeArg = method.ReturnType.GetGenericArguments()[0];
                        var resultType =
                            typeArg == typeof(object) ? BuiltinType.Unit : MapNativeType(typeArg);
                        functions.Add(new CompiledKontrolFunction(ksFunction.Name ?? ToSnakeCase(method.Name),
                            NormalizeDescription(ksFunction.Description), true, parameters, resultType, method));
                    } else {
                        var resultType = MapNativeType(method.ReturnType);
                        functions.Add(new CompiledKontrolFunction(ksFunction.Name ?? ToSnakeCase(method.Name),
                            NormalizeDescription(ksFunction.Description), false, parameters, resultType, method));
                    }
                }

                runtimeType = runtimeType.BaseType;
            }

            var module = new CompiledKontrolModule(ksModule.Name,
                NormalizeDescription(ksModule.Description), true, null, types.Select(t => (t.LocalName, t)),
                constants, functions, []);
            BoundModules.Add(moduleType, module);
            return module;
        }
    }

    private static BoundType BindType(string modulePrefix, Type type) {
        var ksClass = type.GetCustomAttribute<KSClass>();

        if (ksClass == null) throw new ArgumentException($"Type {type} must have a kSClass attribute");

        var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
        var isArrayLike = typeof(IArrayLike).IsAssignableFrom(type);
        var boundType = new BoundType(modulePrefix, ksClass.Name ?? type.Name,
            NormalizeDescription(ksClass.Description), type,
            allowedPrefixOperators: BuiltinType.NoOperators,
            allowedSuffixOperators: BuiltinType.NoOperators,
            allowedMethods: isEnumerable ? EnumerableMethods.MethodInvokers(type) : [],
            allowedFields: isArrayLike ? [
                (name: "length", access: new BoundPropertyLikeFieldAccessFactory("length", () => BuiltinType.Int, type, type.GetProperty("Length")))] : [],
            indexAccessEmitterFactory: isArrayLike ? indexSpec => indexSpec.indexType switch {
                IndexSpecType.Single => new BoundArrayLikeIndexAccess(type, indexSpec.start),
                _ => null,
            } : null,
            forInSource: isEnumerable ? new BoundEnumerableForInSource(type) : null
        );
        RegisterTypeMapping(type, boundType);
        return boundType;
    }

    private static void LinkType(BoundType boundType) {
        var interfaces = boundType.runtimeType.GetCustomAttribute<KSClass>().ScanInterfaces ?? [];
        foreach (var method in boundType.runtimeType.GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
            var ksMethod = method.GetCustomAttribute<KSMethod>() ?? interfaces.Select(i => i.GetMethod(method.Name)?.GetCustomAttribute<KSMethod>()).FirstOrDefault(attr => attr != null);
            if (ksMethod == null) continue;
            boundType.allowedMethods.Add(ksMethod.Name ?? ToSnakeCase(method.Name),
                BindMethod(NormalizeDescription(ksMethod.Description), boundType.runtimeType, method));
        }

        foreach (var property in boundType.runtimeType.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
            var ksField = property.GetCustomAttribute<KSField>() ?? interfaces.Select(i => i.GetProperty(property.Name)?.GetCustomAttribute<KSField>()).FirstOrDefault(attr => attr != null); ;
            if (ksField == null) continue;

            boundType.allowedFields.Add(ksField.Name ?? ToSnakeCase(property.Name),
                BindProperty(NormalizeDescription(ksField.Description), boundType.runtimeType, property,
                    ksField.IsAsyncStore));
        }
    }

    private static IMethodInvokeFactory BindMethod(string? description, Type type, MethodInfo method) {
        var parameters = method.GetParameters().Select(p => {
            var ksParameter = p.GetCustomAttribute<KSParameter>();
            return new RealizedParameter(p.Name, MapNativeType(p.ParameterType), ksParameter?.Description,
                BoundDefaultValue.DefaultValueFor(p));
        })
            .ToList();
        if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Future<>)) {
            var typeArg = method.ReturnType.GetGenericArguments()[0];
            var resultType = typeArg == typeof(object) ? BuiltinType.Unit : MapNativeType(typeArg);
            return new BoundMethodInvokeFactory(description, true, () => resultType, () => parameters, true,
                type,
                method);
        } else {
            var resultType = MapNativeType(method.ReturnType);
            return new BoundMethodInvokeFactory(description, true, () => resultType, () => parameters, false,
                type,
                method);
        }
    }

    private static IFieldAccessFactory BindProperty(string? description, Type type, PropertyInfo property,
        bool isAsyncStore) {
        var result = MapNativeType(property.PropertyType);

        return new BoundPropertyLikeFieldAccessFactory(description, () => result, type, property, isAsyncStore);
    }

    public static void RegisterTypeMapping(Type type, RealizedType to2Type) {
        lock (TypeMappings) {
            if (TypeMappings.ContainsKey(type))
                TypeMappings[type] = to2Type;
            else
                TypeMappings.Add(type, to2Type);
        }
    }

    public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) RegisterEnumTypeMappings(
        string modulePrefix,
        IEnumerable<(string localName, string description, Type enumType, (Enum value, string description)[]
            valueDescriptions)> enums) {
        var types = new List<RealizedType>();
        var constants = new List<IKontrolConstant>();

        foreach (var (localName, description, enumType, valueDescriptions) in enums) {
            var boundEnumType = new BoundEnumType(modulePrefix, localName, description, enumType);

            RegisterTypeMapping(enumType, boundEnumType);
            types.Add(boundEnumType);
            var boundEnumConstants = new BoundEnumConstType(boundEnumType, valueDescriptions);
            types.Add(boundEnumConstants);
            constants.Add(new EnumKontrolConstant(localName, boundEnumConstants, description));
        }

        return (types, constants);
    }

    internal static RealizedType MapNativeType(Type type) {
        lock (TypeMappings) {
            if (type.IsGenericParameter) return new GenericParameter(type.Name);
            if (type.IsGenericType) {
                var baseType = type.GetGenericTypeDefinition();
                var typeArgs = type.GetGenericArguments();

                if (baseType == typeof(Option<>)) {
                    TO2Type innerType = typeArgs[0] == typeof(object)
                        ? BuiltinType.Unit
                        : MapNativeType(typeArgs[0]);

                    return new OptionType(innerType);
                }

                if (baseType == typeof(Result<>)) {
                    TO2Type successType = typeArgs[0] == typeof(object)
                        ? BuiltinType.Unit
                        : MapNativeType(typeArgs[0]);

                    return new ResultType(successType);
                }

                if (baseType.FullName!.StartsWith("System.Func")) {
                    var parameterTypes = typeArgs.Take(typeArgs.Length - 1)
                        .Select(t => MapNativeType(t) as TO2Type).ToList();
                    TO2Type returnType = MapNativeType(typeArgs[^1]);
                    return new FunctionType(false, parameterTypes, returnType);
                }

                if (baseType.FullName.StartsWith("System.Action")) {
                    var parameterTypes = typeArgs.Select(t => MapNativeType(t) as TO2Type).ToList();
                    return new FunctionType(false, parameterTypes, BuiltinType.Unit);
                }

                var to2BaseType = TypeMappings.Get(baseType);
                if (to2BaseType != null) {
                    var typeArguments = typeArgs.Select(MapNativeType).Cast<TO2Type>().ToList();
                    return new DirectTypeReference(to2BaseType, typeArguments);
                }
            } else if (type.IsArray) {
                TO2Type elementType = MapNativeType(type.GetElementType());

                return new ArrayType(elementType);
            } else if (type == typeof(Action)) {
                return new FunctionType(false, [], BuiltinType.Unit);
            }

            var t = TypeMappings.Get(type);
            if (t != null) return t;
            throw new ArgumentException($"No mapping for {type}");
        }
    }

    public static string ToSnakeCase(string name) {
        return name.All(ch => char.IsUpper(ch))
            ? name
            : string
                .Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }

    private static string? NormalizeDescription(string? description) {
        if (description == null) return null;

        var sb = new StringBuilder();

        foreach (var line in description.Split('\n')) {
            sb.Append(line.Trim());
            sb.Append("\n");
        }

        return sb.ToString();
    }
}
