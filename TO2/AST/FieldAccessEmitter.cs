using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public interface IFieldAccessEmitter {
    RealizedType FieldType { get; }

    bool RequiresPtr { get; }

    bool CanStore { get; }

    bool IsAsyncStore { get; }

    void EmitLoad(IBlockContext context);

    void EmitPtr(IBlockContext context);

    void EmitStore(IBlockContext context);

    IREPLValue EvalGet(Node node, IREPLValue target);

    IREPLValue EvalAssign(Node node, IREPLValue target, IREPLValue value);
}

public interface IFieldAccessFactory {
    TO2Type DeclaredType { get; }

    string? Description { get; }

    bool CanStore { get; }

    IFieldAccessEmitter Create(ModuleContext context);

    IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);
}

public delegate IREPLValue REPLFieldAccess(Node node, IREPLValue target);

public class InlineFieldAccessFactory(string description, Func<RealizedType> fieldType, REPLFieldAccess replFieldAccess,
    params OpCode[] opCodes) : IFieldAccessFactory {

    public string Description { get; } = description;

    public TO2Type DeclaredType => fieldType();

    public bool CanStore => false;

    public IFieldAccessEmitter Create(ModuleContext context) {
        return new InlineFieldAccessEmitter(fieldType(), replFieldAccess, opCodes);
    }

    public IFieldAccessFactory
        FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return this;
    }
}

public class InlineFieldAccessEmitter(RealizedType fieldType, REPLFieldAccess replFieldAccess, OpCode[] loadOpCodes)
    : IFieldAccessEmitter {
    public RealizedType FieldType { get; } = fieldType;

    public bool RequiresPtr => false;

    public bool CanStore => false;

    public bool IsAsyncStore => false;

    public void EmitLoad(IBlockContext context) {
        foreach (var opCode in loadOpCodes) context.IL.Emit(opCode);
    }

    public void EmitPtr(IBlockContext context) {
        EmitLoad(context);
        using var tempLocal =
            context.MakeTempVariable(FieldType.UnderlyingType(context.ModuleContext));
        tempLocal.EmitStore(context);
        tempLocal.EmitLoadPtr(context);
    }

    public void EmitStore(IBlockContext context) {
    }

    public IREPLValue EvalGet(Node node, IREPLValue target) => replFieldAccess(node, target);

    public IREPLValue EvalAssign(Node node, IREPLValue target, IREPLValue value) {
        throw new REPLException(node, "Field assign not supported");
    }
}

public class BoundFieldAccessFactory : IFieldAccessFactory {
    private readonly List<FieldInfo> fieldInfos;
    private readonly Type fieldTarget;
    private readonly Func<RealizedType> fieldType;

    public BoundFieldAccessFactory(string description, Func<RealizedType> fieldType, Type fieldTarget,
        FieldInfo fieldInfo) {
        this.Description = description;
        this.fieldType = fieldType;
        this.fieldTarget = fieldTarget;
        fieldInfos = [fieldInfo];
    }

    private BoundFieldAccessFactory(string description, Func<RealizedType> fieldType, Type fieldTarget,
        List<FieldInfo> fieldInfos) {
        this.Description = description;
        this.fieldType = fieldType;
        this.fieldTarget = fieldTarget;
        this.fieldInfos = fieldInfos;
    }

    public TO2Type DeclaredType => fieldType();

    public string Description { get; }

    public bool CanStore => !fieldInfos.Last().IsInitOnly;

    public IFieldAccessEmitter Create(ModuleContext context) => new BoundFieldAccessEmitter(fieldType(), fieldTarget, fieldInfos);

    public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        if (fieldTarget.IsGenericTypeDefinition) {
            var arguments = fieldTarget.GetGenericArguments().Select(t => {
                if (!typeArguments.ContainsKey(t.Name))
                    throw new ArgumentException($"Generic parameter {t.Name} not found");
                return typeArguments[t.Name].GeneratedType(context);
            }).ToArray();
            var genericTarget = fieldTarget.MakeGenericType(arguments);
            var genericFields = new List<FieldInfo>();
            var current = genericTarget;

            foreach (var field in fieldInfos) {
                var genericField = current.GetField(field.Name);
                genericFields.Add(genericField);
                current = genericField.FieldType;
            }

            return new BoundFieldAccessFactory(Description, () => fieldType().FillGenerics(context, typeArguments),
                genericTarget, genericFields);
        }

