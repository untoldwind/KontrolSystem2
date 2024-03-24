using System;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Runtime;

public abstract class REPLValueFuture(TO2Type to2Type) : Future<IREPLValue?> {
    public TO2Type Type { get; } = to2Type;

    public static REPLValueFuture Success(IREPLValue? value) {
        return new SuccessImpl(value);
    }

    public static REPLValueFuture Wrap(TO2Type resultType, IAnyFuture future) {
        return new WrapImpl(resultType, future);
    }

    public REPLValueFuture Then(TO2Type resultType, Func<IREPLValue, IREPLValue> map) {
        return new ChainNImpl(resultType,
            [this], results => Success(map(results[0])));
    }

    public REPLValueFuture Then(TO2Type resultType, Func<IREPLValue, REPLValueFuture> map) {
        return new ChainNImpl(resultType,
            [this], results => map(results[0]));
    }

    public static REPLValueFuture Chain2(TO2Type resultType, REPLValueFuture first, REPLValueFuture second,
        Func<IREPLValue, IREPLValue, IREPLValue> map) {
        return new ChainNImpl(resultType,
            [first, second], results => Success(map(results[0], results[1])));
    }

    public static REPLValueFuture Chain2(TO2Type resultType, REPLValueFuture first, REPLValueFuture second,
        Func<IREPLValue, IREPLValue, REPLValueFuture> map) {
        return new ChainNImpl(resultType,
            [first, second], results => map(results[0], results[1]));
    }

    public static REPLValueFuture ChainN(TO2Type resultType, REPLValueFuture[] futures,
        Func<IREPLValue[], REPLValueFuture> map) {
        return new ChainNImpl(resultType, futures, map);
    }

    internal class SuccessImpl(IREPLValue? value) : REPLValueFuture(value?.Type ?? BuiltinType.Unit) {
        private readonly IREPLValue? value = value;

        public override FutureResult<IREPLValue?> PollValue() {
            return new FutureResult<IREPLValue?>(value);
        }
    }

    internal class WrapImpl(TO2Type resultType, IAnyFuture future) : REPLValueFuture(resultType) {
        private readonly IAnyFuture future = future;

        public override FutureResult<IREPLValue?> PollValue() {
            var result = future.Poll();

            if (result.IsReady)
                return new FutureResult<IREPLValue?>(Type.REPLCast(result.ValueObject));
            return new FutureResult<IREPLValue?>();
        }
    }

    internal class ChainNImpl : REPLValueFuture {
        private readonly REPLValueFuture[] futures;
        private readonly Func<IREPLValue[], REPLValueFuture> map;
        private readonly IREPLValue?[] results;
        private REPLValueFuture? mapFuture;
        private IREPLValue? mapResult;

        public ChainNImpl(TO2Type resultType, REPLValueFuture[] futures, Func<IREPLValue[], REPLValueFuture> map) :
            base(resultType) {
            this.futures = futures;
            results = new IREPLValue[this.futures.Length];
            this.map = map;
        }

        public override FutureResult<IREPLValue?> PollValue() {
            for (var i = 0; i < futures.Length; i++)
                if (results[i] == null) {
                    var result = futures[i].PollValue();
                    if (result.IsReady)
                        results[i] = result.value;
                    else
                        return new FutureResult<IREPLValue?>();
                }

            mapFuture ??= map.Invoke(results!);

            if (mapResult == null) {
                var result = mapFuture.PollValue();

                if (result.IsReady)
                    mapResult = result.value;
                else
                    return new FutureResult<IREPLValue?>();
            }

            return new FutureResult<IREPLValue?>(mapResult);
        }
    }
}

public interface IREPLValue {
    TO2Type Type { get; }

    object? Value { get; }

    bool IsBreak { get; }

    bool IsContinue { get; }

    bool IsReturn { get; }

    IREPLForInSource? ForInSource();
}

public interface IREPLForInSource {
    TO2Type ElementType { get; }

    IREPLValue? Next();
}

public class REPLUnit : IREPLValue {
    public static REPLUnit INSTANCE = new();

    private REPLUnit() {
    }

    public TO2Type Type => BuiltinType.Unit;

    public object? Value => null;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }
}

public class REPLBreak : IREPLValue {
    public static REPLBreak INSTANCE = new();

    private REPLBreak() {
    }

    public TO2Type Type => BuiltinType.Unit;

    public object? Value => null;

    public bool IsBreak => true;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }
}

public class REPLContinue : IREPLValue {
    public static REPLContinue INSTANCE = new();

    private REPLContinue() {
    }

    public TO2Type Type => BuiltinType.Unit;

    public object? Value => null;

    public bool IsBreak => false;

    public bool IsContinue => true;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }
}

