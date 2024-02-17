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

namespace KontrolSystem.GenRefs {
    class MainClass {
        public static void Main(string[] args) {
            var registry = KontrolSystemKSPRegistry.CreateKSP();

            var context = registry.AddDirectory(Path.Combine(Directory.GetCurrentDirectory(), "KSP2Runtime", "to2"));

            var reference = GenerateReference(context.CreateModuleContext("reference"), registry);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Tools", "vscode", "to2-syntax", "server", "src", "reference", "reference.json");

            File.WriteAllText(path, JsonConvert.SerializeObject(reference, Formatting.Indented));
        }

        public static Reference GenerateReference(ModuleContext moduleContext, KontrolRegistry registry) =>
            new Reference(moduleContext, registry);
    }

    public class Reference {
        public Reference(ModuleContext moduleContext, KontrolRegistry registry) {
            Builtin = new Dictionary<string, TypeReference>();
            Modules = new Dictionary<string, ModuleReference>();

            foreach (var type in new List<RealizedType> {
                         BuiltinType.Unit, BuiltinType.Bool, BuiltinType.Int,
                         BuiltinType.Float, BuiltinType.String, BuiltinType.Range, BuiltinType.ArrayBuilder,
                         BuiltinType.Cell, new OptionType(new GenericParameter("T")),
                         new ResultType(new GenericParameter("R"), new GenericParameter("E"))
                     }) {
                Builtin.Add(type.LocalName, new TypeReference(moduleContext, type, type.LocalName));
            }

            foreach (IKontrolModule module in registry.modules.Values) {
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
            Types = new Dictionary<string, TypeReference>();
            TypeAliases = new Dictionary<string, TypeRef>();
            Constants = new Dictionary<string, ConstantReference>();
            Functions = new Dictionary<string, FunctionReference>();

            foreach (string typeName in module.AllTypeNames) {
                RealizedType type = module.FindType(typeName)!.UnderlyingType(moduleContext);
                if (type is FunctionType functionType) {
                    TypeAliases.Add(typeName, new TypeRef(moduleContext, functionType));
                } else {
                    var typeReference = new TypeReference(moduleContext, type, typeName);
                    Types.Add(typeReference.Name, typeReference);
                }
            }

            foreach (string constantName in module.AllConstantNames) {
                IKontrolConstant constant = module.FindConstant(constantName)!;
                var constantReference = new ConstantReference(moduleContext, constant);
                Constants.Add(constantReference.Name, constantReference);
            }

            foreach (var functionName in module.AllFunctionNames) {
                IKontrolFunction function = module.FindFunction(functionName)!;
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
            Fields = new Dictionary<string, FieldReference>();
            Methods = new Dictionary<string, FunctionReference>();
            if (type is ResultType)
                GenericParameters = new[] { "R", "E" };
            else if (type is OptionType) {
                GenericParameters = new[] { "T" };
            } else {
                GenericParameters = type.GenericParameters.Length > 0 ? type.GenericParameters.Select(t => t.Name).ToArray() : null;
            }

            AssignableFromAny = type == BuiltinType.Unit;
            AssignableFrom = new List<TypeRef>();

            foreach (var field in type.DeclaredFields) {
                var fieldReference = new FieldReference(moduleContext, field);
                Fields.Add(fieldReference.Name, fieldReference);
            }

            foreach (var method in type.DeclaredMethods) {
                var methodReference = new FunctionReference(moduleContext, method);
                Methods.Add(methodReference.Name, methodReference);
            }

            foreach (var prefixOperator in type.AllowedPrefixOperators(moduleContext)) {
                PrefixOperators ??= new Dictionary<string, List<OperatorReference>>();
                foreach (var emitter in prefixOperator.emitters) {
                    var reference = new OperatorReference(moduleContext, prefixOperator.op, emitter);

                    if (PrefixOperators.TryGetValue(reference.Op, out var value))
                        value.Add(reference);
                    else
                        PrefixOperators.Add(reference.Op, new List<OperatorReference> { reference });
                }
            }

            foreach (var suffixOperator in type.AllowedSuffixOperators(moduleContext)) {
                SuffixOperators ??= new Dictionary<string, List<OperatorReference>>();
                foreach (var emitter in suffixOperator.emitters) {
                    var reference = new OperatorReference(moduleContext, suffixOperator.op, emitter);

                    if (SuffixOperators.TryGetValue(reference.Op, out var value))
                        value.Add(reference);
                    else
                        SuffixOperators.Add(reference.Op, new List<OperatorReference> { reference });
                }
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

        [JsonIgnore]
        public RealizedType TO2Type { get; }
    }

    public class FieldReference {
        public FieldReference(ModuleContext moduleContext, KeyValuePair<string, IFieldAccessFactory> field) {
            Name = field.Key;
            Description = field.Value.Description;
            ReadOnly = !field.Value.CanStore;
            Type = new TypeRef(moduleContext, field.Value.DeclaredType.UnderlyingType(moduleContext));
        }

        [JsonProperty("name")] public string Name { get; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string? Description { get; }

        [JsonProperty("readOnly")] public bool ReadOnly { get; }

        [JsonProperty("type")] public TypeRef Type { get; }
    }

    public class OperatorReference {
        public OperatorReference(ModuleContext moduleContext, Operator op, IOperatorEmitter operatorEmitter) {
            Op = op switch {
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
            OtherType = operatorEmitter.OtherType != null
                ? new TypeRef(moduleContext, operatorEmitter.OtherType.UnderlyingType(moduleContext))
                : null;
            ResultType = new TypeRef(moduleContext, operatorEmitter.ResultType.UnderlyingType(moduleContext));
        }

        [JsonProperty("op")] public string Op { get; }

        [JsonProperty("otherType", NullValueHandling = NullValueHandling.Ignore)]
        public TypeRef? OtherType { get; }

        [JsonProperty("resultType")] public TypeRef ResultType { get; }
    }

    public class ConstantReference {
        public ConstantReference(ModuleContext moduleContext, IKontrolConstant constant) {
            Name = constant.Name;
            Description = constant.Description;
            Type = new TypeRef(moduleContext, constant.Type.UnderlyingType(moduleContext));
        }

        [JsonProperty("name")] public string Name { get; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)] public string? Description { get; }

        [JsonProperty("type")] public TypeRef Type { get; }
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

        [JsonProperty("returnType")] private TypeRef ReturnType { get; }
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

        [JsonProperty("type")] private TypeRef Type { get; }

        [JsonProperty("hasDefault")] public bool HasDefault { get; }

        [JsonProperty("description")] public string? Description { get; }
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
                Parameters = new List<TypeRef> { new TypeRef(moduleContext, arrayType.ElementType.UnderlyingType(moduleContext)) };
            } else if (type is OptionType optionType) {
                Kind = TypeKind.Option;
                Parameters = new List<TypeRef> { new TypeRef(moduleContext, optionType.elementType.UnderlyingType(moduleContext)) };
            } else if (type is ResultType resultType) {
                Kind = TypeKind.Result;
                Parameters = new List<TypeRef> { new TypeRef(moduleContext, resultType.successType.UnderlyingType(moduleContext)), new TypeRef(moduleContext, resultType.errorType.UnderlyingType(moduleContext)) };
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
                    Module = type.Name.Substring(0, idx);
                    Name = type.Name.Substring(idx + 2);
                }

                if (type.GenericParameters.Length > 0) {
                    idx = Name.IndexOf('<');
                    if (idx > 0) Name = Name.Substring(0, idx);
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
}
