﻿using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ForIn : Expression, IVariableContainer {
        private readonly string variableName;
        private readonly TO2Type variableType;
        private readonly Expression sourceExpression;
        private readonly Expression loopExpression;

        private IVariableContainer parentContainer;

        public ForIn(string variableName, TO2Type variableType, Expression sourceExpression, Expression loopExpression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.variableName = variableName;
            this.variableType = variableType;
            this.sourceExpression = sourceExpression;
            if (this.variableType != null)
                this.sourceExpression.TypeHint = context =>
                    new ArrayType(this.variableType.UnderlyingType(context.ModuleContext));
            this.loopExpression = loopExpression;
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) {
            if (name != variableName) return null;
            return variableType ?? sourceExpression.ResultType(context)?.ForInSource(context.ModuleContext, null)
                .ElementType;
        }

        public override IVariableContainer VariableContainer {
            set {
                parentContainer = value;
                sourceExpression.VariableContainer = this;
                loopExpression.VariableContainer = this;
            }
        }

        public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            RealizedType sourceType = sourceExpression.ResultType(context).UnderlyingType(context.ModuleContext);
            IForInSource source = sourceType.ForInSource(context.ModuleContext, variableType);

            if (source == null)
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"{sourceType} cannot be use as for ... in source",
                        Start,
                        End
                    )
                );
            if (context.FindVariable(variableName) != null)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.DuplicateVariableName,
                    $"Variable '{variableName}' already declared in this scope",
                    Start,
                    End
                ));
            if (source != null && variableType != null &&
                !variableType.IsAssignableFrom(context.ModuleContext, source.ElementType))
                context.AddError(
                    new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"{sourceType} has elements of type {source.ElementType}, expected {variableType}",
                        Start,
                        End
                    )
                );

            if (context.HasErrors) return;

            using ITempLocalRef loopCounter = context.IL.TempLocal(typeof(int));
            ILCount loopSize = EstimateLoop(context, source);
            LabelRef start = context.IL.DefineLabel(loopSize.opCodes < 110);
            LabelRef end = context.IL.DefineLabel(loopSize.opCodes < 110);
            LabelRef loop = context.IL.DefineLabel(loopSize.opCodes < 100);

            IBlockContext loopContext = context.CreateLoopContext(start, end);
            IBlockVariable loopVariable = loopContext.DeclaredVariable(variableName, true, source!.ElementType);

            sourceExpression.EmitCode(context, false);

            if (context.HasErrors) return;

            source.EmitInitialize(loopContext);
            loopContext.IL.Emit(start.isShort ? OpCodes.Br_S : OpCodes.Br, start);

            loopContext.IL.MarkLabel(loop);

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

            source.EmitNext(loopContext);
            loopVariable.EmitStore(loopContext);
            loopExpression.EmitCode(loopContext, true);
            loopContext.IL.MarkLabel(start);
            source.EmitCheckDone(loopContext, loop);
            loopContext.IL.MarkLabel(end);
            if (!dropResult) context.IL.Emit(OpCodes.Ldnull);
        }

        private ILCount EstimateLoop(IBlockContext context, IForInSource source) {
            IBlockContext prepContext = context.CloneCountingContext();

            sourceExpression.EmitCode(prepContext, false);
            source.EmitInitialize(prepContext);

            IBlockContext countingContext = prepContext.CloneCountingContext()
                .CreateLoopContext(context.IL.DefineLabel(false), context.IL.DefineLabel(false));
            IBlockVariable loopVariable = countingContext.DeclaredVariable(variableName, true, source.ElementType);
            LabelRef loop = countingContext.IL.DefineLabel(false);

            source.EmitNext(countingContext);
            loopVariable.EmitStore(countingContext);
            loopExpression.EmitCode(countingContext, true);
            source.EmitCheckDone(countingContext, loop);

            return new ILCount {
                opCodes = countingContext.IL.ILSize,
                stack = countingContext.IL.StackCount
            };
        }
        
        public override REPLValueFuture Eval(REPLContext context) {
            if (context.FindVariable(variableName) != null)
                throw new REPLException(this, $"Variable '{variableName}' already declared in this scope");

            return new REPLForInFuture(context.CreateChildContext(), variableName, variableType, sourceExpression, loopExpression);
        }

        internal class REPLForInFuture : REPLValueFuture {
            private readonly REPLContext context;
            private readonly string variableName;
            private readonly TO2Type variableType;
            private REPLContext.REPLVariable variable;
            private readonly Expression sourceExpression;
            private REPLValueFuture sourceFuture;
            private IREPLForInSource source;
            private IREPLValue current;
            private readonly Expression loopExpression;
            private REPLValueFuture loopExpressionFuture;
            
            public REPLForInFuture(REPLContext context, string variableName, TO2Type variableType, Expression sourceExpression, Expression loopExpression) : base(BuiltinType.Unit) {
                this.context = context;
                this.variableName = variableName;
                this.variableType = variableType;
                this.sourceExpression = sourceExpression;
                this.loopExpression = loopExpression;
            }

            public override FutureResult<IREPLValue> PollValue() {
                sourceFuture ??= sourceExpression.Eval(context);
                if (source == null) {
                    var sourceResult = sourceFuture.PollValue();

                    if (!sourceResult.IsReady) return new FutureResult<IREPLValue>();

                    source = sourceResult.value.ForInSource();

                    if (source == null) {
                        throw new REPLException(sourceExpression,
                            $"{sourceFuture.Type} cannot be use as for ... in source");
                    }

                    if (variableType != null) {
                        if(!variableType.IsAssignableFrom(context.replModuleContext, source.ElementType))
                            throw new REPLException(sourceExpression,
                            $"{sourceFuture.Type} has elements of type {source.ElementType}, expected {variableType}");
                        variable = context.DeclaredVariable(variableName, true, variableType);
                    } else {
                        variable = context.DeclaredVariable(variableName, true, source.ElementType);
                    }
                }

                if (current == null) {
                    current = source.Next();
                    
                    if(current == null) return new FutureResult<IREPLValue>(REPLUnit.INSTANCE);

                    variable.value = variable.declaredType.REPLCast(current.Value);
                } 

                loopExpressionFuture ??= loopExpression.Eval(context);
                var loopExpressionResult = loopExpressionFuture.PollValue();
                
                if(!loopExpressionResult.IsReady) return new FutureResult<IREPLValue>();
                
                if(loopExpressionResult.value.IsBreak) return new FutureResult<IREPLValue>(REPLUnit.INSTANCE);
                if(loopExpressionResult.value.IsReturn) return new FutureResult<IREPLValue>(loopExpressionResult.value);
                loopExpressionFuture = null;
                current = null;
                
                return new FutureResult<IREPLValue>();
            }
        }
    }
}