public class REPLReturn(IREPLValue returnValue) : IREPLValue {
    public readonly IREPLValue returnValue = returnValue;

    public TO2Type Type => returnValue.Type;

    public object? Value => returnValue.Value;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => true;

    public IREPLForInSource? ForInSource() {
        return null;
    }
}

public readonly struct REPLBool(bool boolValue) : IREPLValue {
    public readonly bool boolValue = boolValue;

    public TO2Type Type => BuiltinType.Bool;

    public object Value => boolValue;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }

    public static IREPLValue Not(Node node, IREPLValue other, IREPLValue? _) {
        if (other is REPLBool b) return new REPLBool(!b.boolValue);

        throw new REPLException(node, $"Can not preform boolean not on non-boolean: {other.Type.Name}");
    }

    public static IREPLValue Eq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLBool lb && right is REPLBool rb) return new REPLBool(lb.boolValue == rb.boolValue);

        throw new REPLException(node, $"Can not preform boolean eq on non-boolean: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Neq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLBool lb && right is REPLBool rb) return new REPLBool(lb.boolValue != rb.boolValue);

        throw new REPLException(node,
            $"Can not preform boolean neq on non-boolean: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue And(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLBool lb && right is REPLBool rb) return new REPLBool(lb.boolValue && rb.boolValue);

        throw new REPLException(node,
            $"Can not preform boolean and on non-boolean: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Or(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLBool lb && right is REPLBool rb) return new REPLBool(lb.boolValue || rb.boolValue);

        throw new REPLException(node, $"Can not preform boolean or on non-boolean: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue ToInt(Node node, IREPLValue target) {
        if (target is REPLBool b) return new REPLInt(b.boolValue ? 1 : 0);

        throw new REPLException(node, $"Can not preform to_int on non-boolean: {target.Type.Name}");
    }

    public static IREPLValue ToFloat(Node node, IREPLValue target) {
        if (target is REPLBool b) return new REPLFloat(b.boolValue ? 1 : 0);

        throw new REPLException(node, $"Can not preform to_float on non-boolean: {target.Type.Name}");
    }
}

public readonly struct REPLInt(long intValue) : IREPLValue {
    public readonly long intValue = intValue;

    public TO2Type Type => BuiltinType.Int;

    public object Value => intValue;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }

    public static IREPLValue Neg(Node node, IREPLValue other, IREPLValue? _) {
        if (other is REPLInt i) return new REPLInt(-i.intValue);

        throw new REPLException(node, $"Can not preform int neg on non-int: {other.Type.Name}");
    }

    public static IREPLValue Add(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue + ri.intValue);

        throw new REPLException(node, $"Can not preform int add on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Sub(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue - ri.intValue);

        throw new REPLException(node, $"Can not preform int sub on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Mul(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue * ri.intValue);

        throw new REPLException(node, $"Can not preform int mul on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Div(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue / ri.intValue);

        throw new REPLException(node, $"Can not preform int div on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Rem(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue % ri.intValue);

        throw new REPLException(node, $"Can not preform int rem on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue BitOr(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue | ri.intValue);

        throw new REPLException(node, $"Can not preform int bit or on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue BitAnd(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue & ri.intValue);

        throw new REPLException(node, $"Can not preform int bit and on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue BitXor(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLInt(li.intValue ^ ri.intValue);

        throw new REPLException(node, $"Can not preform int bit xor on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Eq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLBool(li.intValue == ri.intValue);

        throw new REPLException(node, $"Can not preform int eq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Neq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLBool(li.intValue != ri.intValue);

        throw new REPLException(node, $"Can not preform int neq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Gt(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLBool(li.intValue > ri.intValue);

        throw new REPLException(node, $"Can not preform int gt on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Geq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLBool(li.intValue >= ri.intValue);

        throw new REPLException(node, $"Can not preform int geq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Lt(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLBool(li.intValue < ri.intValue);

        throw new REPLException(node, $"Can not preform int lt on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Leq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLInt li && right is REPLInt ri) return new REPLBool(li.intValue <= ri.intValue);

        throw new REPLException(node, $"Can not preform int leq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue ToBool(Node node, IREPLValue target) {
        if (target is REPLInt i) return new REPLBool(i.intValue != 0);

        throw new REPLException(node, $"Can not preform to_bool on non-integer: {target.Type.Name}");
    }

    public static IREPLValue ToFloat(Node node, IREPLValue target) {
        if (target is REPLInt i) return new REPLFloat(i.intValue);

        throw new REPLException(node, $"Can not preform to_float on non-integer: {target.Type.Name}");
    }
}

public readonly struct REPLFloat(double floatValue) : IREPLValue {
    public readonly double floatValue = floatValue;

    public TO2Type Type => BuiltinType.Float;

    public object Value => floatValue;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }

    public static IREPLValue Neg(Node node, IREPLValue other, IREPLValue? _) {
        if (other is REPLFloat f) return new REPLFloat(-f.floatValue);

        throw new REPLException(node, $"Can not preform float neg on non-float: {other.Type.Name}");
    }

    public static IREPLValue Add(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLFloat(lf.floatValue + rf.floatValue);

        throw new REPLException(node, $"Can not preform int add on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Sub(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLFloat(lf.floatValue - rf.floatValue);

        throw new REPLException(node, $"Can not preform int sub on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Mul(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLFloat(lf.floatValue * rf.floatValue);

        throw new REPLException(node, $"Can not preform int mul on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Div(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLFloat(lf.floatValue / rf.floatValue);

        throw new REPLException(node, $"Can not preform int div on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Rem(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLFloat(lf.floatValue % rf.floatValue);

        throw new REPLException(node, $"Can not preform int rem on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Eq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLBool(lf.floatValue == rf.floatValue);

        throw new REPLException(node, $"Can not preform int eq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Neq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLBool(lf.floatValue != rf.floatValue);

        throw new REPLException(node, $"Can not preform int neq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Gt(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLBool(lf.floatValue > rf.floatValue);

        throw new REPLException(node, $"Can not preform int gt on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Geq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLBool(lf.floatValue >= rf.floatValue);

        throw new REPLException(node, $"Can not preform int geq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Lt(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLBool(lf.floatValue < rf.floatValue);

        throw new REPLException(node, $"Can not preform int lt on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue Leq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLFloat lf && right is REPLFloat rf) return new REPLBool(lf.floatValue <= rf.floatValue);

        throw new REPLException(node, $"Can not preform int leq on non-int: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue ToInt(Node node, IREPLValue target) {
        if (target is REPLFloat f) return new REPLInt((long)f.floatValue);

        throw new REPLException(node, $"Can not preform to_int on non-float: {target.Type.Name}");
    }
}

public readonly struct REPLString(string stringValue) : IREPLValue {
    public readonly string stringValue = stringValue;

    public TO2Type Type => BuiltinType.String;

    public object Value => stringValue;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }
}

public readonly struct REPLArray(ArrayType arrayType, Array arrayValue) : IREPLValue {
    public readonly Array arrayValue = arrayValue;
    public readonly ArrayType arrayType = arrayType;

    public TO2Type Type => arrayType;

    public object Value => arrayValue;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource ForInSource() {
        return new REPLArrayForInSource(arrayValue, arrayType);
    }
}

public class REPLArrayForInSource(Array arrayValue, ArrayType arrayType) : IREPLForInSource {
    private int nextIdx = 0;

    public TO2Type ElementType => arrayType.ElementType;

    public IREPLValue? Next() {
        if (nextIdx >= arrayValue.Length) return null;

        var current = arrayValue.GetValue(nextIdx);
        nextIdx++;
        return arrayType.ElementType.REPLCast(current);
    }
}

public readonly struct REPLRange(Range rangeValue) : IREPLValue {
    public readonly Range rangeValue = rangeValue;

    public readonly TO2Type Type => BuiltinType.Range;

    public readonly object Value => rangeValue;

    public readonly bool IsBreak => false;

    public readonly bool IsContinue => false;

    public readonly bool IsReturn => false;

    public IREPLForInSource ForInSource() {
        return new REPLRangeForInSource(rangeValue.from, rangeValue.to);
    }
}

public class REPLRangeForInSource(long next, long to) : IREPLForInSource {
    public TO2Type ElementType => BuiltinType.Int;

    public IREPLValue? Next() {
        if (next >= to) return null;

        var current = next;
        next++;
        return new REPLInt(current);
    }
}

public readonly struct REPLAny(TO2Type type, object? anyValue) : IREPLValue {
    private readonly object? anyValue = anyValue;

    public TO2Type Type => type;

    public object? Value => anyValue;

    public bool IsBreak => false;

    public bool IsContinue => false;

    public bool IsReturn => false;

    public IREPLForInSource? ForInSource() {
        return null;
    }

    public static IREPLValue ObjEq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLAny li && right is REPLAny ri) return new REPLBool(li.anyValue == ri.anyValue);

        throw new REPLException(node, $"Can not preform int eq on: {left.Type.Name} {right?.Type.Name}");
    }

    public static IREPLValue ObjNeq(Node node, IREPLValue left, IREPLValue? right) {
        if (left is REPLAny li && right is REPLAny ri) return new REPLBool(li.anyValue != ri.anyValue);

        throw new REPLException(node, $"Can not preform int neq on: {left.Type.Name} {right?.Type.Name}");
    }
}
