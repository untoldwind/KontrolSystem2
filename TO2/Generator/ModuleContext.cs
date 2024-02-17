using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Generator;

public class ModuleContext {
    public readonly ConstructorBuilder? constructorBuilder;
    public readonly IILEmitter? constructorEmitter;
    public readonly List<(string alias, TO2Type type)>? exportedTypes;
    public readonly Dictionary<string, IKontrolConstant> mappedConstants;
    public readonly Dictionary<string, IKontrolFunction> mappedFunctions;
    public readonly Dictionary<string, TO2Type> mappedTypes;
    public readonly Dictionary<string, string> moduleAliases;
    public readonly string? moduleName;
    public readonly Context root;
    private readonly Dictionary<string, TypeBuilder> subTypes;
    public readonly TypeBuilder typeBuilder;

    protected ModuleContext() {
        // This is only needed for testing
        root = null!;
        typeBuilder = null!;
        moduleAliases = new Dictionary<string, string>();
        mappedTypes = new Dictionary<string, TO2Type> {
            { "ArrayBuilder", BuiltinType.ArrayBuilder },
            { "Cell", BuiltinType.Cell }
        };
        exportedTypes = new List<(string alias, TO2Type type)>();
        mappedConstants = new Dictionary<string, IKontrolConstant>();
        mappedFunctions = new Dictionary<string, IKontrolFunction>();
        subTypes = new Dictionary<string, TypeBuilder>();
    }

    internal ModuleContext(Context root, string moduleName) {
        this.root = root;
        this.moduleName = moduleName;
        typeBuilder = this.root.moduleBuilder.DefineType(this.moduleName.ToUpperInvariant().Replace(':', '_'),
            TypeAttributes.Public);
        moduleAliases = new Dictionary<string, string>();
        mappedTypes = new Dictionary<string, TO2Type> {
            { "ArrayBuilder", BuiltinType.ArrayBuilder },
            { "Cell", BuiltinType.Cell }
        };
        exportedTypes = new List<(string alias, TO2Type type)>();
        mappedConstants = new Dictionary<string, IKontrolConstant>();
        mappedFunctions = new Dictionary<string, IKontrolFunction>();
        subTypes = new Dictionary<string, TypeBuilder>();

        constructorBuilder = typeBuilder.DefineTypeInitializer();
        constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());
    }

    private ModuleContext(ModuleContext parent, string subTypeName, Type parentType, Type[] interfaces,
        bool nestedType) {
        root = parent.root;
        moduleName = parent.moduleName;
        typeBuilder = nestedType
            ? parent.typeBuilder.DefineNestedType(subTypeName, TypeAttributes.NestedPublic, parentType, interfaces)
            : root.moduleBuilder.DefineType(
                moduleName?.ToUpperInvariant().Replace(':', '_') + "_" + subTypeName, TypeAttributes.Public);
        moduleAliases = parent.moduleAliases;
        mappedTypes = parent.mappedTypes;
        mappedConstants = parent.mappedConstants;
        mappedFunctions = parent.mappedFunctions;
        subTypes = parent.subTypes;
    }

    public Type CreateType() {
        constructorEmitter?.EmitReturn(typeof(void));
        foreach (var subType in subTypes.Values) subType.CreateType();
        var type = typeBuilder.CreateType();
#if TRIGGER_JIT
            foreach(MethodInfo method in type.GetMethods()) System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method.MethodHandle);
#endif
        return type;
    }

    public IKontrolModule? FindModule(string moduleName) {
        return moduleAliases.ContainsKey(moduleName)
            ? root.registry.modules.Get(moduleAliases[moduleName])
            : root.registry.modules.Get(moduleName);
    }

    public IBlockContext CreateMethodContext(FunctionModifier modifier, bool isAsync, string methodName,
        TO2Type returnType, IEnumerable<FunctionParameter> parameters) {
        return new SyncBlockContext(this, modifier, isAsync, methodName, returnType, parameters.ToList());
    }

    public ModuleContext DefineSiblingContext(string name, Type parentType, params Type[] interfaces) {
        var subContext = new ModuleContext(this, name, parentType, interfaces, false);

        subTypes.Add(name, subContext.typeBuilder);

        return subContext;
    }

    public ModuleContext DefineSubContext(string name, Type parentType, params Type[] interfaces) {
        var subContext = new ModuleContext(this, name, parentType, interfaces, true);

        subTypes.Add(name, subContext.typeBuilder);

        return subContext;
    }

    public (Type future, Type futureResult) FutureTypeOf(TO2Type to2Type) {
        var type = to2Type.GeneratedType(this);
        var parameterType = type == typeof(void) ? typeof(object) : type;

        return (typeof(Future<>).MakeGenericType(parameterType),
            typeof(FutureResult<>).MakeGenericType(parameterType));
    }
}
