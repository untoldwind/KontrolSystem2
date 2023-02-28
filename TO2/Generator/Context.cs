using System;
using System.Reflection;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator {
    public class Context {
        private readonly AssemblyBuilder assemblyBuilder;
        public readonly ModuleBuilder moduleBuilder;

        public readonly KontrolRegistry registry;

        public Context(KontrolRegistry registry) {
            string id = Guid.NewGuid().ToString("N");
            AppDomain appDomain = AppDomain.CurrentDomain;
            AssemblyName assemblyName = new AssemblyName("KontrolTO2Generated" + id);

            this.registry = registry;
#if DEBUG
            assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            moduleBuilder =
                assemblyBuilder.DefineDynamicModule("KontrolTO2.Generated" + id, "KontrolTO2" + id + ".dll");
#else
            assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("KontrolTO2.Generated" + id);
#endif
        }

        public ModuleContext CreateModuleContext(string moduleName) {
            return new ModuleContext(this, moduleName);
        }

        public void Save(string fileName) {
#if DEBUG
            assemblyBuilder.Save(fileName);
#endif
        }
    }
}
