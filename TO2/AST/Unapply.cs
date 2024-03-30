using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public interface IUnapplyEmitter {
    string Name { get; }

    List<TO2Type> Items { get; }

    void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables);
}

public class Unapply(
    string pattern,
    List<string> extractNames,
    Expression expression,
    Position start,
    Position end)
    : Expression(start, end) {
    public override IVariableContainer? VariableContainer {
        set => expression.VariableContainer = value;
    }

    public override TO2Type ResultType(IBlockContext context) => BuiltinType.Bool;

    public override Dictionary<string, TO2Type>? GetScopeVariables(IBlockContext context) {
        var valueType = expression.ResultType(context);
        var unapplyPattern =
            valueType.AllowedUnapplyPatterns(context.ModuleContext, pattern, extractNames.Count);

        if (unapplyPattern == null) return null;

        return extractNames.Zip(unapplyPattern.Items, (name, type) => (name, type)).Where(item => item.name != "_")
            .ToDictionary(item => item.name, item => item.type);
    }

    public override void EmitCode(IBlockContext context, bool dropResult) {
        var valueType = expression.ResultType(context);
        var unapplyPattern =
            valueType.AllowedUnapplyPatterns(context.ModuleContext, pattern, extractNames.Count);

        if (unapplyPattern == null) {
            context.AddError(new StructuralError(
                StructuralError.ErrorType.NoSuchUnapply,
                $"Type '{valueType.Name}' does not support an unapply to {pattern}({string.Join(", ", Enumerable.Repeat("_", extractNames.Count))})",
                Start,
                End
            ));
            return;
        }

        if (!dropResult) {
            expression.EmitCode(context, false);

            if (context.HasErrors) return;

            var extractVariables =
                extractNames.Select(extractName => context.FindVariable(extractName)!).ToList();

            unapplyPattern.EmitExtract(context, extractVariables);
        }
    }

    public override void Prepare(IBlockContext context) {
        expression.Prepare(context);
    }

    public override REPLValueFuture Eval(REPLContext context) {
        throw new REPLException(this, "Not supported in REPL mode");
    }
}
