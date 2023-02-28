using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class IfThen : Expression, IVariableContainer {
        private readonly Expression condition;
        private readonly Expression thenExpression;
        private IVariableContainer parentContainer;

        public IfThen(Expression condition, Expression thenExpression, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.condition = condition;
            this.condition.TypeHint = _ => BuiltinType.Bool;
            this.thenExpression = thenExpression;
        }

        public override IVariableContainer VariableContainer {
            set {
                parentContainer = value;
                condition.VariableContainer = value;
                thenExpression.VariableContainer = this;
            }
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) =>
            condition.GetScopeVariables(context)?.Get(name);


        public override TypeHint TypeHint {
            set {
                thenExpression.TypeHint = context =>
                    (value(context) as OptionType)?.elementType.UnderlyingType(context.ModuleContext);
            }
        }

        public override TO2Type ResultType(IBlockContext context) => new OptionType(thenExpression.ResultType(context));

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (condition.ResultType(context) != BuiltinType.Bool) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        "Condition of if is not a boolean",
                        Start,
                        End
                    )
                );
                return;
            }

            IBlockContext thenContext = context.CreateChildContext();
            Dictionary<string, TO2Type> scopeVariables = condition.GetScopeVariables(thenContext);

            if (scopeVariables != null) {
                foreach (var (name, type) in scopeVariables) {
                    if (thenContext.FindVariable(name) != null) {
                        thenContext.AddError(new StructuralError(
                            StructuralError.ErrorType.DuplicateVariableName,
                            $"Variable '{name}' already declared in this scope",
                            Start,
                            End
                        ));
                        return;
                    }

                    thenContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));
                }
            }

            ILCount thenCount = thenExpression.GetILCount(thenContext, true);

            if (!context.HasErrors && thenCount.stack > 0) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        "Then expression leaves values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            condition.EmitCode(thenContext, false);

            if (context.HasErrors) return;

            TO2Type thenResultType = thenExpression.ResultType(thenContext);

            if (dropResult) {
                LabelRef skipThen = context.IL.DefineLabel(thenCount.opCodes < 124);

                thenContext.IL.Emit(skipThen.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipThen);
                thenExpression.EmitCode(thenContext, true);
                thenContext.IL.MarkLabel(skipThen);
            } else {
                OptionType optionType = new OptionType(thenResultType);
                Type generatedType = optionType.GeneratedType(thenContext.ModuleContext);
                using ITempLocalRef tempResult = thenContext.IL.TempLocal(generatedType);
                LabelRef skipThen = thenContext.IL.DefineLabel(thenCount.opCodes < 114);

                thenContext.IL.Emit(skipThen.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipThen);
                thenExpression.Prepare(thenContext);
                tempResult.EmitLoadPtr(context);
                thenContext.IL.Emit(OpCodes.Dup);
                thenContext.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
                thenContext.IL.Emit(OpCodes.Dup);
                thenContext.IL.Emit(OpCodes.Ldc_I4_1);
                thenContext.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
                thenExpression.EmitCode(thenContext, false);
                thenContext.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
                LabelRef ifEnd = context.IL.DefineLabel(true);
                thenContext.IL.Emit(OpCodes.Br_S, ifEnd);

                thenContext.IL.MarkLabel(skipThen);

                tempResult.EmitLoadPtr(context);
                thenContext.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);

                thenContext.IL.MarkLabel(ifEnd);

                tempResult.EmitLoad(thenContext);
            }
        }
    }

    public class IfThenElse : Expression, IVariableContainer {
        private readonly Expression condition;
        private readonly Expression thenExpression;
        private readonly Expression elseExpression;
        private IVariableContainer parentContainer;

        public IfThenElse(Expression condition, Expression thenExpression, Expression elseExpression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.condition = condition;
            this.thenExpression = thenExpression;
            this.elseExpression = elseExpression;
        }

        public override IVariableContainer VariableContainer {
            set {
                parentContainer = value;
                condition.VariableContainer = value;
                thenExpression.VariableContainer = this;
                elseExpression.VariableContainer = value;
            }
        }

        public override TypeHint TypeHint {
            set {
                condition.TypeHint = _ => BuiltinType.Bool;
                thenExpression.TypeHint = value;
                elseExpression.TypeHint = value;
            }
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) =>
            condition.GetScopeVariables(context)?.Get(name);

        public override TO2Type ResultType(IBlockContext context) => thenExpression.ResultType(context);

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (condition.ResultType(context) != BuiltinType.Bool) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        "Condition of if is not a boolean",
                        Start,
                        End
                    )
                );
                return;
            }

            IBlockContext thenContext = context.CreateChildContext();
            IBlockContext elseContext = context.CreateChildContext();

            Dictionary<string, TO2Type> scopeVariables = condition.GetScopeVariables(thenContext);

            if (scopeVariables != null) {
                foreach (var (name, type) in scopeVariables) {
                    if (thenContext.FindVariable(name) != null) {
                        thenContext.AddError(new StructuralError(
                            StructuralError.ErrorType.DuplicateVariableName,
                            $"Variable '{name}' already declared in this scope",
                            Start,
                            End
                        ));
                        return;
                    }

                    thenContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));
                }
            }

            ILCount thenCount = thenExpression.GetILCount(thenContext, dropResult);
            ILCount elseCount = elseExpression.GetILCount(elseContext, dropResult);

            if (!context.HasErrors && thenCount.stack > 1) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        "Then expression leaves too many values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            if (!context.HasErrors && elseCount.stack > 1) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        "Else expression leaves too many values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            condition.EmitCode(thenContext, false);

            if (context.HasErrors) return;

            TO2Type thenType = thenExpression.ResultType(thenContext);
            TO2Type elseType = elseExpression.ResultType(elseContext);
            if (!dropResult) {
                if (!thenType.IsAssignableFrom(context.ModuleContext, elseType)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"If condition has incompatible result {thenType} != {elseType}",
                        Start,
                        End
                    ));
                }
            }

            if (context.HasErrors) return;

            LabelRef thenEnd = context.IL.DefineLabel(thenCount.opCodes < 124);
            LabelRef elseEnd = context.IL.DefineLabel(elseCount.opCodes < 124);

            context.IL.Emit(thenEnd.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, thenEnd);
            thenExpression.EmitCode(thenContext, dropResult);
            context.IL.Emit(elseEnd.isShort ? OpCodes.Br_S : OpCodes.Br, elseEnd);
            context.IL.MarkLabel(thenEnd);
            if (!dropResult) context.IL.AdjustStack(-1); // Then leave its result on the stack, so is else supposed to
            elseExpression.EmitCode(elseContext, dropResult);
            if (!dropResult) thenType.AssignFrom(context.ModuleContext, elseType).EmitConvert(context);
            context.IL.MarkLabel(elseEnd);
        }
    }
}
