using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KontrolSystem.GenRefs;

class MainClass {
    public static void Main(string[] args) {
        var registry = KontrolSystemKSPRegistry.CreateKSP();

        var context = registry.AddDirectory(Path.Combine(Directory.GetCurrentDirectory(), "KSP2Runtime", "to2"));

        var reference = GenerateReference(context.CreateModuleContext("reference"), registry);

        var vsCodePath = Path.Combine(Directory.GetCurrentDirectory(), "Tools", "vscode", "to2-syntax", "server", "src", "reference", "reference.json");

        File.WriteAllText(vsCodePath, JsonConvert.SerializeObject(reference, Formatting.Indented));

        var modPath = Path.Combine(Directory.GetCurrentDirectory(), "SpaceWarpMod", "reference.json");

        File.WriteAllText(modPath, JsonConvert.SerializeObject(reference, Formatting.Indented));
    }

    public static Reference GenerateReference(ModuleContext moduleContext, KontrolRegistry registry) =>
        new(moduleContext, registry);
}

public class Reference {
    public Reference(ModuleContext moduleContext, KontrolRegistry registry) {
        Builtin = [];
        Modules = [];

        foreach (var type in new List<RealizedType> {
                     BuiltinType.Unit, BuiltinType.Bool, BuiltinType.Int,
                     BuiltinType.Float, BuiltinType.String, BuiltinType.Range,
                     BuiltinType.ArrayBuilder, BuiltinType.Cell, new OptionType(new GenericParameter("T")),
                     new ResultType(new GenericParameter("R"))
                 }) {
            Builtin.Add(type.LocalName, new TypeReference(moduleContext, type, type.LocalName));
        }

        foreach (var module in registry.modules.Values) {
            if (IsModuleEmpty(module) || !module.Name.Contains("::")) continue;
            var moduleReference = new ModuleReference(moduleContext, module);
            Modules.Add(moduleReference.Name, moduleReference);
        }

        var allTypes = Builtin.Values.Concat(Modules.Values.SelectMany(module => module.Types.Values)).ToList();

        foreach (var typeReference in allTypes) {
            if (typeReference.AssignableFromAny) continue;
            foreach (var otherType in allTypes) {
                if (otherType == typeReference) continue;
                if (!typeReference.TO2Type.IsAssignableFrom(moduleContext, otherType.TO2Type)) continue;

                typeReference.AssignableFrom.Add(new TypeRef(moduleContext, otherType.TO2Type));
            }
        }
    }

    [JsonProperty("builtin")] public Dictionary<string, TypeReference> Builtin { get; }

    [JsonProperty("modules")] public Dictionary<string, ModuleReference> Modules { get; }

    public static bool IsModuleEmpty(IKontrolModule module) => !module.AllConstantNames.Any() &&
                                                               !module.AllFunctionNames.Any() &&
                                                               !module.AllTypeNames.Any();
}

public class ModuleReference {
    public ModuleReference(ModuleContext moduleContext, IKontrolModule module) {
        Name = module.Name;
        Description = module.Description;
        Types = [];
        TypeAliases = [];
        Constants = [];
        Functions = [];

        foreach (var typeName in module.AllTypeNames) {
            var type = module.FindType(typeName)!.UnderlyingType(moduleContext);
            if (type is FunctionType functionType) {
                TypeAliases.Add(typeName, new TypeRef(moduleContext, functionType));
            } else {
                var typeReference = new TypeReference(moduleContext, type, typeName);
                Types.Add(typeReference.Name, typeReference);
            }
        }

        foreach (var constantName in module.AllConstantNames) {
            var constant = module.FindConstant(constantName)!;
            var constantReference = new ConstantReference(moduleContext, constant);
            Constants.Add(constantReference.Name, constantReference);
        }

        foreach (var functionName in module.AllFunctionNames) {
            var function = module.FindFunction(functionName)?.PreferSync!;
            var functionReference = new FunctionReference(moduleContext, function);
            Functions.Add(functionReference.Name, functionReference);
        }
    }

