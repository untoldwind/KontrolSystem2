using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public interface IBlockItem {
    Position Start { get; }

    Position End { get; }

    bool IsComment { get; }

    IVariableContainer VariableContainer { set; }

    TypeHint? TypeHint { set; }

    TO2Type ResultType(IBlockContext context);

    void Prepare(IBlockContext context);

    void EmitCode(IBlockContext context, bool dropResult);

    REPLValueFuture Eval(REPLContext context);
}

public interface IVariableRef {
    string Name { get; }

    TO2Type? VariableType(IBlockContext context);
}

public class Block : Expression, IVariableContainer {
    private readonly List<IBlockItem> items;
    private readonly Dictionary<string, IVariableRef> variables;
    private IVariableContainer? parentContainer;
    private ILocalRef? preparedResult;

    public Block(List<IBlockItem> items, Position start = new(), Position end = new()) :
        base(start, end) {
        this.items = items;
        variables = [];
        foreach (var item in this.items) {
            item.VariableContainer = this;
            switch (item) {
            case VariableDeclaration variable:
                if (!variables.ContainsKey(variable.declaration.target!))
                    variables.Add(variable.declaration.target!, variable);
                break;
            case TupleDeconstructDeclaration tuple:
                foreach (var r in tuple.Refs)
                    if (!variables.ContainsKey(r.Name))
                        variables.Add(r.Name, r);

                break;
            }
        }
    }

    public override IVariableContainer? VariableContainer {
        set => parentContainer = value;
    }

    public override TypeHint? TypeHint {
        set {
            var last = items.LastOrDefault();
            if (last != null) last.TypeHint = value;
        }
    }

    public IVariableContainer? ParentContainer => parentContainer;

    public TO2Type? FindVariableLocal(IBlockContext context, string name) => variables.Get(name)?.VariableType(context);

    public override TO2Type ResultType(IBlockContext context) => items.LastOrDefault(item => !item.IsComment)?.ResultType(context) ?? BuiltinType.Unit;

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

        var nonComments = items.Where(item => !item.IsComment).ToList();
        var len = nonComments.Count;
        if (len > 0) {
            for (var i = 0; i < len - 1; i++) {
                var item = nonComments[i];
                try {
                    if (effectiveContext.IsAsync) item.Prepare(effectiveContext);

                    item.EmitCode(effectiveContext, true);
                } catch (CodeGenerationException e) {
                    context.AddError(new StructuralError(StructuralError.ErrorType.CoreGeneration, e.Message,
                        item.Start, item.End));
                }
            }

            nonComments[len - 1].EmitCode(effectiveContext, dropResult);
        } else if (!dropResult) {
            context.IL.Emit(OpCodes.Ldnull);
        }

        if (childScope) context.IL.EndScope();
    }

    internal override Expression CollapseFinalReturn() {
        var (lastItem, lastIdx) = items.Select((item, idx) => (item, idx)).LastOrDefault(item => !item.item.IsComment);

        if (lastItem != null && lastItem is Expression e) {
            items[lastIdx] = e.CollapseFinalReturn();
        }

        return this;
    }

    public override REPLValueFuture Eval(REPLContext context) {
        bool childScope = parentContainer == null || parentContainer is Block;
        REPLContext effectiveContext = context;

        if (childScope) effectiveContext = context.CreateChildContext();

        return new REPLBlockEval(ResultType(context.replBlockContext), effectiveContext, items);
    }

    internal class REPLBlockEval(TO2Type to2Type, REPLContext context, List<IBlockItem> items) : REPLValueFuture(to2Type) {
        private readonly IEnumerator<IBlockItem> items = items.Where(item => !item.IsComment).GetEnumerator();
        private REPLValueFuture? lastFuture;
        private IREPLValue? lastResult = REPLUnit.INSTANCE;

        public override FutureResult<IREPLValue?> PollValue() {
            if (lastFuture == null) {
                if (!items.MoveNext()) return new FutureResult<IREPLValue?>(lastResult);
                lastFuture = items.Current!.Eval(context);
            }

            var result = lastFuture.PollValue();
            if (!result.IsReady) return new FutureResult<IREPLValue?>();
            if (result.value!.IsBreak || result.value.IsContinue || result.value.IsReturn)
                return new FutureResult<IREPLValue?>(result.value);
            lastFuture = null;
            lastResult = result.value;

            return new FutureResult<IREPLValue?>();
        }
    }
}
