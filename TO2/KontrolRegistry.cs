using System;
using System.Collections.Generic;
using System.IO;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Runtime.KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2;

public class KontrolRegistry {
    public readonly SortedDictionary<string, IKontrolModule> modules = new();

    public void RegisterModule(IKontrolModule kontrolModule) {
        if (modules.ContainsKey(kontrolModule.Name)) {
            if (!(modules[kontrolModule.Name] is DeclaredKontrolModule))
                throw new ArgumentException($"Module {kontrolModule.Name} is defined twice");
            modules[kontrolModule.Name] = kontrolModule;
        } else {
            modules.Add(kontrolModule.Name, kontrolModule);
        }
    }

    public static KontrolRegistry CreateCore() {
        var registry = new KontrolRegistry();

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

    public Context AddDirectory(string baseDir) {
        var context = new Context(this);
        var declaredModules = new List<DeclaredKontrolModule>();

        foreach (var fileName in Directory.GetFiles(baseDir, "*.to2", SearchOption.AllDirectories)) {
            if (!fileName.EndsWith(".to2")) continue;

            // First declare the existence of a module and its types
            var to2Module = TO2Parser.ParseModuleFile(baseDir, fileName.Remove(0, baseDir.Length + 1));
            var module = ModuleGenerator.DeclareModule(context, to2Module, fileName);

            declaredModules.Add(module);
            context.registry.RegisterModule(module);
        }

        foreach (var declared in declaredModules)
            // ... so that types can be imported by other modules
            ModuleGenerator.ImportTypes(declared);

        foreach (var declared in declaredModules) ModuleGenerator.DeclareConstants(declared);

        foreach (var declared in declaredModules) ModuleGenerator.ImportConstants(declared);

        foreach (var declared in declaredModules)
            // ... so that function can be declared (potentially using imported types as arguments or return)
            ModuleGenerator.DeclareFunctions(declared);

        foreach (var declared in declaredModules)
            // ... so that other modules may import these functions
            ModuleGenerator.ImportFunctions(declared);

        foreach (var declared in declaredModules) ModuleGenerator.CompileStructs(declared);

        foreach (var declared in declaredModules)
            // ... so that we should now be able to infer all types
            ModuleGenerator.VerifyFunctions(declared);

        foreach (var declared in declaredModules) {
            // ... and eventually emit the code and bake the modules
            var compiled = ModuleGenerator.CompileModule(declared);

            context.registry.RegisterModule(compiled);
        }

        return context;
    }
}
