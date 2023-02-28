using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public readonly struct RecordStructField {
        public readonly string name;
        public readonly RealizedType type;
        public readonly FieldInfo field;
        public readonly string description;

        public RecordStructField(string name, string description, RealizedType type, FieldInfo field) {
            this.name = name;
            this.description = description;
            this.type = type;
            this.field = field;
        }
    }

    public class RecordStructType : RecordType {
        private readonly string modulePrefix;
        private readonly string localName;
        public override string Description { get; }
        public Type runtimeType;
        private readonly SortedDictionary<string, TO2Type> itemTypes;
        internal readonly SortedDictionary<string, FieldInfo> fields;
        private readonly OperatorCollection allowedPrefixOperators;
        private readonly Dictionary<string, IFieldAccessFactory> allowedFields;
        private readonly ConstructorInfo constructor;
        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods { get; }

        public RecordStructType(string modulePrefix, string localName, string description, Type runtimeType,
            IEnumerable<RecordStructField> fields,
            OperatorCollection allowedPrefixOperators,
            OperatorCollection allowedSuffixOperators,
            Dictionary<string, IMethodInvokeFactory> allowedMethods,
            Dictionary<string, IFieldAccessFactory> allowedFields,
            ConstructorInfo constructor = null) : base(allowedSuffixOperators) {
            this.modulePrefix = modulePrefix;
            this.localName = localName;
            Description = description;
            this.runtimeType = runtimeType;
            this.allowedPrefixOperators = allowedPrefixOperators;
            DeclaredMethods = allowedMethods;
            this.allowedFields = allowedFields;
            this.constructor = constructor;
            itemTypes = new SortedDictionary<string, TO2Type>();
            this.fields = new SortedDictionary<string, FieldInfo>();
            foreach (var f in fields) {
                itemTypes.Add(f.name, f.type);
                this.fields.Add(f.name, f.field);
                this.allowedFields.Add(f.name,
                    new BoundFieldAccessFactory(f.description, () => f.type, runtimeType, f.field));
            }
        }

        internal void AddField(RecordStructField field) {
            itemTypes.Add(field.name, field.type);
            fields.Add(field.name, field.field);
            allowedFields.Add(field.name,
                new BoundFieldAccessFactory(field.description, () => field.type, runtimeType, field.field));
        }

        public override SortedDictionary<string, TO2Type> ItemTypes => itemTypes;

        public override string Name => modulePrefix + "::" + localName;

        public override string LocalName => localName;

        public override bool IsValid(ModuleContext context) => true;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => runtimeType;

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => allowedPrefixOperators;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) =>
            null; // TODO: Actually this should be allowed

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
            Type generatedType = GeneratedType(context);
            Type generatedOther = otherType.GeneratedType(context);

            return otherType is RecordType otherRecordType && generatedType != generatedOther
                ? new AssignRecordStruct(this, otherRecordType)
                : DefaultAssignEmitter.Instance;
        }

        public override void EmitInitialize(IBlockContext context, IBlockVariable variable) {
            if (runtimeType.IsValueType) return;

            context.IL.EmitNew(OpCodes.Newobj, constructor, 0, 1);
            variable.EmitStore(context);
        }

        internal override IOperatorEmitter CombineFrom(RecordType otherType) => new AssignRecordStruct(this, otherType);
    }

    internal class AssignRecordStruct : RecordTypeAssignEmitter<RecordStructType> {
        internal AssignRecordStruct(RecordStructType targetType, RecordType sourceType) : base(targetType, sourceType) {
        }

        protected override void EmitAssignToPtr(IBlockContext context, IBlockVariable tempSource) {
            foreach (var kv in targetType.fields) {
                IFieldAccessFactory sourceFieldFactory = sourceType.FindField(context.ModuleContext, kv.Key);
                if (sourceFieldFactory == null) continue;

                IFieldAccessEmitter sourceField = sourceFieldFactory.Create(context.ModuleContext);
                context.IL.Emit(OpCodes.Dup);
                if (sourceField.RequiresPtr) tempSource.EmitLoadPtr(context);
                else tempSource.EmitLoad(context);
                sourceField.EmitLoad(context);
                targetType.ItemTypes[kv.Key].AssignFrom(context.ModuleContext, sourceType.ItemTypes[kv.Key])
                    .EmitConvert(context);
                context.IL.Emit(OpCodes.Stfld, kv.Value);
            }

            context.IL.Emit(OpCodes.Pop);
        }
    }
}
