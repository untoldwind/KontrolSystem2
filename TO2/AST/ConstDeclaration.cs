using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class ConstDeclaration : Node, IModuleItem {
        public readonly bool isPublic;
        public readonly string name;
        public readonly string description;
        public readonly TO2Type type;
        public readonly Expression expression;

        public ConstDeclaration(bool isPublic, string name, string description, TO2Type type, Expression expression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.isPublic = isPublic;
            this.name = name;
            this.description = description;
            this.type = type;
            this.expression = expression;
            this.expression.TypeHint = context => this.type.UnderlyingType(context.ModuleContext);
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();
    }
}
