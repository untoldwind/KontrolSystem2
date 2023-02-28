using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class FieldGet : Expression, IAssignContext {
        private readonly Expression target;
        private readonly string fieldName;

        public FieldGet(Expression target, string fieldName, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.target = target;
            this.fieldName = fieldName;
        }

        public override IVariableContainer VariableContainer {
            set => target.VariableContainer = value;
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

            return fieldAccess.FieldType;
        }

        public bool IsConst(IBlockContext context) => (target as IAssignContext)?.IsConst(context) ?? true;

        public override void Prepare(IBlockContext context) {
            target.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
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
                return;
            }

            if (!dropResult) {
                if (fieldAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                fieldAccess.EmitLoad(context);
            }
        }

        public override void EmitPtr(IBlockContext context) {
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
                return;
            }

            if (fieldAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);

            if (context.HasErrors) return;

            fieldAccess.EmitPtr(context);
        }
    }
}