        return this;
    }
}

public class BoundFieldAccessEmitter(RealizedType fieldType, Type fieldTarget, List<FieldInfo> fieldInfos)
    : IFieldAccessEmitter {
    public RealizedType FieldType { get; } = fieldType;

    public bool RequiresPtr => fieldTarget.IsValueType;

    public bool CanStore => !fieldInfos.Last().IsInitOnly;

    public bool IsAsyncStore => false;

    public void EmitLoad(IBlockContext context) {
        foreach (var fieldInfo in fieldInfos)
            context.IL.Emit(OpCodes.Ldfld, fieldInfo);
    }

    public void EmitPtr(IBlockContext context) {
        foreach (var fieldInfo in fieldInfos)
            context.IL.Emit(OpCodes.Ldflda, fieldInfo);
    }

    public void EmitStore(IBlockContext context) {
        var fieldCount = fieldInfos.Count;

        foreach (var fieldInfo in fieldInfos.Take(fieldCount - 1)) context.IL.Emit(OpCodes.Ldflda, fieldInfo);

        context.IL.Emit(OpCodes.Stfld, fieldInfos[fieldCount - 1]);
    }

    public IREPLValue EvalGet(Node node, IREPLValue target) {
        var current = target.Value;

        foreach (var fieldInfo in fieldInfos) current = fieldInfo.GetValue(current);

        return FieldType.REPLCast(current);
    }

    public IREPLValue EvalAssign(Node node, IREPLValue target, IREPLValue value) {
        var fieldCount = fieldInfos.Count;
        var current = target.Value;

        foreach (var fieldInfo in fieldInfos.Take(fieldCount - 1)) current = fieldInfo.GetValue(current);

        fieldInfos[fieldCount - 1].SetValue(current, value.Value);

        return FieldType.REPLCast(value.Value);
    }
}

public class BoundPropertyLikeFieldAccessFactory : IFieldAccessFactory {
    private readonly Func<RealizedType> fieldType;
    private readonly MethodInfo getter;
    private readonly bool isAsyncStore;
    private readonly Type methodTarget;
    private readonly OpCode[] opCodes;
    private readonly MethodInfo? setter;

    public BoundPropertyLikeFieldAccessFactory(string? description, Func<RealizedType> fieldType, Type methodTarget,
        MethodInfo? getter, MethodInfo? setter, params OpCode[] opCodes) {
        this.Description = description;
        this.fieldType = fieldType;
        this.methodTarget = methodTarget;
        this.getter = getter ??
                      throw new ArgumentException($"Getter is null for {description} in type {fieldType}");
        this.setter = setter;
        isAsyncStore = false;
        this.opCodes = opCodes;
    }

    private BoundPropertyLikeFieldAccessFactory(string? description, Func<RealizedType> fieldType, Type methodTarget,
        MethodInfo? getter, MethodInfo? setter, bool isAsyncStore, params OpCode[] opCodes) {
        this.Description = description;
        this.fieldType = fieldType;
        this.methodTarget = methodTarget;
        this.getter = getter ??
                      throw new ArgumentException($"Getter is null for {description} in type {fieldType}");
        this.setter = setter;
        this.isAsyncStore = isAsyncStore;
        this.opCodes = opCodes;
    }

    public BoundPropertyLikeFieldAccessFactory(string description, Func<RealizedType> fieldType, Type methodTarget,
        PropertyInfo? propertyInfo, params OpCode[] opCodes) {
        if (propertyInfo == null)
            throw new ArgumentException($"PropertyInfo is null for {description} in type {fieldType}");
        this.Description = description;
        this.fieldType = fieldType;
        this.methodTarget = methodTarget;
        getter = propertyInfo.GetMethod;
        setter = propertyInfo.SetMethod;
        isAsyncStore = false;
        this.opCodes = opCodes;
    }

