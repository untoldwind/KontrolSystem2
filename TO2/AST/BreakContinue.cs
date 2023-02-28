using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class Break : Expression {
        public Break(Position start = new Position(), Position end = new Position()) : base(start, end) {
        }

        public override IVariableContainer VariableContainer {
            set { }
        }

        public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (!context.InnerLoop.HasValue) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidScope,
                    "break can only be used inside a loop",
                    Start,
                    End
                ));
                return;
            }

            context.IL.Emit(context.InnerLoop.Value.end.isShort ? OpCodes.Br_S : OpCodes.Br,
                context.InnerLoop.Value.end);
        }
    }

    public class Continue : Expression {
        public Continue(Position start = new Position(), Position end = new Position()) : base(start, end) {
        }

        public override IVariableContainer VariableContainer {
            set { }
        }

        public override TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (!context.InnerLoop.HasValue) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.InvalidScope,
                    "continue can only be used inside a loop",
                    Start,
                    End
                ));
                return;
            }

            context.IL.Emit(context.InnerLoop.Value.start.isShort ? OpCodes.Br_S : OpCodes.Br,
                context.InnerLoop.Value.start);
        }
    }
}