    [JsonProperty("name")] public string Name { get; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string? Description { get; }

    [JsonProperty("types")] public Dictionary<string, TypeReference> Types { get; }

    [JsonProperty("typeAliases")] public Dictionary<string, TypeRef> TypeAliases { get; }

    [JsonProperty("constants")] public Dictionary<string, ConstantReference> Constants { get; }

    [JsonProperty("functions")] public Dictionary<string, FunctionReference> Functions { get; }
}

public class TypeReference {
    public TypeReference(ModuleContext moduleContext, RealizedType type, string name) {
        TO2Type = type;
        Name = name;
        Description = type.Description;
        Fields = [];
        Methods = [];
        if (type is ResultType)
            GenericParameters = ["R", "E"];
        else if (type is OptionType) {
            GenericParameters = ["T"];
        } else {
            GenericParameters = type.GenericParameters.Length > 0 ? type.GenericParameters.Select(t => t.Name).ToArray() : null;
        }

        AssignableFromAny = type == BuiltinType.Unit;
        AssignableFrom = [];

        foreach (var field in type.DeclaredFields) {
            var fieldReference = new FieldReference(moduleContext, field);
            Fields.Add(fieldReference.Name, fieldReference);
        }

        foreach (var method in type.DeclaredMethods) {
            var methodReference = new FunctionReference(moduleContext, method);
            Methods.Add(methodReference.Name, methodReference);
        }

        foreach (var (op, emitters) in type.AllowedPrefixOperators(moduleContext)) {
            PrefixOperators ??= [];
            foreach (var emitter in emitters) {
                var reference = new OperatorReference(moduleContext, op, emitter);

                if (PrefixOperators.TryGetValue(reference.Op, out var value))
                    value.Add(reference);
                else
                    PrefixOperators.Add(reference.Op, [reference]);
            }
        }

        foreach (var (op, emitters) in type.AllowedSuffixOperators(moduleContext)) {
            SuffixOperators ??= [];
            foreach (var emitter in emitters) {
                var reference = new OperatorReference(moduleContext, op, emitter);

                if (SuffixOperators.TryGetValue(reference.Op, out var value))
                    value.Add(reference);
                else
                    SuffixOperators.Add(reference.Op, [reference]);
            }
        }

        IForInSource? forInSource = type.ForInSource(moduleContext, null);

        if (forInSource != null) {
            ForInSource = new TypeRef(moduleContext, forInSource.ElementType);
        }

        IIndexAccessEmitter? indexAccess = type.AllowedIndexAccess(moduleContext, new IndexSpec(new LiteralInt(0)));

        if (indexAccess != null) {
            IndexAccess = new TypeRef(moduleContext, indexAccess.TargetType.UnderlyingType(moduleContext));
        }
    }

