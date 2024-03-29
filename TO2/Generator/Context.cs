﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator;

public class Context {
    private readonly AssemblyBuilder assemblyBuilder;
    public readonly ModuleBuilder moduleBuilder;

    public readonly KontrolRegistry registry;

    public Context(KontrolRegistry registry) {
        var id = Guid.NewGuid().ToString("N");
        var assemblyName = new AssemblyName("KontrolTO2Generated" + id);

        this.registry = registry;
#if (DEBUG && NETFRAMEWORK)
            AppDomain appDomain = AppDomain.CurrentDomain;
            assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            moduleBuilder =
                assemblyBuilder.DefineDynamicModule("KontrolTO2.Generated" + id, "KontrolTO2" + id + ".dll");
#else
        assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        moduleBuilder = assemblyBuilder.DefineDynamicModule("KontrolTO2.Generated" + id);
#endif
    }

    public ModuleContext CreateModuleContext(string moduleName) {
        return new ModuleContext(this, moduleName);
    }

    public void Save(string fileName) {
#if (DEBUG && NETFRAMEWORK)
            // Very helpful for deep dive debugging. Unluckily was dropped in .NET 5 onward (and .netstandard).
            // .NET 9 will probably re-introduce it again
            assemblyBuilder.Save(fileName);
#endif
    }
}
