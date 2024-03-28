using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class LineComment(string comment, Position start, Position end) : IBlockItem, IModuleItem {
    public string Comment { get; } = comment;
    public Position Start { get; } = start;
    public Position End { get; } = end;

    public bool IsComment => true;

    public IVariableContainer VariableContainer {
        set { }
    }

    public TypeHint? TypeHint {
        set { }
    }

    public TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

    public void Prepare(IBlockContext context) {
    }

    public void EmitCode(IBlockContext context, bool dropResult) {
    }

    public REPLValueFuture Eval(REPLContext context) => REPLValueFuture.Success(REPLUnit.INSTANCE);

    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => [];
}