    [JsonProperty("name")] public string Name { get; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string Description { get; }

    [JsonProperty("genericParameters", NullValueHandling = NullValueHandling.Ignore)]
    public string[]? GenericParameters { get; }

    [JsonProperty("fields")] public Dictionary<string, FieldReference> Fields { get; }

    [JsonProperty("methods")] public Dictionary<string, FunctionReference> Methods { get; }

    [JsonProperty("prefixOperators", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, List<OperatorReference>>? PrefixOperators { get; }

    [JsonProperty("suffixOperators", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, List<OperatorReference>>? SuffixOperators { get; }

    [JsonProperty("assignableFromAny")]
    public bool AssignableFromAny { get; }

    [JsonProperty("assignableFrom")]
    public List<TypeRef> AssignableFrom { get; }

    [JsonProperty("forInSource", NullValueHandling = NullValueHandling.Ignore)]
    public TypeRef? ForInSource { get; }

    [JsonProperty("indexAccess", NullValueHandling = NullValueHandling.Ignore)]
    public TypeRef? IndexAccess { get; }

    [JsonIgnore]
    public RealizedType TO2Type { get; }
}

public class FieldReference(ModuleContext moduleContext, KeyValuePair<string, IFieldAccessFactory> field) {
    [JsonProperty("name")] public string Name { get; } = field.Key;

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string? Description { get; } = field.Value.Description;

    [JsonProperty("readOnly")] public bool ReadOnly { get; } = !field.Value.CanStore;

    [JsonProperty("type")] public TypeRef Type { get; } = new TypeRef(moduleContext, field.Value.DeclaredType.UnderlyingType(moduleContext));
}

public class OperatorReference(ModuleContext moduleContext, Operator op, IOperatorEmitter operatorEmitter) {
    [JsonProperty("op")]
    public string Op { get; } = op switch {
        Operator.Assign => "=",
        Operator.Add => "+",
        Operator.AddAssign => "+=",
        Operator.Sub => "-",
        Operator.SubAssign => "-=",
        Operator.Mul => "*",
        Operator.MulAssign => "*=",
        Operator.Div => "/",
        Operator.DivAssign => "/=",
        Operator.Mod => "%",
        Operator.ModAssign => "%=",
        Operator.BitOr => "|",
        Operator.BitOrAssign => "|=",
        Operator.BitAnd => "&",
        Operator.BitAndAssign => "&=",
        Operator.BitXor => "^",
        Operator.BitXorAssign => "^=",
        Operator.Pow => "**",
        Operator.PowAssign => "**=",
        Operator.Eq => "==",
        Operator.NotEq => "!=",
        Operator.Lt => "<",
        Operator.Le => "<=",
        Operator.Gt => ">",
        Operator.Ge => ">=",
        Operator.Neg => "-",
        Operator.Not => "!",
        Operator.BitNot => "~",
        Operator.BoolAnd => "&&",
        Operator.BoolOr => "||",
        Operator.Unwrap => "?",
        _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
    };

    [JsonProperty("otherType", NullValueHandling = NullValueHandling.Ignore)]
    public TypeRef? OtherType { get; } = operatorEmitter.OtherType != null
        ? new TypeRef(moduleContext, operatorEmitter.OtherType.UnderlyingType(moduleContext))
        : null;

    [JsonProperty("resultType")] public TypeRef ResultType { get; } = new TypeRef(moduleContext, operatorEmitter.ResultType.UnderlyingType(moduleContext));
}

public class ConstantReference(ModuleContext moduleContext, IKontrolConstant constant) {
    [JsonProperty("name")] public string Name { get; } = constant.Name;

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string? Description { get; } = constant.Description;

    [JsonProperty("type")] public TypeRef Type { get; } = new TypeRef(moduleContext, constant.Type.UnderlyingType(moduleContext));
}

public class FunctionReference {
    public FunctionReference(ModuleContext moduleContext, IKontrolFunction function) {
        Name = function.Name;
        Description = function.Description;
        ReturnType = new TypeRef(moduleContext, function.ReturnType);
        Parameters = function.Parameters.Select(param => new FunctionParameterReference(moduleContext, param))
            .ToArray();
    }

    public FunctionReference(ModuleContext moduleContext, KeyValuePair<string, IMethodInvokeFactory> method) {
        IsAsync = method.Value.IsAsync;
        Name = method.Key;
        Description = method.Value.Description;
        ReturnType = new TypeRef(moduleContext, method.Value.DeclaredReturn.UnderlyingType(moduleContext));
        Parameters = method.Value.DeclaredParameters
            .Select(param => new FunctionParameterReference(moduleContext, param))
            .ToArray();
    }

    [JsonProperty("isAsync")] public bool IsAsync { get; }

    [JsonProperty("name")] public string Name { get; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string? Description { get; }

    [JsonProperty("parameters")] public FunctionParameterReference[] Parameters { get; }

    [JsonProperty("returnType")] public TypeRef ReturnType { get; }
}

public class FunctionParameterReference {
    public FunctionParameterReference(ModuleContext moduleContext, RealizedParameter parameter) {
        Name = parameter.name;
        Type = new TypeRef(moduleContext, parameter.type);
        HasDefault = parameter.HasDefault;
        Description = parameter.description;
    }

    public FunctionParameterReference(ModuleContext moduleContext, FunctionParameter parameter) {
        Name = parameter.name;
        Type = new TypeRef(moduleContext, parameter.type!.UnderlyingType(moduleContext));
        HasDefault = parameter.HasDefault;
        Description = parameter.description;
    }

    [JsonProperty("name")] public string Name { get; }

    [JsonProperty("type")] public TypeRef Type { get; }

    [JsonProperty("hasDefault")] public bool HasDefault { get; }

    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string? Description { get; }
}

public class TypeRef {
    public enum TypeKind {
        Generic,
        Builtin,
        Standard,
        Array,
        Record,
        Result,
        Option,
        Tuple,
        Function,
    }

    public TypeRef(ModuleContext moduleContext, RealizedType type) {
        if (type is GenericParameter) {
            Kind = TypeKind.Generic;
            Name = type.Name;
        } else if (type is BuiltinType) {
            Kind = TypeKind.Builtin;
            Name = type.Name;
        } else if (type is ArrayType arrayType) {
            Kind = TypeKind.Array;
            Parameters = [new TypeRef(moduleContext, arrayType.ElementType.UnderlyingType(moduleContext))];
        } else if (type is OptionType optionType) {
            Kind = TypeKind.Option;
            Parameters = [new TypeRef(moduleContext, optionType.elementType.UnderlyingType(moduleContext))];
        } else if (type is ResultType resultType) {
            Kind = TypeKind.Result;
            Parameters = [new TypeRef(moduleContext, resultType.successType.UnderlyingType(moduleContext))];
        } else if (type is TupleType tupleType) {
            Kind = TypeKind.Tuple;
            Parameters = tupleType.itemTypes.Select(item => new TypeRef(moduleContext, item.UnderlyingType(moduleContext))).ToList();
        } else if (type is RecordTupleType recordType) {
            Kind = TypeKind.Record;
            Names = recordType.ItemTypes.Select(item => item.Key).ToList();
            Parameters = recordType.ItemTypes.Select(item => new TypeRef(moduleContext, item.Value.UnderlyingType(moduleContext))).ToList();
        } else if (type is FunctionType functionType) {
            Kind = TypeKind.Function;
            Parameters = functionType.parameterTypes.Select(item => new TypeRef(moduleContext, item.UnderlyingType(moduleContext))).ToList();
            ReturnType = new TypeRef(moduleContext, functionType.returnType.UnderlyingType(moduleContext));
            IsAsync = functionType.isAsync;
        } else {
            Kind = TypeKind.Standard;
            var idx = type.Name.LastIndexOf("::", StringComparison.Ordinal);

            if (idx < 0) {
                Module = "";
                Name = type.Name;
            } else {
                Module = type.Name[..idx];
                Name = type.Name[(idx + 2)..];
            }

            if (type.GenericParameters.Length > 0) {
                idx = Name.IndexOf('<');
                if (idx > 0) Name = Name[..idx];
                Parameters = type.GenericParameters
                    .Select(t => new TypeRef(moduleContext, t.UnderlyingType(moduleContext))).ToList();
            }
        }
    }

    [JsonProperty("kind")]
    [JsonConverter(typeof(StringEnumConverter))]
    public TypeKind Kind { get; }

    [JsonProperty("module", NullValueHandling = NullValueHandling.Ignore)]
    public string? Module { get; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; }

    [JsonProperty("names", NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Names { get; }

    [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
    public List<TypeRef>? Parameters { get; }

    [JsonProperty("returnType", NullValueHandling = NullValueHandling.Ignore)]
    public TypeRef? ReturnType { get; }

    [JsonProperty("isAsync", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsAsync { get; }
}
