﻿using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class BinaryBool : Expression {
        private readonly Operator op;
        private readonly Expression left;
        private readonly Expression right;

        public BinaryBool(Expression left, Operator op, Expression right, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.left = left;
            this.op = op;
            this.right = right;
            this.left.TypeHint = _ => BuiltinType.Bool;
            this.right.TypeHint = _ => BuiltinType.Bool;
        }

        public override IVariableContainer VariableContainer {
            set {
                left.VariableContainer = value;
                right.VariableContainer = value;
            }
        }

        public override TO2Type ResultType(IBlockContext context) => BuiltinType.Bool;

        public override void Prepare(IBlockContext context) {
            left.Prepare(context);
            right.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type leftType = left.ResultType(context);
            TO2Type rightType = right.ResultType(context);

            if (leftType != BuiltinType.Bool)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    "Expected boolean",
                    left.Start,
                    left.End
                ));
            if (rightType != BuiltinType.Bool)
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    "Expected boolean",
                    right.Start,
                    right.End
                ));

            if (context.HasErrors) return;

            left.EmitCode(context, false);
            if (!dropResult) context.IL.Emit(OpCodes.Dup);

            ILCount rightCount = right.GetILCount(context, dropResult);
            LabelRef skipRight = context.IL.DefineLabel(rightCount.opCodes < 124);

            if (context.HasErrors) return;

            switch (op) {
            case Operator.BoolAnd:
                context.IL.Emit(skipRight.isShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, skipRight);
                break;
            case Operator.BoolOr:
                context.IL.Emit(skipRight.isShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, skipRight);
                break;
            default:
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidOperator,
                    $"Invalid boolean operator {op}",
                    Start,
                    End
                ));
                return;
            }

            if (!dropResult) context.IL.Emit(OpCodes.Pop);

            right.EmitCode(context, dropResult);
            context.IL.MarkLabel(skipRight);
        }

        public override REPLValueFuture Eval(REPLContext context) {
            TO2Type leftType = left.ResultType(context.replBlockContext);
            TO2Type rightType = right.ResultType(context.replBlockContext);

            if (leftType != BuiltinType.Bool)
                throw new REPLException(left, "Expected boolean");
            if (rightType != BuiltinType.Bool)
                throw new REPLException(right, "Expected boolean");

            switch (op) {
            case Operator.BoolAnd:
                return left.Eval(context).Then(BuiltinType.Bool, leftValue => {
                    if (leftValue is REPLBool b) {
                        if (b.boolValue)
                            return right.Eval(context);
                        return REPLValueFuture.Success(new REPLBool(false));
                    }
                    throw new REPLException(left, "Expected boolean");
                });
            case Operator.BoolOr:
                return left.Eval(context).Then(BuiltinType.Bool, leftValue => {
                    if (leftValue is REPLBool b) {
                        if (!b.boolValue)
                            return REPLValueFuture.Success(new REPLBool(false));
                        return right.Eval(context);
                    }
                    throw new REPLException(left, "Expected boolean");
                });
            default:
                throw new REPLException(this, $"Invalid boolean operator {op}");
            }
        }
    }
}
