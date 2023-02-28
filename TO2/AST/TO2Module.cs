using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IModuleItem {
        Position Start { get; }

        Position End { get; }

        IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context);

        IEnumerable<StructuralError> TryImportTypes(ModuleContext context);

        IEnumerable<StructuralError> TryImportConstants(ModuleContext context);

        IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context);

        IEnumerable<StructuralError> TryImportFunctions(ModuleContext context);
    }

    public class TO2Module {
        public readonly string name;
        public readonly string description;
        private readonly List<IModuleItem> items;
        public readonly List<FunctionDeclaration> functions;
        public readonly List<ConstDeclaration> constants;
        public readonly List<StructDeclaration> structs;

        public TO2Module(string name, string description, List<IModuleItem> items) {
            this.name = name;
            this.description = description;
            this.items = items;
            functions = this.items.Where(item => item is FunctionDeclaration).Cast<FunctionDeclaration>().ToList();
            constants = this.items.Where(item => item is ConstDeclaration).Cast<ConstDeclaration>().ToList();
            structs = this.items.Where(item => item is StructDeclaration).Cast<StructDeclaration>().ToList();
        }

        public List<StructuralError> TryDeclareTypes(ModuleContext context) =>
            items.SelectMany(item => item.TryDeclareTypes(context)).ToList();

        public List<StructuralError> TryImportTypes(ModuleContext context) =>
            items.SelectMany(item => item.TryImportTypes(context)).ToList();

        public List<StructuralError> TryImportConstants(ModuleContext context) =>
            items.SelectMany(item => item.TryImportConstants(context)).ToList();

        public List<StructuralError> TryVerifyFunctions(ModuleContext context) =>
            items.SelectMany(item => item.TryVerifyFunctions(context)).ToList();

        public List<StructuralError> TryImportFunctions(ModuleContext context) =>
            items.SelectMany(item => item.TryImportFunctions(context)).ToList();

        public static string BuildName(string fileName) {
            fileName = fileName.ToLower();
            if (fileName.EndsWith(".to2")) fileName = fileName.Substring(0, fileName.Length - 4);
            return Regex.Replace(Regex.Replace(fileName, "[^A-Za-z0-9_\\\\/]", "_"), "[\\\\/]", "::");
        }
    }
}
