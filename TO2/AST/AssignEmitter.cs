using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IAssignEmitter {
        void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult);

        void EmitConvert(IBlockContext context);
    }

    public class DefaultAssignEmitter : IAssignEmitter {
        public static readonly IAssignEmitter Instance = new DefaultAssignEmitter();

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
            bool dropResult) => expression.EmitStore(context, variable, dropResult);

        public void EmitConvert(IBlockContext context) {
        } // Nothing to convert
    }
}
