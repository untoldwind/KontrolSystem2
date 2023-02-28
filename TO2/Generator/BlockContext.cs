using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Generator {
    public interface IBlockContext {
        ModuleContext ModuleContext { get; }

        MethodBuilder MethodBuilder { get; }

        IILEmitter IL { get; }

        TO2Type ExpectedReturn { get; }

        bool IsAsync { get; }

        void AddError(StructuralError error);

        bool HasErrors { get; }

        List<StructuralError> AllErrors { get; }

        IBlockContext CreateChildContext();

        IBlockContext CreateLoopContext(LabelRef start, LabelRef end);

        IBlockContext CloneCountingContext();

        (LabelRef start, LabelRef end)? InnerLoop { get; }

        IBlockVariable FindVariable(string name);

        ITempBlockVariable MakeTempVariable(RealizedType to2Type);

        ILocalRef DeclareHiddenLocal(Type rawType);

        IBlockVariable DeclaredVariable(string name, bool isConst, RealizedType to2Type);

        void RegisterAsyncResume(TO2Type returnType);
    }
}
