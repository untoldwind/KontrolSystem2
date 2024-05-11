using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2;

public class KontrolRegistry {
    public readonly SortedDictionary<string, IKontrolModule> modules = [];

    public void RegisterModule(IKontrolModule kontrolModule) {
        if (modules.ContainsKey(kontrolModule.Name)) {
            if (modules[kontrolModule.Name] is not DeclaredKontrolModule)
                throw new ArgumentException($"Module {kontrolModule.Name} is defined twice");
            modules[kontrolModule.Name] = kontrolModule;
        } else {
            modules.Add(kontrolModule.Name, kontrolModule);
        }
    }

    public static KontrolRegistry CreateCore() {
        var registry = new KontrolRegistry();

        registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreError)));
        registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreStr)));
        registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreLogging)));
        registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreTesting)));
        registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreBackground)));
        registry.RegisterModule(DirectBindingMath.Module);

        return registry;
    }

    public Context AddFile(string baseDir, string file) {
        var to2Module = TO2Parser.ParseModuleFile(baseDir, file);
        var context = new Context(this);
        var declaredKontrolModule = ModuleGenerator.DeclareModule(context, to2Module, Path.Combine(baseDir, file));
        ModuleGenerator.ImportTypes(declaredKontrolModule);
        ModuleGenerator.DeclareConstants(declaredKontrolModule);
        ModuleGenerator.ImportConstants(declaredKontrolModule);
        ModuleGenerator.DeclareFunctions(declaredKontrolModule);
        ModuleGenerator.ImportFunctions(declaredKontrolModule);
        ModuleGenerator.VerifyFunctions(declaredKontrolModule);
        var kontrolModule = ModuleGenerator.CompileModule(declaredKontrolModule);

        RegisterModule(kontrolModule);

        return context;
    }

    public Context AddDirectory(string baseDir, ITO2Logger? logger = null) {
        var context = new Context(this);
        var declaredModules = new List<DeclaredKontrolModule>();
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        foreach (var fileName in Directory.GetFiles(baseDir, "*.to2", SearchOption.AllDirectories)) {
            if (!fileName.EndsWith(".to2")) continue;

            // First declare the existence of a module and its types
            var to2Module = TO2Parser.ParseModuleFile(baseDir, fileName.Remove(0, baseDir.Length + 1));
            var module = ModuleGenerator.DeclareModule(context, to2Module, fileName);

            declaredModules.Add(module);
            context.registry.RegisterModule(module);
        }
        logger?.Info($"Phase1: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules)
            // ... so that types can be imported by other modules
            ModuleGenerator.ImportTypes(declared);

        logger?.Info($"Phase2: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules) ModuleGenerator.DeclareConstants(declared);

        logger?.Info($"Phase3: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules) ModuleGenerator.ImportConstants(declared);

        logger?.Info($"Phase4: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules)
            // ... so that function can be declared (potentially using imported types as arguments or return)
            ModuleGenerator.DeclareFunctions(declared);

        logger?.Info($"Phase5: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules)
            // ... so that other modules may import these functions
            ModuleGenerator.ImportFunctions(declared);

        logger?.Info($"Phase6: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules) ModuleGenerator.CompileStructs(declared);

        logger?.Info($"Phase6: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules)
            // ... so that we should now be able to infer all types
            ModuleGenerator.VerifyFunctions(declared);

        logger?.Info($"Phase7: {stopwatch.Elapsed}");

        foreach (var declared in declaredModules) {
            // ... and eventually emit the code and bake the modules
            var compiled = ModuleGenerator.CompileModule(declared);

            context.registry.RegisterModule(compiled);
        }

        logger?.Info($"Finalized: {stopwatch.Elapsed}");

        return context;
    }

    public CompiledKontrolModule CompileTemporary(TO2Module to2Module) {
        var context = new Context(this);
        var declaredKontrolModule = ModuleGenerator.DeclareModule(context, to2Module, "temporary");
        ModuleGenerator.ImportTypes(declaredKontrolModule);
        ModuleGenerator.DeclareConstants(declaredKontrolModule);
        ModuleGenerator.ImportConstants(declaredKontrolModule);
        ModuleGenerator.DeclareFunctions(declaredKontrolModule);
        ModuleGenerator.ImportFunctions(declaredKontrolModule);
        ModuleGenerator.VerifyFunctions(declaredKontrolModule);

        return ModuleGenerator.CompileModule(declaredKontrolModule);
    }

    public IAnyFuture RunREPL(IContext context, string repl) {
        //        try {
        var result = TO2ParserREPL.REPLItems.Parse(repl);

        var to2Module = new TO2Module("<repl>", "", [
            .. result.OfType<UseDeclaration>(),
            .. result.OfType<TypeAlias>(),
            new FunctionDeclaration(FunctionModifier.Public, true, "REPLMain", "", [], BuiltinType.Any,
                new Block([.. result.OfType<IBlockItem>()]), new Position("<inline>"), new Position("<inline>"))
        ]);

        var compiled = CompileTemporary(to2Module);
        var replMain = compiled.FindFunction("REPLMain")!.PreferAsync!;

        return (IAnyFuture)replMain.Invoke(context);
        //        } catch (Exception e) {
        //            return new Future.Success<Runtime.Result<object?>>(
        //                new Runtime.Result<object?>(false, null, new CoreError.Error(e.Message, [])));
        //        }
    }
}
