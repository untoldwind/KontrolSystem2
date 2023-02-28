using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IFieldAccessEmitter {
        RealizedType FieldType { get; }

        bool RequiresPtr { get; }

        bool CanStore { get; }

        void EmitLoad(IBlockContext context);

        void EmitPtr(IBlockContext context);

        void EmitStore(IBlockContext context);
    }

    public interface IFieldAccessFactory {
        TO2Type DeclaredType { get; }

        string Description { get; }

        IFieldAccessEmitter Create(ModuleContext context);

        IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);
    }

    public class InlineFieldAccessFactory : IFieldAccessFactory {
        private readonly Func<RealizedType> fieldType;
        private readonly OpCode[] opCodes;
        public string Description { get; }

        public InlineFieldAccessFactory(string description, Func<RealizedType> fieldType, params OpCode[] opCodes) {
            Description = description;
            this.fieldType = fieldType;
            this.opCodes = opCodes;
        }

        public TO2Type DeclaredType => fieldType();


        public IFieldAccessEmitter Create(ModuleContext context) => new InlineFieldAccessEmitter(fieldType(), opCodes);

        public IFieldAccessFactory
            FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    public class InlineFieldAccessEmitter : IFieldAccessEmitter {
        private readonly OpCode[] loadOpCodes;
        public RealizedType FieldType { get; }

        public InlineFieldAccessEmitter(RealizedType fieldType, OpCode[] loadOpCodes) {
            FieldType = fieldType;
            this.loadOpCodes = loadOpCodes;
        }

        public bool RequiresPtr => false;

        public bool CanStore => false;

        public void EmitLoad(IBlockContext context) {
            foreach (OpCode opCode in loadOpCodes) {
                context.IL.Emit(opCode);
            }
        }

        public void EmitPtr(IBlockContext context) {
            EmitLoad(context);
            using ITempBlockVariable tempLocal =
                context.MakeTempVariable(FieldType.UnderlyingType(context.ModuleContext));
            tempLocal.EmitStore(context);
            tempLocal.EmitLoadPtr(context);
        }

        public void EmitStore(IBlockContext context) {
        }
    }

    public class BoundFieldAccessFactory : IFieldAccessFactory {
        private readonly Func<RealizedType> fieldType;
        private readonly List<FieldInfo> fieldInfos;
        private readonly Type fieldTarget;
        private readonly string description;

        public BoundFieldAccessFactory(string description, Func<RealizedType> fieldType, Type fieldTarget,
            FieldInfo fieldInfo) {
            this.description = description;
            this.fieldType = fieldType;
            this.fieldTarget = fieldTarget;
            fieldInfos = new List<FieldInfo> { fieldInfo };
        }

        private BoundFieldAccessFactory(string description, Func<RealizedType> fieldType, Type fieldTarget,
            List<FieldInfo> fieldInfos) {
            this.description = description;
            this.fieldType = fieldType;
            this.fieldTarget = fieldTarget;
            this.fieldInfos = fieldInfos;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) =>
            new BoundFieldAccessEmitter(fieldType(), fieldTarget, fieldInfos);

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (fieldTarget.IsGenericTypeDefinition) {
                Type[] arguments = fieldTarget.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name))
                        throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();
                Type genericTarget = fieldTarget.MakeGenericType(arguments);
                List<FieldInfo> genericFields = new List<FieldInfo>();
                Type current = genericTarget;

                foreach (FieldInfo field in fieldInfos) {
                    FieldInfo genericField = current.GetField(field.Name);
                    genericFields.Add(genericField);
                    current = genericField.FieldType;
                }

                return new BoundFieldAccessFactory(description, () => fieldType().FillGenerics(context, typeArguments),
                    genericTarget, genericFields);
            }

            return this;
        }
    }

    public class BoundFieldAccessEmitter : IFieldAccessEmitter {
        private readonly List<FieldInfo> fieldInfos;
        private readonly Type fieldTarget;
        public RealizedType FieldType { get; }

        public BoundFieldAccessEmitter(RealizedType fieldType, Type fieldTarget, List<FieldInfo> fieldInfos) {
            FieldType = fieldType;
            this.fieldTarget = fieldTarget;
            this.fieldInfos = fieldInfos;
        }

        public bool RequiresPtr => fieldTarget.IsValueType;

        public bool CanStore => true;

        public void EmitLoad(IBlockContext context) {
            foreach (FieldInfo fieldInfo in fieldInfos)
                context.IL.Emit(OpCodes.Ldfld, fieldInfo);
        }

        public void EmitPtr(IBlockContext context) {
            foreach (FieldInfo fieldInfo in fieldInfos)
                context.IL.Emit(OpCodes.Ldflda, fieldInfo);
        }

        public void EmitStore(IBlockContext context) {
            var fieldCount = fieldInfos.Count;

            foreach (var fieldInfo in fieldInfos.Take(fieldCount - 1)) {
                context.IL.Emit(OpCodes.Ldflda, fieldInfo);
            }

            context.IL.Emit(OpCodes.Stfld, fieldInfos[fieldCount - 1]);
        }
    }

    public class BoundPropertyLikeFieldAccessFactory : IFieldAccessFactory {
        private readonly Func<RealizedType> fieldType;
        private readonly MethodInfo getter;
        private readonly MethodInfo setter;
        private readonly OpCode[] opCodes;
        private readonly Type methodTarget;
        private readonly string description;

        public BoundPropertyLikeFieldAccessFactory(string description, Func<RealizedType> fieldType, Type methodTarget,
            MethodInfo getter, MethodInfo setter, params OpCode[] opCodes) {
            this.description = description;
            this.fieldType = fieldType;
            this.methodTarget = methodTarget;
            this.getter = getter ??
                          throw new ArgumentException($"Getter is null for {description} in type {fieldType}");
            this.setter = setter;
            this.opCodes = opCodes;
        }

        public BoundPropertyLikeFieldAccessFactory(string description, Func<RealizedType> fieldType, Type methodTarget,
            PropertyInfo propertyInfo, params OpCode[] opCodes) {
            if (propertyInfo == null)
                throw new ArgumentException($"PropertyInfo is null for {description} in type {fieldType}");
            this.description = description;
            this.fieldType = fieldType;
            this.methodTarget = methodTarget;
            getter = propertyInfo.GetMethod;
            setter = propertyInfo.SetMethod;
            this.opCodes = opCodes;
        }

        public TO2Type DeclaredType => fieldType();

        public string Description => description;

        public IFieldAccessEmitter Create(ModuleContext context) =>
            new BoundPropertyLikeFieldAccessEmitter(fieldType(), methodTarget, getter, setter, opCodes);

        public IFieldAccessFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (methodTarget.IsGenericTypeDefinition) {
                Type[] arguments = methodTarget.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name))
                        throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();
                Type genericTarget = methodTarget.MakeGenericType(arguments);
                MethodInfo genericGetterMethod = genericTarget.GetMethod(getter.Name, new Type[0]);

                if (genericGetterMethod == null)
                    throw new ArgumentException(
                        $"Unable to relocate method {getter.Name} on {methodTarget} for type arguments {typeArguments}");

                MethodInfo genericSetterMethod = null;

                if (setter != null) {
                    genericSetterMethod = genericTarget.GetMethod(setter.Name, new[] { genericGetterMethod.ReturnType });

                    if (genericSetterMethod == null)
                        throw new ArgumentException(
                            $"Unable to relocate method {setter.Name} on {methodTarget} for type arguments {typeArguments}");
                }

                return new BoundPropertyLikeFieldAccessFactory(description,
                    () => fieldType().FillGenerics(context, typeArguments), genericTarget, genericGetterMethod,
                    genericSetterMethod, opCodes);
            }

            return this;
        }
    }

    public class BoundPropertyLikeFieldAccessEmitter : IFieldAccessEmitter {
        private readonly MethodInfo getter;
        private readonly MethodInfo setter;
        private readonly OpCode[] opCodes;
        private readonly Type methodTarget;
        public RealizedType FieldType { get; }

        public BoundPropertyLikeFieldAccessEmitter(RealizedType fieldType, Type methodTarget, MethodInfo getter,
            MethodInfo setter,
            OpCode[] opCodes) {
            FieldType = fieldType;
            this.methodTarget = methodTarget;
            this.getter = getter;
            this.setter = setter;
            this.opCodes = opCodes;
        }

        public bool CanStore => setter != null;

        public bool RequiresPtr =>
            methodTarget.IsValueType && (getter.CallingConvention & CallingConventions.HasThis) != 0;

        public void EmitLoad(IBlockContext context) {
            context.IL.EmitCall(getter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getter, 1);
            foreach (OpCode opCode in opCodes) {
                context.IL.Emit(opCode);
            }
        }

        public void EmitPtr(IBlockContext context) {
            EmitLoad(context);
            using ITempBlockVariable tempLocal =
                context.MakeTempVariable(FieldType.UnderlyingType(context.ModuleContext));
            tempLocal.EmitStore(context);
            tempLocal.EmitLoadPtr(context);
        }

        public void EmitStore(IBlockContext context) {
            context.IL.EmitCall(getter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setter, 2);
        }
    }
}
