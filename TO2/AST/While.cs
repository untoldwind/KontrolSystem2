using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class While : Expression, IVariableContainer {
        private readonly Expression condition;
        private readonly Expression loopExpression;
        private IVariableContainer parentContainer;

        public While(Expression condition, Expression loopExpression, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.condition = condition;
            this.condition.TypeHint = _ => BuiltinType.Bool;
            this.loopExpression = loopExpression;
        }

        public override IVariableContainer VariableContainer {
            set {
                parentContainer = value;
                condition.VariableContainer = value;
                loopExpression.VariableContainer = this;
            }
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) =>
            condition.GetScopeVariables(context)?.Get(name);

        public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (condition.ResultType(context) != BuiltinType.Bool) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        "Condition of while is not a boolean",
                        Start,
                        End
                    )
                );
            }

            IBlockContext tmpContext =
                context.CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
            Dictionary<string, TO2Type> scopeVariables = condition.GetScopeVariables(tmpContext);

            if (scopeVariables != null) {
                foreach (var (name, type) in scopeVariables) {
                    tmpContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));
                }
            }

            ILCount conditionCount = condition.GetILCount(tmpContext, false);
            ILCount loopCount = loopExpression.GetILCount(tmpContext, true);

            if (loopCount.stack > 0) {
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.CoreGeneration,
                        "Body of the while expression leaves values on stack. This must not happen",
                        Start,
                        End
                    )
                );
                return;
            }

            if (context.HasErrors) return;

            using ITempLocalRef loopCounter = context.IL.TempLocal(typeof(int));
            LabelRef whileStart = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 110);
            LabelRef whileEnd = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 110);
            LabelRef whileLoop = context.IL.DefineLabel(conditionCount.opCodes + loopCount.opCodes < 100);
            IBlockContext loopContext = context.CreateLoopContext(whileStart, whileEnd);

            if (scopeVariables != null) {
                foreach (var (name, type) in scopeVariables) {
                    if (loopContext.FindVariable(name) != null) {
                        loopContext.AddError(new StructuralError(
                            StructuralError.ErrorType.DuplicateVariableName,
                            $"Variable '{name}' already declared in this scope",
                            Start,
                            End
                        ));
                        return;
                    }

                    loopContext.DeclaredVariable(name, true, type.UnderlyingType(context.ModuleContext));
                }
            }

            loopContext.IL.Emit(whileStart.isShort ? OpCodes.Br_S : OpCodes.Br, whileStart);
            context.IL.MarkLabel(whileLoop);

            // Timeout check
            LabelRef skipCheck = context.IL.DefineLabel(true);
            loopCounter.EmitLoad(loopContext);
            loopContext.IL.Emit(OpCodes.Ldc_I4_1);
            loopContext.IL.Emit(OpCodes.Add);
            loopContext.IL.Emit(OpCodes.Dup);
            loopCounter.EmitStore(loopContext);
            loopContext.IL.Emit(OpCodes.Ldc_I4, 10000);
            loopContext.IL.Emit(OpCodes.Cgt);
            loopContext.IL.Emit(OpCodes.Brfalse, skipCheck);
            loopContext.IL.Emit(OpCodes.Ldc_I4_0);
            loopCounter.EmitStore(loopContext);
            context.IL.EmitCall(OpCodes.Call, typeof(Runtime.ContextHolder).GetMethod("CheckTimeout"), 0);
            loopContext.IL.MarkLabel(skipCheck);

            loopExpression.EmitCode(loopContext, true);
            loopContext.IL.MarkLabel(whileStart);
            condition.EmitCode(loopContext, false);

            loopContext.IL.Emit(whileLoop.isShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, whileLoop);

            loopContext.IL.MarkLabel(whileEnd);
            if (!dropResult) context.IL.Emit(OpCodes.Ldnull);
        }
    }
}
