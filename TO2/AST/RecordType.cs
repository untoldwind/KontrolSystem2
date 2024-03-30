using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST;

public abstract class RecordType : RealizedType {
    private readonly IOperatorCollection recordTypeOperators;

    protected RecordType(IOperatorCollection allowedSuffixOperators) {
        recordTypeOperators = new RecordTypeOperators(this, allowedSuffixOperators);
    }

    public abstract SortedDictionary<string, TO2Type> ItemTypes { get; }

    public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
        var recordType = otherType.UnderlyingType(context) as RecordType;
        if (recordType == null) return false;
        foreach (var kv in ItemTypes) {
            var otherItem = recordType.ItemTypes.Get(kv.Key);

            if (otherItem == null || !kv.Value.IsAssignableFrom(context, otherItem)) return false;
        }

        return true;
    }

    public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => recordTypeOperators;

    internal abstract IOperatorEmitter CombineFrom(RecordType otherType);
}

internal class RecordTypeOperators : IOperatorCollection {
    private readonly IOperatorCollection allowedOperators;
    private readonly RecordType recordType;

    internal RecordTypeOperators(RecordType recordType, IOperatorCollection allowedOperators) {
        this.recordType = recordType;
        this.allowedOperators = allowedOperators;
    }

    public IOperatorEmitter? GetMatching(ModuleContext context, Operator op, TO2Type otherType) {
        var existing = allowedOperators.GetMatching(context, op, otherType);
        if (existing != null) return existing;

        if (op != Operator.BitAnd && op != Operator.BitAndAssign) return null;

        var otherRecordType = otherType.UnderlyingType(context) as RecordType;

        if (otherRecordType == null) return null;

        var hasMatch = false;
        foreach (var otherKV in otherRecordType.ItemTypes) {
            var item = recordType.ItemTypes.Get(otherKV.Key);

            if (item == null) continue;
            if (!item.IsAssignableFrom(context, otherKV.Value)) return null;
            hasMatch = true;
        }

        return hasMatch ? recordType.CombineFrom(otherRecordType) : null;
    }

    public IEnumerator<(Operator op, List<IOperatorEmitter> emitters)> GetEnumerator() => allowedOperators.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal abstract class RecordTypeAssignEmitter<T> : IAssignEmitter, IOperatorEmitter where T : RecordType {
    protected readonly RecordType sourceType;
    protected readonly T targetType;

    protected RecordTypeAssignEmitter(T targetType, RecordType sourceType) {
        this.targetType = targetType;
        this.sourceType = sourceType;
    }

    // ---------------- IAssignEmitter -----------------
    public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
        using var valueTemp = context.MakeTempVariable(sourceType);
        expression.EmitStore(context, valueTemp, true);

        variable.EmitLoadPtr(context);
        EmitAssignToPtr(context, valueTemp);
        if (!dropResult) variable.EmitLoad(context);
    }

    public void EmitConvert(IBlockContext context, bool mutableTarget) {
        using var valueTemp = context.MakeTempVariable(sourceType);
        valueTemp.EmitStore(context);

        var generatedType = targetType.GeneratedType(context.ModuleContext);
        using var someResult = context.IL.TempLocal(generatedType);
        someResult.EmitLoadPtr(context);
        EmitAssignToPtr(context, valueTemp);
        someResult.EmitLoad(context);
    }

    public IREPLValue EvalConvert(Node node, IREPLValue value) {
        throw new REPLException(node, "Not supported in REPL mode");
    }

    public TO2Type ResultType => targetType;

    public TO2Type OtherType => sourceType;

    public bool Accepts(ModuleContext context, TO2Type otherType) => sourceType.IsAssignableFrom(context, otherType);

    // ---------------- IOperatorEmitter ----------------
    public void EmitCode(IBlockContext context, Node target) {
        using var tempRight = context.MakeTempVariable(sourceType);
        tempRight.EmitStore(context);

        var generatedType = targetType.GeneratedType(context.ModuleContext);
        using var someResult = context.IL.TempLocal(generatedType);
        someResult.EmitStore(context);

        someResult.EmitLoadPtr(context);
        EmitAssignToPtr(context, tempRight);
        someResult.EmitLoad(context);
    }

    public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
        using var tempRight = context.MakeTempVariable(sourceType);
        tempRight.EmitStore(context);
        context.IL.Emit(OpCodes.Pop); // Left side is just the variable we are about to override

        variable.EmitLoadPtr(context);
        EmitAssignToPtr(context, tempRight);
    }

    public IOperatorEmitter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;

    public IREPLValue Eval(Node node, IREPLValue left, IREPLValue? right) {
        throw new REPLException(node, "Not supported in REPL mode");
    }

    protected abstract void EmitAssignToPtr(IBlockContext context, IBlockVariable tempSource);
}
