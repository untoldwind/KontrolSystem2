using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IUnapplyEmitter {
        string Name { get; }

        List<TO2Type> Items { get; }

        void EmitExtract(IBlockContext context, List<IBlockVariable> targetVariables);
    }

    public class Unapply : Expression {
        private readonly string pattern;
        private readonly List<string> extractNames;
        private readonly Expression expression;

        public Unapply(string pattern, List<string> extractNames, Expression expression, Position start,
            Position end) : base(start, end) {
            this.pattern = pattern;
            this.extractNames = extractNames;
            this.expression = expression;
        }

        public override IVariableContainer VariableContainer {
            set => expression.VariableContainer = value;
        }

        public override TO2Type ResultType(IBlockContext context) => BuiltinType.Bool;

        public override Dictionary<string, TO2Type> GetScopeVariables(IBlockContext context) {
            TO2Type valueType = expression.ResultType(context);
            IUnapplyEmitter unapplyPattern =
                valueType.AllowedUnapplyPatterns(context.ModuleContext, pattern, extractNames.Count);

            if (unapplyPattern == null) return null;

            return extractNames.Zip(unapplyPattern.Items, (name, type) => (name, type)).Where(item => item.name != "_")
                .ToDictionary(item => item.name, item => item.type);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type valueType = expression.ResultType(context);
            IUnapplyEmitter unapplyPattern =
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

                List<IBlockVariable> extractVariables =
                    extractNames.Select(extractName => context.FindVariable(extractName)).ToList();

                unapplyPattern.EmitExtract(context, extractVariables);
            }
        }

        public override void Prepare(IBlockContext context) => expression.Prepare(context);
    }
}
