using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class LineComment : IBlockItem, IModuleItem {
    public LineComment(string comment, Position start, Position end) {
        Comment = comment;
        Start = start;
        End = end;
    }

    public string Comment { get; }
    public Position Start { get; }
    public Position End { get; }

    public bool IsComment => true;

    public IVariableContainer VariableContainer {
        set { }
    }

    public TypeHint? TypeHint {
        set { }
    }

    public TO2Type ResultType(IBlockContext context) {
        return BuiltinType.Unit;
    }

    public void Prepare(IBlockContext context) {
    }

    public void EmitCode(IBlockContext context, bool dropResult) {
    }

    public REPLValueFuture Eval(REPLContext context) {
        return REPLValueFuture.Success(REPLUnit.INSTANCE);
    }

    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) {
        return Enumerable.Empty<StructuralError>();
    }

    public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
        return Enumerable.Empty<StructuralError>();
    }

    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
        return Enumerable.Empty<StructuralError>();
    }

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
        return Enumerable.Empty<StructuralError>();
    }

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) {
        return Enumerable.Empty<StructuralError>();
    }
}
