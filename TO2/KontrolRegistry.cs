using System;
using System.Collections.Generic;
using System.IO;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2 {
    public class KontrolRegistry {
        public readonly SortedDictionary<string, IKontrolModule> modules =
            new SortedDictionary<string, IKontrolModule>();

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
            KontrolRegistry registry = new KontrolRegistry();

            registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreLogging)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreTesting)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(CoreBackground)));
            registry.RegisterModule(DirectBindingMath.Module);

            return registry;
        }

        public Context AddFile(string baseDir, string file) {
            TO2Module to2Module = TO2Parser.ParseModuleFile(baseDir, file);
            Context context = new Context(this);
            DeclaredKontrolModule declaredKontrolModule = ModuleGenerator.DeclareModule(context, to2Module);
            ModuleGenerator.ImportTypes(declaredKontrolModule);
            ModuleGenerator.DeclareFunctions(declaredKontrolModule);
            ModuleGenerator.ImportFunctions(declaredKontrolModule);
            ModuleGenerator.VerifyFunctions(declaredKontrolModule);
            CompiledKontrolModule kontrolModule = ModuleGenerator.CompileModule(declaredKontrolModule);

            RegisterModule(kontrolModule);

            return context;
        }

        public Context AddDirectory(string baseDir) {
            Context context = new Context(this);
            List<DeclaredKontrolModule> declaredModules = new List<DeclaredKontrolModule>();

            foreach (string fileName in Directory.GetFiles(baseDir, "*.to2", SearchOption.AllDirectories)) {
                // First declare the existence of a module and its types
                TO2Module to2Module = TO2Parser.ParseModuleFile(baseDir, fileName.Remove(0, baseDir.Length + 1));
                DeclaredKontrolModule module = ModuleGenerator.DeclareModule(context, to2Module);

                declaredModules.Add(module);
                context.registry.RegisterModule(module);
            }

            foreach (DeclaredKontrolModule declared in declaredModules) {
                // ... so that types can be imported by other modules
                ModuleGenerator.ImportTypes(declared);
            }

            foreach (DeclaredKontrolModule declared in declaredModules) {
                // ... so that function can be declared (potentially using imported types as arguments or return)
                ModuleGenerator.DeclareFunctions(declared);
            }

            foreach (DeclaredKontrolModule declared in declaredModules) {
                // ... so that other modules may import these functions
                ModuleGenerator.ImportFunctions(declared);
            }

            foreach (DeclaredKontrolModule declared in declaredModules) {
                // ... so that we should now be able to infer all types
                ModuleGenerator.VerifyFunctions(declared);
            }

            foreach (DeclaredKontrolModule declared in declaredModules) {
                ModuleGenerator.CompileStructs(declared);
            }

            foreach (DeclaredKontrolModule declared in declaredModules) {
                // ... and eventually emit the code and bake the modules
                CompiledKontrolModule compiled = ModuleGenerator.CompileModule(declared);

                context.registry.RegisterModule(compiled);
            }

            return context;
        }
    }
}
