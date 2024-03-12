using System;

namespace KontrolSystem.Parsing;

public static partial class Parsers {
    /// <summary>
    ///     Parse first, and if successful, then parse second.
    /// </summary>
    public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second) {
        return input =>
            first(input).Select(s => second(s.value)(s.remaining));
    }

    /// <summary>
    ///     Parse first, and if successful, then parse second.
    /// </summary>
    public static Parser<U> Then<T, U>(this Parser<T> first, Parser<U> second) {
        return input =>
            first(input).Select(s => second(s.remaining));
    }

    /// <summary>
    ///     Monadic combinator Then, adapted for Linq comprehension syntax.
    /// </summary>
    public static Parser<V> SelectMany<T, U, V>(
        this Parser<T> parser,
        Func<T, Parser<U>> selector,
        Func<T, U, V> projector) {
        return parser.Then(t => selector(t).Map(u => projector(t, u)));
    }

    public static Parser<T> Preceded<T, P>(Parser<P> prefix, Parser<T> parser) {
        return input =>
            prefix(input).Select(p => parser(p.remaining));
    }

    public static Parser<IOption<T>> IfPreceded<T, P>(Parser<P> conditionPrefix, Parser<T> parser) {
        return input => {
            var resultPrefix = conditionPrefix(input);
            if (!resultPrefix.success) return Result.Success(input, Option.None<T>());
            var parserResult = parser(resultPrefix.remaining);
            return parser(resultPrefix.remaining).Map(p => Option.Some(p));
        };
    }

    public static Parser<T> Terminated<T, S>(Parser<T> parser, Parser<S> suffix) {
        return input =>
            parser(input).Select(t => suffix(t.remaining).To(t.value));
    }

    /// <summary>
    ///     Parse an item between a given prefix and suffix.
    /// </summary>
    public static Parser<T> Between<T, P, S>(this Parser<T> parser, Parser<P> prefix, Parser<S> suffix) {
        return input =>
            prefix(input).Select(p => parser(p.remaining).Select(t => suffix(t.remaining).To(t.value)));
    }

    public static Parser<(T1, T2)> Seq<T1, T2>(Parser<T1> parser1, Parser<T2> parser2) {
        return input => {
            var result1 = parser1(input);
            if (!result1.success) return Result.Failure<(T1, T2)>(result1.remaining, result1.expected);
            var result2 = parser2(result1.remaining);
            if (!result2.success) return Result.Failure<(T1, T2)>(result2.remaining, result2.expected);
            return Result.Success(result2.remaining, (result1.value, result2.value));
        };
    }

    public static Parser<(T1, T2, T3)>
        Seq<T1, T2, T3>(Parser<T1> parser1, Parser<T2> parser2, Parser<T3> parser3) {
        return input => {
            var result1 = parser1(input);
            if (!result1.success) return Result.Failure<(T1, T2, T3)>(result1.remaining, result1.expected);
            var result2 = parser2(result1.remaining);
            if (!result2.success) return Result.Failure<(T1, T2, T3)>(result2.remaining, result2.expected);
            var result3 = parser3(result2.remaining);
            if (!result3.success) return Result.Failure<(T1, T2, T3)>(result3.remaining, result3.expected);
            return Result.Success(result3.remaining, (result1.value, result2.value, result3.value));
        };
    }

    public static Parser<(T1, T2, T3, T4)> Seq<T1, T2, T3, T4>(Parser<T1> parser1, Parser<T2> parser2,
        Parser<T3> parser3, Parser<T4> parser4) {
        return input => {
            var result1 = parser1(input);
            if (!result1.success) return Result.Failure<(T1, T2, T3, T4)>(result1.remaining, result1.expected);
            var result2 = parser2(result1.remaining);
            if (!result2.success) return Result.Failure<(T1, T2, T3, T4)>(result2.remaining, result2.expected);
            var result3 = parser3(result2.remaining);
            if (!result3.success) return Result.Failure<(T1, T2, T3, T4)>(result3.remaining, result3.expected);
            var result4 = parser4(result3.remaining);
            if (!result4.success) return Result.Failure<(T1, T2, T3, T4)>(result4.remaining, result4.expected);
            return Result.Success(result4.remaining, (result1.value, result2.value, result3.value, result4.value));
        };
    }

    public static Parser<(T1, T2, T3, T4, T5)> Seq<T1, T2, T3, T4, T5>(Parser<T1> parser1, Parser<T2> parser2,
        Parser<T3> parser3, Parser<T4> parser4, Parser<T5> parser5) {
        return input => {
            var result1 = parser1(input);
            if (!result1.success)
                return Result.Failure<(T1, T2, T3, T4, T5)>(result1.remaining, result1.expected);
            var result2 = parser2(result1.remaining);
            if (!result2.success)
                return Result.Failure<(T1, T2, T3, T4, T5)>(result2.remaining, result2.expected);
            var result3 = parser3(result2.remaining);
            if (!result3.success)
                return Result.Failure<(T1, T2, T3, T4, T5)>(result3.remaining, result3.expected);
            var result4 = parser4(result3.remaining);
            if (!result4.success)
                return Result.Failure<(T1, T2, T3, T4, T5)>(result4.remaining, result4.expected);
            var result5 = parser5(result4.remaining);
            if (!result5.success)
                return Result.Failure<(T1, T2, T3, T4, T5)>(result5.remaining, result5.expected);
            return Result.Success(result5.remaining,
                (result1.value, result2.value, result3.value, result4.value, result5.value));
        };
    }

    public static Parser<(T1, T2, T3, T4, T5, T6)> Seq<T1, T2, T3, T4, T5, T6>(Parser<T1> parser1,
        Parser<T2> parser2, Parser<T3> parser3, Parser<T4> parser4, Parser<T5> parser5, Parser<T6> parser6) {
        return input => {
            var result1 = parser1(input);
            if (!result1.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result1.remaining, result1.expected);
            var result2 = parser2(result1.remaining);
            if (!result2.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result2.remaining, result2.expected);
            var result3 = parser3(result2.remaining);
            if (!result3.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result3.remaining, result3.expected);
            var result4 = parser4(result3.remaining);
            if (!result4.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result4.remaining, result4.expected);
            var result5 = parser5(result4.remaining);
            if (!result5.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result5.remaining, result5.expected);
            var result6 = parser6(result5.remaining);
            if (!result6.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result6.remaining, result6.expected);
            return Result.Success(result6.remaining,
                (result1.value, result2.value, result3.value, result4.value, result5.value, result6.value));
        };
    }

    public static Parser<(T1, T2, T3, T4, T5, T6, T7)> Seq<T1, T2, T3, T4, T5, T6, T7>(Parser<T1> parser1,
        Parser<T2> parser2, Parser<T3> parser3, Parser<T4> parser4, Parser<T5> parser5, Parser<T6> parser6,
        Parser<T7> parser7) {
        return input => {
            var result1 = parser1(input);
            if (!result1.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result1.remaining, result1.expected);
            var result2 = parser2(result1.remaining);
            if (!result2.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result2.remaining, result2.expected);
            var result3 = parser3(result2.remaining);
            if (!result3.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result3.remaining, result3.expected);
            var result4 = parser4(result3.remaining);
            if (!result4.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result4.remaining, result4.expected);
            var result5 = parser5(result4.remaining);
            if (!result5.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result5.remaining, result5.expected);
            var result6 = parser6(result5.remaining);
            if (!result6.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result6.remaining, result6.expected);
            var result7 = parser7(result6.remaining);
            if (!result7.success)
                return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result7.remaining, result7.expected);
            return Result.Success(result7.remaining,
                (result1.value, result2.value, result3.value, result4.value, result5.value, result6.value,
                    result7.value));
        };
    }
}
