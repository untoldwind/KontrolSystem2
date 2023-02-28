using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public abstract class RecordType : RealizedType {
        private readonly IOperatorCollection recordTypeOperators;

        public abstract SortedDictionary<string, TO2Type> ItemTypes { get; }

        protected RecordType(IOperatorCollection allowedSuffixOperators) =>
            recordTypeOperators = new RecordTypeOperators(this, allowedSuffixOperators);

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
            RecordType recordType = otherType.UnderlyingType(context) as RecordType;
            if (recordType == null) return false;
            foreach (var kv in ItemTypes) {
                TO2Type otherItem = recordType.ItemTypes.Get(kv.Key);

                if (otherItem == null || !kv.Value.IsAssignableFrom(context, otherItem)) return false;
            }

            return true;
        }

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => recordTypeOperators;

        internal abstract IOperatorEmitter CombineFrom(RecordType otherType);
    }

    internal class RecordTypeOperators : IOperatorCollection {
        private readonly RecordType recordType;
        private readonly IOperatorCollection allowedOperators;

        internal RecordTypeOperators(RecordType recordType, IOperatorCollection allowedOperators) {
            this.recordType = recordType;
            this.allowedOperators = allowedOperators;
        }

        public IOperatorEmitter GetMatching(ModuleContext context, Operator op, TO2Type otherType) {
            IOperatorEmitter existing = allowedOperators.GetMatching(context, op, otherType);
            if (existing != null) return existing;

            if (op != Operator.BitAnd && op != Operator.BitAndAssign) return null;

            RecordType otherRecordType = otherType.UnderlyingType(context) as RecordType;

            if (otherRecordType == null) return null;

            bool hasMatch = false;
            foreach (var otherKV in otherRecordType.ItemTypes) {
                TO2Type item = recordType.ItemTypes.Get(otherKV.Key);

                if (item == null) continue;
                if (!item.IsAssignableFrom(context, otherKV.Value)) return null;
                hasMatch = true;
            }

            return hasMatch ? recordType.CombineFrom(otherRecordType) : null;
        }
    }

    internal abstract class RecordTypeAssignEmitter<T> : IAssignEmitter, IOperatorEmitter where T : RecordType {
        protected readonly T targetType;
        protected readonly RecordType sourceType;

        protected RecordTypeAssignEmitter(T targetType, RecordType sourceType) {
            this.targetType = targetType;
            this.sourceType = sourceType;
        }

        public TO2Type ResultType => targetType;

        public TO2Type OtherType => sourceType;

        public bool Accepts(ModuleContext context, TO2Type otherType) =>
            sourceType.IsAssignableFrom(context, otherType);

        // ---------------- IOperatorEmitter ----------------
        public void EmitCode(IBlockContext context, Node target) {
            using ITempBlockVariable tempRight = context.MakeTempVariable(sourceType);
            tempRight.EmitStore(context);

            Type generatedType = targetType.GeneratedType(context.ModuleContext);
            using ITempLocalRef someResult = context.IL.TempLocal(generatedType);
            someResult.EmitStore(context);

            someResult.EmitLoadPtr(context);
            EmitAssignToPtr(context, tempRight);
            someResult.EmitLoad(context);
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            using ITempBlockVariable tempRight = context.MakeTempVariable(sourceType);
            tempRight.EmitStore(context);
            context.IL.Emit(OpCodes.Pop); // Left side is just the variable we are about to override

            variable.EmitLoadPtr(context);
            EmitAssignToPtr(context, tempRight);
        }

        public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            this;

        // ---------------- IAssignEmitter -----------------
        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
            using ITempBlockVariable valueTemp = context.MakeTempVariable(sourceType);
            expression.EmitStore(context, valueTemp, true);

            variable.EmitLoadPtr(context);
            EmitAssignToPtr(context, valueTemp);
            if (!dropResult) variable.EmitLoad(context);
        }

        public void EmitConvert(IBlockContext context) {
            using ITempBlockVariable valueTemp = context.MakeTempVariable(sourceType);
            valueTemp.EmitStore(context);

            Type generatedType = targetType.GeneratedType(context.ModuleContext);
            using ITempLocalRef someResult = context.IL.TempLocal(generatedType);
            someResult.EmitLoadPtr(context);
            EmitAssignToPtr(context, valueTemp);
            someResult.EmitLoad(context);
        }

        protected abstract void EmitAssignToPtr(IBlockContext context, IBlockVariable tempSource);
    }
}
