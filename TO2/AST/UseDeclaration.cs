using System.Collections.Generic;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public class UseDeclaration : Node, IModuleItem {
    private readonly string? alias;
    private readonly string fromModule;
    private readonly List<string>? names;

    public UseDeclaration(List<string>? names, List<string> moduleNamePath, Position start = new(),
        Position end = new()) : base(start, end) {
        fromModule = string.Join("::", moduleNamePath);
        this.names = names;
    }

    public UseDeclaration(List<string> moduleNamePath, string alias, Position start = new(),
        Position end = new()) : base(start, end) {
        fromModule = string.Join("::", moduleNamePath);
        this.alias = alias;
    }

    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
        var module = context.FindModule(fromModule);

        if (module == null)
            return [new StructuralError(
                StructuralError.ErrorType.NoSuchModule,
                $"Module '{fromModule}' not found",
                Start,
                End
            )];
        if (alias != null)
            context.moduleAliases.Add(alias, fromModule);
        else
            foreach (var name in names ?? module.AllTypeNames) {
                var type = module.FindType(name);

                if (type != null) context.mappedTypes.Add(name, type);
            }

        return [];
    }

    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
        if (alias != null) return [];
        var module = context.FindModule(fromModule);

        if (module == null)
            return [new StructuralError(
                StructuralError.ErrorType.NoSuchModule,
                $"Module '{fromModule}' not found",
                Start,
                End
            )];
        foreach (var name in names ?? module.AllConstantNames) {
            var constant = module.FindConstant(name);

            if (constant != null) context.mappedConstants.Add(name, constant);
        }

        return [];
    }

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) {
        if (alias != null) return [];
        var module = context.FindModule(fromModule);

        if (module == null)
            return [new StructuralError(
                StructuralError.ErrorType.NoSuchModule,
                $"Module '{fromModule}' not found",
                Start,
                End
            )];

        var errors = new List<StructuralError>();

        foreach (var name in names ?? module.AllFunctionNames) {
            if (context.mappedConstants.ContainsKey(name)) continue;

            var function = module.FindFunction(name);

            if (function != null)
                context.mappedFunctions.Add(name, function);
            else if (!context.mappedTypes.ContainsKey(name))
                errors.Add(new StructuralError(
                    StructuralError.ErrorType.InvalidImport,
                    $"Module '{fromModule}' does not have public member '{name}''",
                    Start,
                    End
                ));
        }

        return errors;
    }
}
