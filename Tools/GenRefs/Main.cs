using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace KontrolSystem.GenRefs {
    class MainClass {
        public static void Main(string[] args) {
            var registry = KontrolSystemKSPRegistry.CreateKSP();

            var context = registry.AddDirectory(Path.Combine(Directory.GetCurrentDirectory(), "KSP2Runtime", "to2"));
            
            var reference = GenerateReference(context.CreateModuleContext("reference"), registry);
            
            var path = Path.Combine(Directory.GetCurrentDirectory(), "docs", "reference.json");
            
            File.WriteAllText(path, JsonConvert.SerializeObject(reference, Formatting.Indented));
        }

        public static Dictionary<string, ModuleReference> GenerateReference(ModuleContext moduleContext, KontrolRegistry registry) {
            var reference = new Dictionary<string, ModuleReference>();
            foreach (IKontrolModule module in registry.modules.Values) {
                if (IsModuleEmpty(module) || !module.Name.Contains("::")) continue;
                var moduleReference = new ModuleReference(moduleContext, module);
                reference.Add(moduleReference.Name, moduleReference);
            }

            return reference;
        }
        
        public static bool IsModuleEmpty(IKontrolModule module) => !module.AllConstantNames.Any() &&
                                                                   !module.AllFunctionNames.Any() &&
                                                                   !module.AllTypeNames.Any();
    }

    public class ModuleReference {
        public ModuleReference(ModuleContext moduleContext, IKontrolModule module) {
            Name = module.Name;
            Description = module.Description;
            Types = new Dictionary<string, TypeReference>();
            Constants = new Dictionary<string, ConstantReference>();
            Functions = new Dictionary<string, FunctionReference>();
            
            foreach (string typeName in module.AllTypeNames) {
                RealizedType type = module.FindType(typeName)?.UnderlyingType(moduleContext);
                var typeReference = new TypeReference(moduleContext, type);
                Types.Add(typeReference.Name, typeReference);
            }

            foreach (string constantName in module.AllConstantNames) {
                IKontrolConstant constant = module.FindConstant(constantName);
                var constantReference = new ConstantReference(moduleContext, constant);
                Constants.Add(constantReference.Name, constantReference);
            }

            foreach (var functionName in module.AllFunctionNames) {
                IKontrolFunction function = module.FindFunction(functionName);
                var functionReference = new FunctionReference(moduleContext, function);
                Functions.Add(functionReference.Name, functionReference);
            }
        }
        
        [JsonProperty("name")]
        public string Name { get; }
        
        [JsonProperty("description")]
        public string Description { get; }
        
        [JsonProperty("types")]
        public Dictionary<string, TypeReference> Types { get; }
                
        [JsonProperty("constants")]
        public Dictionary<string, ConstantReference> Constants { get; }

        [JsonProperty("functions")]
        public Dictionary<string, FunctionReference> Functions { get; }

    }

    public class TypeReference {
        public TypeReference(ModuleContext moduleContext, RealizedType type) {
            Name = type.LocalName;
            Description = type.Description;
            Fields = new Dictionary<string, FieldReference>();
            Methods = new Dictionary<string, FunctionReference>();
            GenericParameters = type.GenericParameters.Length > 0 ? type.GenericParameters : null;
            
            foreach (var field in type.DeclaredFields) {
                var fieldReference = new FieldReference(moduleContext, field);
                Fields.Add(fieldReference.Name, fieldReference);
            }

            foreach (var method in type.DeclaredMethods) {
                var methodReference = new FunctionReference(moduleContext, method);
                Methods.Add(methodReference.Name, methodReference);
            }
        }

        [JsonProperty("name")]
        public string Name { get; }
        
        [JsonProperty("description")]
        public string Description { get; }
        
        [JsonProperty("genericParameters", NullValueHandling = NullValueHandling.Ignore)]
        public string[] GenericParameters { get; }
        
        [JsonProperty("fields")]
        public Dictionary<string, FieldReference> Fields { get; }
        
        [JsonProperty("methods")]
        public Dictionary<string, FunctionReference> Methods { get; }
    }

    public class FieldReference {
        public FieldReference(ModuleContext moduleContext, KeyValuePair<string, IFieldAccessFactory> field) {
            Name = field.Key;
            Description = field.Value.Description;
            ReadOnly = !field.Value.CanStore;
            Type = new TypeRef(moduleContext, field.Value.DeclaredType);
        }
        
        [JsonProperty("name")]
        public string Name { get; }
        
        [JsonProperty("description")]
        public string Description { get; }
        
        [JsonProperty("readOnly")]
        public bool ReadOnly { get; }
        
        [JsonProperty("type")]
        public TypeRef Type { get; }
    }

    public class ConstantReference {
        public ConstantReference(ModuleContext moduleContext, IKontrolConstant constant) {
            Name = constant.Name;
            Description = constant.Description;
            Type = new TypeRef(moduleContext, constant.Type);
        }
        
        [JsonProperty("name")]
        public string Name { get; }
        
        [JsonProperty("description")]
        public string Description { get; }

        [JsonProperty("type")]
        public TypeRef Type { get; }
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
            Name = method.Key;
            Description = method.Value.Description;
            ReturnType = new TypeRef(moduleContext, method.Value.DeclaredReturn);
            Parameters = method.Value.DeclaredParameters.Select(param => new FunctionParameterReference(moduleContext, param))
                .ToArray();
        }

        [JsonProperty("name")]
        public string Name { get; }
        
        [JsonProperty("description")]
        public string Description { get; }
        
        [JsonProperty("parameters")]
        public FunctionParameterReference[] Parameters { get; }
        
        [JsonProperty("returnType")] private TypeRef ReturnType { get; }
    }

    public class FunctionParameterReference {
        public FunctionParameterReference(ModuleContext moduleContext, RealizedParameter parameter) {
            Name = parameter.name;
            Type = new TypeRef(moduleContext, parameter.type);
            HasDefault = parameter.HasDefault;
        }
        
        public FunctionParameterReference(ModuleContext moduleContext, FunctionParameter parameter) {
            Name = parameter.name;
            Type = new TypeRef(moduleContext, parameter.type);
            HasDefault = parameter.defaultValue != null;
        }
        
        [JsonProperty("name")]
        public string Name { get; }
        
        [JsonProperty("type")] private TypeRef Type { get; }

        [JsonProperty("hasDefault")]
        public bool HasDefault { get; }
    }
    
    public class TypeRef {
        public enum TypeKind {
            Generic,
            Builtin,
            Standard,
        }
        
        public TypeRef(ModuleContext moduleContext, TO2Type type) {
            if (type is GenericParameter) {
                Kind = TypeKind.Generic;
                Module = null;
                Name = type.Name;
            } else if (type is BuiltinType) {
                Kind = TypeKind.Builtin;
                Module = null;
                Name = type.Name; 
            } else {
                Kind = TypeKind.Standard;
                var idx = type.Name.LastIndexOf("::", StringComparison.Ordinal);

                if(idx < 0) {
                    Module = "";
                    Name = type.Name;
                } else {
                    Module = type.Name.Substring(0, idx);
                    Name = type.Name.Substring(idx + 2);
                }
            }
        }
        
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]  
        public TypeKind Kind { get; }
        
        [JsonProperty("module", NullValueHandling = NullValueHandling.Ignore)]
        public string Module { get; }
        
        [JsonProperty("name")]
        public string Name { get; }
    }
}
