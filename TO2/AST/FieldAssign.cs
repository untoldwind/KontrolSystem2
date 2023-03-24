﻿using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class FieldAssign : Expression {
        private readonly Expression target;
        private readonly string fieldName;
        private readonly Operator op;
        private readonly Expression expression;

        public FieldAssign(Expression target, string fieldName, Operator op, Expression expression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.target = target;
            this.fieldName = fieldName;
            this.op = op;
            this.expression = expression;
        }

        public override IVariableContainer VariableContainer {
            set {
                target.VariableContainer = value;
                expression.VariableContainer = value;
            }
        }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type targetType = target.ResultType(context);
            IFieldAccessEmitter fieldAccess =
                targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);
            if (fieldAccess == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                    Start,
                    End
                ));
                return BuiltinType.Unit;
            }

            if (!fieldAccess.CanStore) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' field '{fieldName}' is read-only",
                    Start,
                    End
                ));
                return BuiltinType.Unit;
            }

            return fieldAccess.FieldType;
        }

        public override void Prepare(IBlockContext context) {
            target.Prepare(context);
            expression.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type targetType = target.ResultType(context);
            IFieldAccessEmitter fieldAccess =
                targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);
            TO2Type valueType = expression.ResultType(context);

            if (fieldAccess == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                    Start,
                    End
                ));
                return;
            }

            if (!fieldAccess.CanStore) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' field '{fieldName}' is read-only",
                    Start,
                    End
                ));
                return;
            }

            if (target is IAssignContext assignContext) {
                if (fieldAccess.RequiresPtr && assignContext.IsConst(context)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.NoSuchField,
                        $"Type '{targetType.Name}' field '{fieldName}' can not be set on a read-only variable",
                        Start,
                        End
                    ));
                    return;
                }
            } else {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    $"Field assign '{targetType.Name}'.'{fieldName}' on invalid target expression",
                    Start,
                    End
                ));
                return;
            }

            if (!fieldAccess.FieldType.IsAssignableFrom(context.ModuleContext, valueType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Type '{targetType.Name}' field '{fieldName}' is of type {fieldAccess.FieldType} but is assigned to {valueType}",
                    Start,
                    End
                ));
                return;
            }

            if (op == Operator.Assign) {
                if (fieldAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                expression.EmitCode(context, false);
                fieldAccess.FieldType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context);

                if (!dropResult) {
                    using ITempBlockVariable tmpResult = context.MakeTempVariable(fieldAccess.FieldType);

                    context.IL.Emit(OpCodes.Dup);
                    tmpResult.EmitStore(context);

                    fieldAccess.EmitStore(context);

                    tmpResult.EmitLoad(context);
                } else {
                    fieldAccess.EmitStore(context);
                }
            } else {
                IOperatorEmitter operatorEmitter = fieldAccess.FieldType.AllowedSuffixOperators(context.ModuleContext)
                    .GetMatching(context.ModuleContext, op, valueType);

                if (operatorEmitter == null) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.IncompatibleTypes,
                        $"Type '{targetType.Name}' field '{fieldName}': Cannot {op} a {fieldAccess.FieldType} with a {valueType}",
                        Start,
                        End
                    ));
                    return;
                }

                expression.Prepare(context);

                if (fieldAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (fieldAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                fieldAccess.EmitLoad(context);
                expression.EmitCode(context, false);
                operatorEmitter.OtherType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context);
                operatorEmitter.EmitCode(context, this);

                if (!dropResult) {
                    using ITempBlockVariable tmpResult = context.MakeTempVariable(fieldAccess.FieldType);

                    context.IL.Emit(OpCodes.Dup);
                    tmpResult.EmitStore(context);

                    fieldAccess.EmitStore(context);

                    tmpResult.EmitLoad(context);
                } else {
                    fieldAccess.EmitStore(context);
                }
            }
        }
        
        public override REPLValueFuture Eval(REPLContext context) {
            var targetFuture = target.Eval(context);
            var valueFuture = expression.Eval(context);
            
            IFieldAccessEmitter fieldAccess =
                targetFuture.Type.FindField(context.replModuleContext, fieldName)?.Create(context.replModuleContext);

            if (fieldAccess == null) {
                throw new REPLException(this, $"Type '{targetFuture.Type.Name}' does not have a field '{fieldName}'");
            }
            
            if (!fieldAccess.CanStore) {
                throw new REPLException(this, $"Type '{targetFuture.Type.Name}' field '{fieldName}' is read-only");
            }
            
            if (!fieldAccess.FieldType.IsAssignableFrom(context.replModuleContext, valueFuture.Type)) {
                throw new REPLException(this,$"Type '{targetFuture.Type.Name}' field '{fieldName}' is of type {fieldAccess.FieldType} but is assigned to {valueFuture.Type}");
            }
            
            if (target is IAssignContext assignContext) {
                if (fieldAccess.RequiresPtr && assignContext.IsConst(context.replBlockContext)) {
                    throw new REPLException(this,
                        $"Type '{targetFuture.Type.Name}' field '{fieldName}' can not be set on a read-only variable");
                }
            } else {
                throw new REPLException(this,
                    $"Field assign '{targetFuture.Type.Name}'.'{fieldName}' on invalid target expression");
            }
            
            var assign = fieldAccess.FieldType.AssignFrom(context.replModuleContext, valueFuture.Type);
            
            if (op == Operator.Assign) {
                return REPLValueFuture.Chain2(fieldAccess.FieldType, targetFuture, valueFuture,
                    (target, value) => {
                        var converted = assign.EvalConvert(this, value);

                        return fieldAccess.EvalAssign(this, target, converted);
                    });
            }
            
            IOperatorEmitter operatorEmitter = fieldAccess.FieldType.AllowedSuffixOperators(context.replModuleContext)
                .GetMatching(context.replModuleContext, op, valueFuture.Type);

            if (operatorEmitter == null) {
                throw new REPLException(this, $"Type '{targetFuture.Type.Name}' field '{fieldName}': Cannot {op} a {fieldAccess.FieldType} with a {valueFuture.Type}");
            }
            
            return REPLValueFuture.Chain2(fieldAccess.FieldType, targetFuture, valueFuture,
                (target, value) => {
                    var prevValue = fieldAccess.EvalGet(this, target);
                    var converted = assign.EvalConvert(this, operatorEmitter.Eval(this, prevValue, value));

                    return fieldAccess.EvalAssign(this, target, converted);
                });
        }
    }
}
