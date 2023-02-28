using System;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IIndexAccessEmitter {
        TO2Type TargetType { get; }

        bool RequiresPtr { get; }

        void EmitLoad(IBlockContext context);

        void EmitPtr(IBlockContext context);

        void EmitStore(IBlockContext context, Action<IBlockContext> emitValue);
    }

    public class InlineArrayIndexAccessEmitter : IIndexAccessEmitter {
        private readonly RealizedType targetType;
        private readonly Expression indexExpression;

        public InlineArrayIndexAccessEmitter(RealizedType targetType, Expression indexExpression) {
            this.targetType = targetType;
            this.indexExpression = indexExpression;
            this.indexExpression.TypeHint = _ => BuiltinType.Int;
        }

        public TO2Type TargetType => targetType;

        public bool RequiresPtr => false;

        public void EmitLoad(IBlockContext context) {
            TO2Type resultType = indexExpression.ResultType(context);
            if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, resultType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Index has to be of type {BuiltinType.Int}",
                    indexExpression.Start,
                    indexExpression.End
                ));
                return;
            }

            indexExpression.EmitCode(context, false);
            BuiltinType.Int.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);

            context.IL.Emit(OpCodes.Conv_I4);
            if (targetType == BuiltinType.Bool) context.IL.Emit(OpCodes.Ldelem_I4);
            else if (targetType == BuiltinType.Int) context.IL.Emit(OpCodes.Ldelem_I8);
            else if (targetType == BuiltinType.Float) context.IL.Emit(OpCodes.Ldelem_R8);
            else context.IL.Emit(OpCodes.Ldelem, targetType.GeneratedType(context.ModuleContext));
        }

        public void EmitPtr(IBlockContext context) {
            TO2Type resultType = indexExpression.ResultType(context);
            if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, resultType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Index has to be of type {BuiltinType.Int}",
                    indexExpression.Start,
                    indexExpression.End
                ));
                return;
            }

            indexExpression.EmitCode(context, false);
            BuiltinType.Int.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);

            context.IL.Emit(OpCodes.Conv_I4);
            context.IL.Emit(OpCodes.Ldelema, targetType.GeneratedType(context.ModuleContext));
        }

        public void EmitStore(IBlockContext context, Action<IBlockContext> emitValue) {
            TO2Type resultType = indexExpression.ResultType(context);
            if (!BuiltinType.Int.IsAssignableFrom(context.ModuleContext, resultType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Index has to be of type {BuiltinType.Int}",
                    indexExpression.Start,
                    indexExpression.End
                ));
                return;
            }

            indexExpression.EmitCode(context, false);
            BuiltinType.Int.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);
            context.IL.Emit(OpCodes.Conv_I4);

            emitValue(context);

            if (targetType == BuiltinType.Bool) context.IL.Emit(OpCodes.Stelem_I4);
            else if (targetType == BuiltinType.Int) context.IL.Emit(OpCodes.Stelem_I8);
            else if (targetType == BuiltinType.Float) context.IL.Emit(OpCodes.Stelem_R8);
            else context.IL.Emit(OpCodes.Stelem, targetType.GeneratedType(context.ModuleContext));
        }
    }
}
