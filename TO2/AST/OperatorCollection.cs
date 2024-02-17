using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST;

public interface IOperatorCollection : IEnumerable<(Operator op, List<IOperatorEmitter> emitters)> {
    IOperatorEmitter? GetMatching(ModuleContext context, Operator op, TO2Type otherType);
}

public class OperatorCollection : IOperatorCollection {
    private readonly Dictionary<Operator, List<IOperatorEmitter>> collection;

    public OperatorCollection() {
        collection = new Dictionary<Operator, List<IOperatorEmitter>>();
    }

    private OperatorCollection(IEnumerable<(Operator op, IOperatorEmitter emitter)> collection) {
        this.collection =
            collection.GroupBy(o => o.op, o => o.emitter).ToDictionary(g => g.Key, g => g.ToList());
    }

    public IOperatorEmitter? GetMatching(ModuleContext context, Operator op, TO2Type otherType) {
        if (!collection.ContainsKey(op)) return null;
        return collection[op].Find(o => o.Accepts(context, otherType));
    }

    public IEnumerator<(Operator op, List<IOperatorEmitter> emitters)> GetEnumerator() {
        return collection.Select(o => (o.Key, o.Value)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void Add(Operator op, IOperatorEmitter operatorEmitter) {
        if (collection.TryGetValue(op, out var value))
            value.Add(operatorEmitter);
        else
            collection.Add(op, new List<IOperatorEmitter> { operatorEmitter });
    }

    public OperatorCollection FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
        return new OperatorCollection(collection.SelectMany(g =>
            g.Value.Select(o => (g.Key, o.FillGenerics(context, typeArguments)))));
    }
}
