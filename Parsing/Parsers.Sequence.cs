using System;

namespace KontrolSystem.Parsing {
    public static partial class Parsers {
        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// </summary>
        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second) => input =>
            first(input).Select(s => second(s.Value)(s.Remaining));

        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// </summary>
        public static Parser<U> Then<T, U>(this Parser<T> first, Parser<U> second) => input =>
            first(input).Select(s => second(s.Remaining));

        /// <summary>
        /// Monadic combinator Then, adapted for Linq comprehension syntax.
        /// </summary>
        public static Parser<V> SelectMany<T, U, V>(
            this Parser<T> parser,
            Func<T, Parser<U>> selector,
            Func<T, U, V> projector) => parser.Then(t => selector(t).Map(u => projector(t, u)));

        public static Parser<T> Preceded<T, P>(Parser<P> prefix, Parser<T> parser) => input =>
            prefix(input).Select(p => parser(p.Remaining));

        public static Parser<T> Terminated<T, S>(Parser<T> parser, Parser<S> suffix) => input =>
            parser(input).Select(t => suffix(t.Remaining).Map(_ => t.Value));

        /// <summary>
        /// Parse an item between a given prefix and suffix.
        /// </summary>
        public static Parser<T> Between<T, P, S>(this Parser<T> parser, Parser<P> prefix, Parser<S> suffix) =>
            input =>
                prefix(input).Select(p => parser(p.Remaining).Select(t => suffix(t.Remaining).Map(_ => t.Value)));

        public static Parser<(T1, T2)> Seq<T1, T2>(Parser<T1> parser1, Parser<T2> parser2) => input => {
            IResult<T1> result1 = parser1(input);
            if (!result1.WasSuccessful) return Result.Failure<(T1, T2)>(result1.Remaining, result1.Expected);
            IResult<T2> result2 = parser2(result1.Remaining);
            if (!result2.WasSuccessful) return Result.Failure<(T1, T2)>(result2.Remaining, result2.Expected);
            return Result.Success(result2.Remaining, (result1.Value, result2.Value));
        };

        public static Parser<(T1, T2, T3)>
            Seq<T1, T2, T3>(Parser<T1> parser1, Parser<T2> parser2, Parser<T3> parser3) => input => {
                IResult<T1> result1 = parser1(input);
                if (!result1.WasSuccessful) return Result.Failure<(T1, T2, T3)>(result1.Remaining, result1.Expected);
                IResult<T2> result2 = parser2(result1.Remaining);
                if (!result2.WasSuccessful) return Result.Failure<(T1, T2, T3)>(result2.Remaining, result2.Expected);
                IResult<T3> result3 = parser3(result2.Remaining);
                if (!result3.WasSuccessful) return Result.Failure<(T1, T2, T3)>(result3.Remaining, result3.Expected);
                return Result.Success(result3.Remaining, (result1.Value, result2.Value, result3.Value));
            };

        public static Parser<(T1, T2, T3, T4)> Seq<T1, T2, T3, T4>(Parser<T1> parser1, Parser<T2> parser2,
            Parser<T3> parser3, Parser<T4> parser4) => input => {
                IResult<T1> result1 = parser1(input);
                if (!result1.WasSuccessful) return Result.Failure<(T1, T2, T3, T4)>(result1.Remaining, result1.Expected);
                IResult<T2> result2 = parser2(result1.Remaining);
                if (!result2.WasSuccessful) return Result.Failure<(T1, T2, T3, T4)>(result2.Remaining, result2.Expected);
                IResult<T3> result3 = parser3(result2.Remaining);
                if (!result3.WasSuccessful) return Result.Failure<(T1, T2, T3, T4)>(result3.Remaining, result3.Expected);
                IResult<T4> result4 = parser4(result3.Remaining);
                if (!result4.WasSuccessful) return Result.Failure<(T1, T2, T3, T4)>(result4.Remaining, result4.Expected);
                return Result.Success(result4.Remaining, (result1.Value, result2.Value, result3.Value, result4.Value));
            };

