using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class ImplDeclaration : Node, IModuleItem {
        private readonly string name;
        private readonly List<MethodDeclaration> methods;

        public ImplDeclaration(string name, List<MethodDeclaration> methods, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.name = name;
            this.methods = methods;
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) {
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
            StructTypeAliasDelegate structDelegate = context.mappedTypes.Get(name) as StructTypeAliasDelegate;

            if (structDelegate == null) {
                return new StructuralError(
                    StructuralError.ErrorType.InvalidType,
                    $"Struct with name {name} is not defined",
                    Start,
                    End).Yield();
            }

            foreach (var method in methods) {
                method.StructType = structDelegate;
                structDelegate.AddMethod(method.name, method.CreateInvokeFactory());
            }

            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
            List<StructuralError> errors = new List<StructuralError>();

            foreach (var method in methods) {
                errors.AddRange(method.EmitCode());
            }

            return errors;
        }

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();
    }
}
