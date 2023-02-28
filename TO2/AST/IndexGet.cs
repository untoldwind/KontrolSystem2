using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    public class IndexGet : Expression, IAssignContext {
        private readonly Expression target;
        private readonly IndexSpec indexSpec;

        public IndexGet(Expression target, IndexSpec indexSpec, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.target = target;
            this.indexSpec = indexSpec;
        }

        public override IVariableContainer VariableContainer {
            set {
                target.VariableContainer = value;
                indexSpec.VariableContainer = value;
            }
        }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type targetType = target.ResultType(context);
            return targetType.AllowedIndexAccess(context.ModuleContext, indexSpec)?.TargetType ?? BuiltinType.Unit;
        }

        public bool IsConst(IBlockContext context) => (target as IAssignContext)?.IsConst(context) ?? true;

        public override void Prepare(IBlockContext context) {
            target.Prepare(context);
            indexSpec.start.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
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

            if (!dropResult) {
                if (indexAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                indexAccess.EmitLoad(context);
            }
        }

        public override void EmitPtr(IBlockContext context) {
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

            if (indexAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (context.HasErrors) return;

            indexAccess.EmitPtr(context);
        }
    }
}
