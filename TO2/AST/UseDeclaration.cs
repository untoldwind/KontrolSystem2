using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

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
            return new StructuralError(
                StructuralError.ErrorType.NoSuchModule,
                $"Module '{fromModule}' not found",
                Start,
                End
            ).Yield();
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
            return new StructuralError(
                StructuralError.ErrorType.NoSuchModule,
                $"Module '{fromModule}' not found",
                Start,
                End
            ).Yield();
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
            return new StructuralError(
                StructuralError.ErrorType.NoSuchModule,
                $"Module '{fromModule}' not found",
                Start,
                End
            ).Yield();

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

    public override REPLValueFuture Eval(REPLContext context) {
        var module = context.replModuleContext.FindModule(fromModule);

        if (module == null)
            throw new REPLException(this, $"Module '{fromModule}' not found");
        if (alias != null)
            context.replModuleContext.moduleAliases.Add(alias, fromModule);
        else
            foreach (var name in names ?? module.AllTypeNames) {
                var type = module.FindType(name);

                if (type != null) context.replModuleContext.mappedTypes.Add(name, type);
            }

        foreach (var name in names ?? module.AllConstantNames) {
            var constant = module.FindConstant(name);

            if (constant != null) context.replModuleContext.mappedConstants.Add(name, constant);
        }

        foreach (var name in names ?? module.AllFunctionNames) {
            if (context.replModuleContext.mappedConstants.ContainsKey(name)) continue;

            var function = module.FindFunction(name);

            if (function != null)
                context.replModuleContext.mappedFunctions.Add(name, function);
            else if (!context.replModuleContext.mappedTypes.ContainsKey(name))
                throw new REPLException(this, $"Module '{fromModule}' does not have public member '{name}''");
        }

        return REPLValueFuture.Success(REPLUnit.INSTANCE);
    }
}
