using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.GenDocs;

class MainClass {
    public static void Main(string[] args) {
        var registry = KontrolSystemKSPRegistry.CreateKSP();

        var context = registry.AddDirectory(Path.Combine(Directory.GetCurrentDirectory(), "KSP2Runtime", "to2"));

        GenerateDocs(context.CreateModuleContext("docs"), registry);
    }

    public static void GenerateDocs(ModuleContext moduleContext, KontrolRegistry registry) {
        foreach (IKontrolModule module in registry.modules.Values) {
            if (IsModuleEmpty(module) || !module.Name.Contains("::")) continue;
            var split = module.Name.Split(["::"], StringSplitOptions.None);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "docs", "reference", split.First());
            Directory.CreateDirectory(path);
            var fileName = Path.Combine(path, String.Join("_", split.Skip(1)) + ".md");
            using StreamWriter fs = File.CreateText(fileName);
            GenerateDocs(moduleContext, module, fs);
            Console.Out.WriteLine($"Generated: {module.Name}");
        }
    }

    public static bool IsModuleEmpty(IKontrolModule module) => !module.AllConstantNames.Any() &&
                                                               !module.AllFunctionNames.Any() &&
                                                               !module.AllTypeNames.Any();

    public static void GenerateDocs(ModuleContext moduleContext, IKontrolModule module, TextWriter output) {
        output.WriteLine($"# {module.Name}");
        output.WriteLine();
        output.WriteLine(module.Description);

        if (module.AllTypeNames.Any()) {
            output.WriteLine();
            output.WriteLine("## Types");
            output.WriteLine();

            foreach (string typeName in module.AllTypeNames.OrderBy(name => name)) {
                RealizedType type = module.FindType(typeName)!.UnderlyingType(moduleContext);

                output.WriteLine();
                output.WriteLine($"### {typeName}");
                output.WriteLine();
                output.WriteLine(type.Description);

                IForInSource? forInSource = type.ForInSource(moduleContext, null);

                if (forInSource != null) {
                    output.WriteLine();
                    output.WriteLine($"Can be used in `for(... in ...)` loop to iterate over {LinkType(forInSource.ElementType.Name)}");
                }

                IIndexAccessEmitter? indexAccess = type.AllowedIndexAccess(moduleContext, new IndexSpec(new LiteralInt(0)));

                if (indexAccess != null) {
                    output.WriteLine();
                    output.WriteLine($"Can be used like an array {LinkType(indexAccess.TargetType.Name)}[]");
                }

                if (type.DeclaredFields.Count > 0) {
                    output.WriteLine();
                    output.WriteLine("#### Fields");
                    output.WriteLine();

                    var table = new TableHelper("Name", "Type", "Read-only", "Description");

                    foreach (var (name, declaredType) in type.DeclaredFields.OrderBy(kv => kv.Key).Select(kv => (kv.Key, kv.Value))) {
                        table.AddRow(name, LinkType(declaredType.DeclaredType.Name), declaredType.CanStore ? "R/W" : "R/O", declaredType.Description?.Replace("\n", " ") ?? "");
                    }
                    output.WriteLine(table.Markdown);
                }

                if (type.DeclaredMethods.Count > 0) {
                    output.WriteLine();
                    output.WriteLine("#### Methods");

                    foreach (var (name, method) in type.DeclaredMethods.OrderBy(kv => kv.Key).Select(kv => (kv.Key, kv.Value))) {
                        output.WriteLine();
                        output.WriteLine($"##### {name}");
                        output.WriteLine();
                        output.WriteLine("```rust");
                        output.WriteLine(MethodSignature(type.LocalName, name, method));
                        output.WriteLine("```");
                        output.WriteLine();
                        output.WriteLine(method.Description);


                        if (method.DeclaredParameters.Count > 0) {
                            output.WriteLine();
                            output.WriteLine("Parameters");
                            output.WriteLine();

                            var table = new TableHelper("Name", "Type", "Optional", "Description");

                            foreach (FunctionParameter parameter in method.DeclaredParameters) {
                                table.AddRow(parameter.name, parameter.type?.ToString() ?? "", parameter.HasDefault ? "x" : "", parameter.description ?? "");
                            }
                            output.WriteLine(table.Markdown);
                        }
                    }
                }
            }
        }

        if (module.AllConstantNames.Any()) {
            output.WriteLine();
            output.WriteLine("## Constants");
            output.WriteLine();

            var table = new TableHelper("Name", "Type", "Description");

            foreach (string constantName in module.AllConstantNames.OrderBy(name => name)) {
                IKontrolConstant constant = module.FindConstant(constantName)!;

                table.AddRow(constantName, constant.Type.ToString(), constant.Description?.Replace("\n", " ") ?? "");
            }

            output.WriteLine(table.Markdown);
        }

        if (module.AllFunctionNames.Any()) {
            output.WriteLine();
            output.WriteLine("## Functions");
            output.WriteLine();

            foreach (string functionName in module.AllFunctionNames.OrderBy(name => name)) {
                IKontrolFunction function = module.FindFunction(functionName)?.PreferSync!;

                output.WriteLine();
                output.WriteLine($"### {functionName}");
                output.WriteLine();
                output.WriteLine("```rust");
                output.WriteLine(FunctionSignature(function));
                output.WriteLine("```");
                output.WriteLine();
                output.WriteLine(function.Description);

                if (function.Parameters.Count > 0) {
                    output.WriteLine();
                    output.WriteLine("Parameters");
                    output.WriteLine();

                    var table = new TableHelper("Name", "Type", "Optional", "Description");

                    foreach (RealizedParameter parameter in function.Parameters) {
                        table.AddRow(parameter.name, parameter.type?.ToString() ?? "", parameter.HasDefault ? "x" : "", parameter.description ?? "");
                    }

                    output.WriteLine(table.Markdown);
                }
            }
        }
    }

    public static string LinkType(string typeName) {
        var idx = typeName.IndexOf('[');
        if (idx > 0) {
            return $"{LinkTypeInner(typeName[..idx])}[]";
        }

        if (typeName.StartsWith("Record<")) {
            idx = typeName.IndexOf(',');
            if (idx > 0) {
                return $"Record&lt;{LinkType(typeName[7..idx])}{typeName[idx..]}";
            }
        }
        if (typeName.StartsWith("Option<")) {
            idx = typeName.IndexOf('>');
            if (idx > 0) {
                return $"Option&lt;{LinkType(typeName[7..idx])}{typeName[idx..]}";
            }
        }

        return LinkTypeInner(typeName);
    }

    public static string LinkTypeInner(string typeName) {
        var idx = typeName.LastIndexOf("::", StringComparison.InvariantCulture);

        if (idx > 0) {
            var moduleName = typeName[..idx];
            var localName = typeName[(idx + 2)..];

            var split = moduleName.Split(["::"], StringSplitOptions.None);
            if (split.Length > 0) {
                var folder = split.First();
                var file = Path.Combine(folder, String.Join("_", split.Skip(1)) + ".md");

                return $"[{typeName}](/reference/{file}#{localName.ToLower(CultureInfo.InvariantCulture)})";
            }
        }

        return typeName;
    }

    public static string FunctionSignature(IKontrolFunction function) {
        StringBuilder sb = new();

        sb.Append("pub ");
        if (!function.IsAsync) sb.Append("sync ");
        sb.Append("fn ");
        sb.Append(function.Name);

        if (function.Parameters.Count == 0) {
            sb.Append(" (");
        } else {
            sb.Append(" ( ");

            string offset = new(' ', sb.Length);

            sb.Append(FunctionParameterSignature(function.Parameters.First()));
            foreach (RealizedParameter parameter in function.Parameters.Skip(1)) {
                sb.Append(",\n");
                sb.Append(offset);
                sb.Append(FunctionParameterSignature(parameter));
            }
        }

        sb.Append(" ) -> ");
        sb.Append(function.ReturnType);

        return sb.ToString();
    }

    public static string FunctionParameterSignature(RealizedParameter parameter) {
        StringBuilder sb = new();

        sb.Append(parameter.name);
        sb.Append(" : ");
        sb.Append(parameter.type);

        return sb.ToString();
    }

    public static string MethodSignature(string type, string name, IMethodInvokeFactory method) {
        StringBuilder sb = new();

        sb.Append(type.ToLowerInvariant());
        sb.Append('.');
        sb.Append(name);

        if (method.DeclaredParameters.Count == 0) {
            sb.Append(" (");
        } else {
            sb.Append(" ( ");

            string offset = new(' ', sb.Length);

            sb.Append(MethodParameterSignature(method.DeclaredParameters.First()));
            foreach (FunctionParameter parameter in method.DeclaredParameters.Skip(1)) {
                sb.Append(",\n");
                sb.Append(offset);
                sb.Append(MethodParameterSignature(parameter));
            }
        }

        sb.Append(" ) -> ");
        sb.Append(method.DeclaredReturn);

        return sb.ToString();
    }

    public static string MethodParameterSignature(FunctionParameter parameter) {
        StringBuilder sb = new();

        sb.Append(parameter.name);
        sb.Append(" : ");
        sb.Append(parameter.type);

        return sb.ToString();
    }
}
