using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IForInSource {
        RealizedType ElementType { get; }

        void EmitInitialize(IBlockContext context);

        void EmitCheckDone(IBlockContext context, LabelRef loop);

        void EmitNext(IBlockContext context);
    }
}