    public BoundPropertyLikeFieldAccessFactory(string? description, Func<RealizedType> fieldType, Type methodTarget,
        PropertyInfo propertyInfo, bool isAsyncStore, params OpCode[] opCodes) {
        if (propertyInfo == null)
            throw new ArgumentException($"PropertyInfo is null for {description} in type {fieldType}");
        this.Description = description;
        this.fieldType = fieldType;
        this.methodTarget = methodTarget;
        getter = propertyInfo.GetMethod;
        setter = propertyInfo.SetMethod;
        this.isAsyncStore = isAsyncStore;
        this.opCodes = opCodes;
    }

    public TO2Type DeclaredType => fieldType();

    public string? Description { get; }

    public bool CanStore => setter != null;

    public IFieldAccessEmitter Create(ModuleContext context) =>
        new BoundPropertyLikeFieldAccessEmitter(fieldType(), methodTarget, getter, setter, isAsyncStore,
            opCodes);

    public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        if (methodTarget.IsGenericTypeDefinition) {
            var arguments = methodTarget.GetGenericArguments().Select(t => {
                if (!typeArguments.ContainsKey(t.Name))
                    throw new ArgumentException($"Generic parameter {t.Name} not found");
                return typeArguments[t.Name].GeneratedType(context);
            }).ToArray();
            var genericTarget = methodTarget.MakeGenericType(arguments);
            var genericGetterMethod = genericTarget.GetMethod(getter.Name, Type.EmptyTypes);

            if (genericGetterMethod == null)
                throw new ArgumentException(
                    $"Unable to relocate method {getter.Name} on {methodTarget} for type arguments {typeArguments}");

            MethodInfo? genericSetterMethod = null;

            if (setter != null) {
                genericSetterMethod = genericTarget.GetMethod(setter.Name, [genericGetterMethod.ReturnType]);

                if (genericSetterMethod == null)
                    throw new ArgumentException(
                        $"Unable to relocate method {setter.Name} on {methodTarget} for type arguments {typeArguments}");
            }

            return new BoundPropertyLikeFieldAccessFactory(Description,
                () => fieldType().FillGenerics(context, typeArguments), genericTarget, genericGetterMethod,
                genericSetterMethod, isAsyncStore, opCodes);
        }

        return this;
    }
}

public class BoundPropertyLikeFieldAccessEmitter(
    RealizedType fieldType,
    Type methodTarget,
    MethodInfo getter,
    MethodInfo? setter,
    bool isAsyncStore,
    OpCode[] opCodes)
    : IFieldAccessEmitter {
    public RealizedType FieldType { get; } = fieldType;

    public bool CanStore => setter != null;

    public bool IsAsyncStore { get; } = isAsyncStore;

    public bool RequiresPtr =>
        methodTarget.IsValueType && (getter.CallingConvention & CallingConventions.HasThis) != 0;

    public void EmitLoad(IBlockContext context) {
        context.IL.EmitCall(getter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getter, 1);
        foreach (var opCode in opCodes) context.IL.Emit(opCode);
    }

    public void EmitPtr(IBlockContext context) {
        EmitLoad(context);
        using var tempLocal =
            context.MakeTempVariable(FieldType.UnderlyingType(context.ModuleContext));
        tempLocal.EmitStore(context);
        tempLocal.EmitLoadPtr(context);
    }

    public void EmitStore(IBlockContext context) {
        context.IL.EmitCall(getter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setter!, 2);
    }

    public IREPLValue EvalGet(Node node, IREPLValue target) {
        var result = getter.IsStatic
            ? getter.Invoke(null, [target.Value])
            : getter.Invoke(target.Value, []);

        return FieldType.REPLCast(result);
    }

    public IREPLValue EvalAssign(Node node, IREPLValue target, IREPLValue value) {
        if (setter == null)
            throw new REPLException(node, "Field assign not supported");

        if (setter.IsStatic)
            setter.Invoke(null, [target.Value, value.Value]);
        else
            setter.Invoke(target.Value, [value.Value]);

        return FieldType.REPLCast(value.Value);
    }
}
