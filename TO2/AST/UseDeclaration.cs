using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class UseDeclaration : Node, IModuleItem {
        private readonly string fromModule;
        private readonly List<string> names;
        private readonly string alias;

        public UseDeclaration(List<string> names, List<string> moduleNamePath, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            fromModule = String.Join("::", moduleNamePath);
            this.names = names;
        }

        public UseDeclaration(List<string> moduleNamePath, string alias, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            fromModule = String.Join("::", moduleNamePath);
            this.alias = alias;
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
            IKontrolModule module = context.FindModule(fromModule);

            if (module == null)
                return new StructuralError(
                    StructuralError.ErrorType.NoSuchModule,
                    $"Module '{fromModule}' not found",
                    Start,
                    End
                ).Yield();
            if (alias != null) {
                context.moduleAliases.Add(alias, fromModule);
            } else {
                foreach (string name in (names ?? module.AllTypeNames)) {
                    TO2Type type = module.FindType(name);

                    if (type != null) context.mappedTypes.Add(name, type);
                }
            }

            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
            if (alias != null) return Enumerable.Empty<StructuralError>();
            IKontrolModule module = context.FindModule(fromModule);

            if (module == null)
                return new StructuralError(
                    StructuralError.ErrorType.NoSuchModule,
                    $"Module '{fromModule}' not found",
                    Start,
                    End
                ).Yield();
            foreach (string name in names ?? module.AllConstantNames) {
                IKontrolConstant constant = module.FindConstant(name);

                if (constant != null) context.mappedConstants.Add(name, constant);
            }

            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) {
            if (alias != null) return Enumerable.Empty<StructuralError>();
            IKontrolModule module = context.FindModule(fromModule);

            if (module == null)
                return new StructuralError(
                    StructuralError.ErrorType.NoSuchModule,
                    $"Module '{fromModule}' not found",
                    Start,
                    End
                ).Yield();

            List<StructuralError> errors = new List<StructuralError>();

            foreach (string name in names ?? module.AllFunctionNames) {
                if (context.mappedConstants.ContainsKey(name)) continue;

                IKontrolFunction function = module.FindFunction(name);

                if (function != null) {
                    context.mappedFunctions.Add(name, function);
                } else if (!context.mappedTypes.ContainsKey(name)) {
                    errors.Add(new StructuralError(
                        StructuralError.ErrorType.InvalidImport,
                        $"Module '{fromModule}' does not have public member '{name}''",
                        Start,
                        End
                    ));
                }
            }

            return errors;
        }
    }
}
