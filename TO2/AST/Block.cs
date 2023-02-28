using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IBlockItem {
        Position Start { get; }

        Position End { get; }

        bool IsComment { get; }

        IVariableContainer VariableContainer { set; }

        TypeHint TypeHint { set; }

        TO2Type ResultType(IBlockContext context);

        void EmitCode(IBlockContext context, bool dropResult);
    }

    public interface IVariableRef {
        string Name { get; }

        TO2Type VariableType(IBlockContext context);
    }

    public class Block : Expression, IVariableContainer {
        private readonly List<IBlockItem> items;
        private readonly Dictionary<string, IVariableRef> variables;
        private IVariableContainer parentContainer;
        private ILocalRef preparedResult;

        public Block(List<IBlockItem> items, Position start = new Position(), Position end = new Position()) :
            base(start, end) {
            this.items = items;
            variables = new Dictionary<string, IVariableRef>();
            foreach (IBlockItem item in this.items) {
                item.VariableContainer = this;
                switch (item) {
                case VariableDeclaration variable:
                    if (!variables.ContainsKey(variable.declaration.target))
                        variables.Add(variable.declaration.target, variable);
                    break;
                case TupleDeconstructDeclaration tuple:
                    foreach (IVariableRef r in tuple.Refs) {
                        if (!variables.ContainsKey(r.Name))
                            variables.Add(r.Name, r);
                    }

                    break;
                }
            }
        }

        public override IVariableContainer VariableContainer {
            set => parentContainer = value;
        }

        public override TypeHint TypeHint {
            set {
                var last = items.LastOrDefault();
                if (last != null) last.TypeHint = value;
            }
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) =>
            variables.Get(name)?.VariableType(context);

        public override TO2Type ResultType(IBlockContext context) =>
            items.LastOrDefault(item => !item.IsComment)?.ResultType(context) ?? BuiltinType.Unit;

        public override void Prepare(IBlockContext context) {
            if (preparedResult != null || !context.IsAsync) return;

            EmitCode(context, false);
            preparedResult = context.DeclareHiddenLocal(ResultType(context).GeneratedType(context.ModuleContext));
            preparedResult.EmitStore(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (preparedResult != null) {
                if (!dropResult) preparedResult.EmitLoad(context);
                preparedResult = null;
                return;
            }

            bool childScope = parentContainer is Block;
            IBlockContext effectiveContext = context;

            if (childScope) {
                context.IL.BeginScope();
                effectiveContext = context.CreateChildContext();
            }

            List<IBlockItem> nonComments = items.Where(item => !item.IsComment).ToList();
            int len = nonComments.Count;
            if (len > 0) {
                for (int i = 0; i < len - 1; i++) {
                    IBlockItem item = nonComments[i];
                    item.EmitCode(effectiveContext, true);
                }

                nonComments[len - 1].EmitCode(effectiveContext, dropResult);
            } else if (!dropResult) {
                context.IL.Emit(OpCodes.Ldnull);
            }

            if (childScope) context.IL.EndScope();
        }
    }
}
