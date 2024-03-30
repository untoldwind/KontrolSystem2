using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public class ImplDeclaration(
    string name,
    List<IEither<LineComment, MethodDeclaration>> methods,
    Position start = new(),
    Position end = new())
    : Node(start, end), IModuleItem {
    public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) => [];

    public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) {
        var structDelegate = context.mappedTypes.Get(name) as StructTypeAliasDelegate;

        if (structDelegate == null)
            return new StructuralError(
                StructuralError.ErrorType.InvalidType,
                $"Struct with name {name} is not defined",
                Start,
                End).Yield();

        foreach (var method in methods)
            if (method.IsRight) {
                method.Right.StructType = structDelegate;
                structDelegate.AddMethod(method.Right.name, method.Right.CreateInvokeFactory());
            }

        return [];
    }

    public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
        var errors = new List<StructuralError>();

        foreach (var method in methods)
            if (method.IsRight)
                errors.AddRange(method.Right.EmitCode());

        return errors;
    }

    public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => [];

    public override REPLValueFuture Eval(REPLContext context) {
        throw new NotSupportedException("Structs are not supported in REPL mode");
    }
}
