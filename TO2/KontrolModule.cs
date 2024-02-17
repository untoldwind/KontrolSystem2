using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2;

public interface IKontrolModule {
    string Name { get; }

    string? Description { get; }

    bool IsCompiled { get; }

    bool IsBuiltin { get; }

    string? SourceFile { get; }

    IEnumerable<string> AllTypeNames { get; }

    IEnumerable<string> AllConstantNames { get; }

    IEnumerable<string> AllFunctionNames { get; }

    IEnumerable<IKontrolFunction> TestFunctions { get; }

    TO2Type? FindType(string name);

    IKontrolConstant? FindConstant(string name);

    IKontrolFunction? FindFunction(string name);
}

public class CompiledKontrolModule : IKontrolModule {
    private readonly Dictionary<string, IKontrolConstant> constants;
    private readonly Dictionary<string, CompiledKontrolFunction> publicFunctions;
    private readonly List<CompiledKontrolFunction> testFunctions;
    private readonly Dictionary<string, RealizedType> types;

    public CompiledKontrolModule(string name,
        string? description,
        bool builtin,
        string? sourceFile,
        IEnumerable<(string alias, RealizedType type)> types,
        IEnumerable<IKontrolConstant> constants,
        IEnumerable<CompiledKontrolFunction> functions,
        List<CompiledKontrolFunction> testFunctions) {
        Name = name;
        Description = description;
        IsBuiltin = builtin;
        SourceFile = sourceFile;
        var compiledKontrolConstants = constants.ToList();
        this.constants = compiledKontrolConstants.ToDictionary(constant => constant.Name);
        var compiledKontrolFunctions = functions.ToList();
        publicFunctions = compiledKontrolFunctions.ToDictionary(function => function.Name);
        this.testFunctions = testFunctions;
        this.types = types.ToDictionary(t => t.alias, t => t.type);

        //            foreach (CompiledKontrolConstant constant in compiledKontrolConstants) constant.Module = this;
        foreach (var function in compiledKontrolFunctions) function.Module = this;
        foreach (var function in testFunctions) function.Module = this;
    }

    public string Name { get; }
    public string? Description { get; }
    public bool IsBuiltin { get; }
    public string? SourceFile { get; }

    public bool IsCompiled => true;

    public IEnumerable<string> AllTypeNames => types.Keys;

    public TO2Type? FindType(string name) {
        return types.Get(name);
    }

    public IEnumerable<string> AllConstantNames => constants.Keys;

    public IKontrolConstant? FindConstant(string name) {
        return constants.Get(name);
    }

    public IEnumerable<string> AllFunctionNames => publicFunctions.Keys;

    public IKontrolFunction? FindFunction(string name) {
        return publicFunctions.Get(name);
    }

    public IEnumerable<IKontrolFunction> TestFunctions => testFunctions;

    public void RegisterType(BoundType to2Type) {
        types.Add(to2Type.localName, to2Type);
    }
}

public class DeclaredKontrolModule : IKontrolModule {
    public readonly Dictionary<string, DeclaredKontrolConstant> declaredConstants;
    public readonly List<DeclaredKontrolFunction> declaredFunctions;
    public readonly List<DeclaredKontrolStructConstructor> declaredStructConstructors;
    public readonly ModuleContext moduleContext;
    public readonly Dictionary<string, IKontrolFunction> publicFunctions;
    private readonly Dictionary<string, TO2Type> publicTypes;
    public readonly TO2Module to2Module;

    public DeclaredKontrolModule(string name, string description,
        bool builtin,
        string sourceFile,
        ModuleContext moduleContext, TO2Module to2Module,
        IEnumerable<(string alias, TO2Type type)> types) {
        Name = name;
        Description = description;
        IsBuiltin = builtin;
        SourceFile = sourceFile;
        this.moduleContext = moduleContext;
        this.to2Module = to2Module;
        publicTypes = types.ToDictionary(t => t.alias, t => t.type);
        publicFunctions = new Dictionary<string, IKontrolFunction>();
        declaredFunctions = new List<DeclaredKontrolFunction>();
        declaredStructConstructors = new List<DeclaredKontrolStructConstructor>();
        declaredConstants = new Dictionary<string, DeclaredKontrolConstant>();
    }

    public string Name { get; }
    public string Description { get; }
    public bool IsBuiltin { get; }
    public string SourceFile { get; }

    public bool IsCompiled => true;

    public IEnumerable<string> AllTypeNames => Enumerable.Empty<string>();

    public TO2Type? FindType(string name) {
        return publicTypes.Get(name);
    }

    public IEnumerable<string> AllConstantNames =>
        declaredConstants.Where(kv => kv.Value.IsPublic).Select(kv => kv.Key);

    public IKontrolConstant? FindConstant(string name) {
        return declaredConstants.Get(name);
    }

    public IEnumerable<string> AllFunctionNames => publicFunctions.Keys;

    public IKontrolFunction? FindFunction(string name) {
        return publicFunctions.Get(name);
    }

    public IEnumerable<IKontrolFunction> TestFunctions =>
        declaredFunctions.Where(f => f.to2Function.modifier == FunctionModifier.Test);
}
