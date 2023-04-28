using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class IndexAssign : Expression {
        private readonly Expression target;
        private readonly IndexSpec indexSpec;
        private readonly Operator op;
        private readonly Expression expression;

        public IndexAssign(Expression target, IndexSpec indexSpec, Operator op, Expression expression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.target = target;
            this.indexSpec = indexSpec;
            this.op = op;
            this.expression = expression;
        }

        public override IVariableContainer VariableContainer {
            set {
                target.VariableContainer = value;
                indexSpec.VariableContainer = value;
                expression.VariableContainer = value;
            }
        }

        public override TO2Type ResultType(IBlockContext context) =>
            target.ResultType(context)?.AllowedIndexAccess(context.ModuleContext, indexSpec)?.TargetType ??
            BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
            target.Prepare(context);
            indexSpec.start.Prepare(context);
            expression.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type valueType = expression.ResultType(context);
            TO2Type targetType = target.ResultType(context);
            IIndexAccessEmitter indexAccess = targetType.AllowedIndexAccess(context.ModuleContext, indexSpec);

            if (indexAccess == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoIndexAccess,
                    $"Type '{targetType.Name}' does not support access by index",
                    Start,
                    End
                ));
                return;
            }

            if (target is IAssignContext assignContext) {
                if (assignContext.IsConst(context)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.NoSuchField,
                        $"Type '{targetType.Name}' elements can not be set on a read-only variable",
                        Start,
                        End
                    ));
                    return;
                }
            } else {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    $"Index assign '{targetType.Name}'.'{indexSpec}' on invalid target expression",
                    Start,
                    End
                ));
                return;
            }

            if (op == Operator.Assign) {
                expression.Prepare(context);

                if (indexAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                if (!dropResult) {
                    using ITempBlockVariable tmpResult =
                        context.MakeTempVariable(indexAccess.TargetType.UnderlyingType(context.ModuleContext));
                    indexAccess.EmitStore(context, subContext => {
                        expression.EmitCode(subContext, false);
                        indexAccess.TargetType.AssignFrom(subContext.ModuleContext, valueType)
                            .EmitConvert(subContext);

                        context.IL.Emit(OpCodes.Dup);
                        // ReSharper disable once AccessToDisposedClosure
                        tmpResult.EmitStore(subContext);
                    });

                    tmpResult.EmitLoad(context);
                } else {
                    indexAccess.EmitStore(context, subContext => {
                        expression.EmitCode(subContext, false);
                        indexAccess.TargetType.AssignFrom(subContext.ModuleContext, valueType).EmitConvert(subContext);
                    });
                }
            } else {
                IOperatorEmitter operatorEmitter = indexAccess.TargetType.AllowedSuffixOperators(context.ModuleContext)
                    .GetMatching(context.ModuleContext, op, valueType);

                if (operatorEmitter == null) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"Index assign '{targetType.Name}'.'{indexSpec}': Cannot {op} a {indexAccess.TargetType} with a {valueType}",
                        Start,
                        End
                    ));
                    return;
                }

                expression.Prepare(context);

                if (indexAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                if (!dropResult) {
                    using ITempBlockVariable tmpResult =
                        context.MakeTempVariable(indexAccess.TargetType.UnderlyingType(context.ModuleContext));
                    indexAccess.EmitStore(context, subContext => {
                        if (indexAccess.RequiresPtr) target.EmitPtr(context);
                        else target.EmitCode(context, false);

                        indexAccess.EmitLoad(context);
                        expression.EmitCode(subContext, false);

                        operatorEmitter.OtherType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context);
                        operatorEmitter.EmitCode(context, this);

                        context.IL.Emit(OpCodes.Dup);
                        // ReSharper disable once AccessToDisposedClosure
                        tmpResult.EmitStore(subContext);
                    });

                    tmpResult.EmitLoad(context);
                } else {
                    indexAccess.EmitStore(context, subContext => {
                        if (indexAccess.RequiresPtr) target.EmitPtr(context);
                        else target.EmitCode(context, false);

                        indexAccess.EmitLoad(context);
                        expression.EmitCode(subContext, false);

                        operatorEmitter.OtherType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context);
                        operatorEmitter.EmitCode(context, this);
                    });
                }
            }
        }
        public override REPLValueFuture Eval(REPLContext context) {
            var targetFuture = target.Eval(context);
            var valueFuture = expression.Eval(context);

            IIndexAccessEmitter indexAccess = targetFuture.Type.AllowedIndexAccess(context.replModuleContext, indexSpec);

            if (indexAccess == null) {
                throw new REPLException(this, $"Type '{targetFuture.Type.Name}' does not support access by index");
            }

            if (target is IAssignContext assignContext) {
                if (assignContext.IsConst(context.replBlockContext)) {
                    throw new REPLException(this,
                        $"Type '{targetFuture.Type.Name}' elements can not be set on a read-only variable");
                }
            } else {
                throw new REPLException(this,
                    $"Index assign '{targetFuture.Type.Name}'.'{indexSpec}' on invalid target expression");
            }

            if (!indexAccess.TargetType.IsAssignableFrom(context.replModuleContext, valueFuture.Type)) {
                throw new REPLException(this, $"Type '{targetFuture.Type.Name}' element is of type {indexAccess.TargetType} but is assigned to {valueFuture.Type}");
            }

            var assign = indexAccess.TargetType.AssignFrom(context.replModuleContext, valueFuture.Type);

            if (op == Operator.Assign) {
                return REPLValueFuture.Chain2(indexAccess.TargetType, targetFuture, valueFuture,
                    (target, value) => {
                        var converted = assign.EvalConvert(this, value);

                        return indexAccess.EvalAssign(this, context, target, converted);
                    });
            }

            IOperatorEmitter operatorEmitter = indexAccess.TargetType.AllowedSuffixOperators(context.replModuleContext)
                .GetMatching(context.replModuleContext, op, valueFuture.Type);

            if (operatorEmitter == null) {
                throw new REPLException(this, $"Index assign '{targetFuture.Type.Name}'.'{indexSpec}': Cannot {op} a {indexAccess.TargetType} with a {valueFuture.Type}");
            }

            return REPLValueFuture.Chain2(indexAccess.TargetType, targetFuture, valueFuture,
                (target, value) => indexAccess.EvalGet(this, context, target).Then(indexAccess.TargetType,
                        prevValue => {
                            var converted = assign.EvalConvert(this, operatorEmitter.Eval(this, prevValue, value));
                            return indexAccess.EvalAssign(this, context, target, converted);
                        }));
        }
    }
}