        public static Parser<(T1, T2, T3, T4, T5)> Seq<T1, T2, T3, T4, T5>(Parser<T1> parser1, Parser<T2> parser2,
            Parser<T3> parser3, Parser<T4> parser4, Parser<T5> parser5) => input => {
                IResult<T1> result1 = parser1(input);
                if (!result1.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5)>(result1.Remaining, result1.Expected);
                IResult<T2> result2 = parser2(result1.Remaining);
                if (!result2.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5)>(result2.Remaining, result2.Expected);
                IResult<T3> result3 = parser3(result2.Remaining);
                if (!result3.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5)>(result3.Remaining, result3.Expected);
                IResult<T4> result4 = parser4(result3.Remaining);
                if (!result4.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5)>(result4.Remaining, result4.Expected);
                IResult<T5> result5 = parser5(result4.Remaining);
                if (!result5.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5)>(result5.Remaining, result5.Expected);
                return Result.Success(result5.Remaining,
                    (result1.Value, result2.Value, result3.Value, result4.Value, result5.Value));
            };

        public static Parser<(T1, T2, T3, T4, T5, T6)> Seq<T1, T2, T3, T4, T5, T6>(Parser<T1> parser1,
            Parser<T2> parser2, Parser<T3> parser3, Parser<T4> parser4, Parser<T5> parser5, Parser<T6> parser6) =>
            input => {
                IResult<T1> result1 = parser1(input);
                if (!result1.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result1.Remaining, result1.Expected);
                IResult<T2> result2 = parser2(result1.Remaining);
                if (!result2.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result2.Remaining, result2.Expected);
                IResult<T3> result3 = parser3(result2.Remaining);
                if (!result3.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result3.Remaining, result3.Expected);
                IResult<T4> result4 = parser4(result3.Remaining);
                if (!result4.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result4.Remaining, result4.Expected);
                IResult<T5> result5 = parser5(result4.Remaining);
                if (!result5.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result5.Remaining, result5.Expected);
                IResult<T6> result6 = parser6(result5.Remaining);
                if (!result6.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6)>(result6.Remaining, result6.Expected);
                return Result.Success(result6.Remaining,
                    (result1.Value, result2.Value, result3.Value, result4.Value, result5.Value, result6.Value));
            };

        public static Parser<(T1, T2, T3, T4, T5, T6, T7)> Seq<T1, T2, T3, T4, T5, T6, T7>(Parser<T1> parser1,
            Parser<T2> parser2, Parser<T3> parser3, Parser<T4> parser4, Parser<T5> parser5, Parser<T6> parser6, Parser<T7> parser7) =>
            input => {
                IResult<T1> result1 = parser1(input);
                if (!result1.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result1.Remaining, result1.Expected);
                IResult<T2> result2 = parser2(result1.Remaining);
                if (!result2.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result2.Remaining, result2.Expected);
                IResult<T3> result3 = parser3(result2.Remaining);
                if (!result3.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result3.Remaining, result3.Expected);
                IResult<T4> result4 = parser4(result3.Remaining);
                if (!result4.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result4.Remaining, result4.Expected);
                IResult<T5> result5 = parser5(result4.Remaining);
                if (!result5.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result5.Remaining, result5.Expected);
                IResult<T6> result6 = parser6(result5.Remaining);
                if (!result6.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result6.Remaining, result6.Expected);
                IResult<T7> result7 = parser7(result6.Remaining);
                if (!result7.WasSuccessful)
                    return Result.Failure<(T1, T2, T3, T4, T5, T6, T7)>(result7.Remaining, result7.Expected);
                return Result.Success(result7.Remaining,
                    (result1.Value, result2.Value, result3.Value, result4.Value, result5.Value, result6.Value, result7.Value));
            };
    }
}
