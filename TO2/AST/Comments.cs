using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class LineComment : IBlockItem, IModuleItem {
        public Position Start { get; }
        public Position End { get; }
        public string Comment { get; }

        public LineComment(string comment, Position start, Position end) {
            Comment = comment;
            Start = start;
            End = end;
        }

        public bool IsComment => true;

        public IVariableContainer VariableContainer {
            set { }
        }

        public TypeHint TypeHint {
            set { }
        }

        public TO2Type ResultType(IBlockContext context) => BuiltinType.Unit;

        public void EmitCode(IBlockContext context, bool dropResult) {
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();
    }
}
