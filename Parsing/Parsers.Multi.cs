using System;
using System.Collections.Generic;

namespace KontrolSystem.Parsing;

public static partial class Parsers {
    /// <summary>
    ///     Parse n to m items.
    /// </summary>
    public static Parser<List<T>> ManyN_M<T>(int? minCount, int? maxCount, Parser<T> itemParser,
        string description = "items") {
        return input => {
            var remaining = input;
            var result = new List<T>();
            var itemResult = itemParser(input);

            while (itemResult.success) {
                if (remaining.Position == itemResult.remaining.Position) break;

                result.Add(itemResult.value);
                remaining = itemResult.remaining;

                itemResult = itemParser(remaining);
            }

            if (minCount.HasValue && result.Count < minCount)
                return Result.Failure<List<T>>(input, $"Expected at least {minCount} {description}");
            if (maxCount.HasValue && result.Count > maxCount)
                return Result.Failure<List<T>>(input, $"Expected at most {minCount} {description}");

            return Result.Success(remaining, result);
        };
    }

    /// <summary>
    ///     Parser zero or more items.
    /// </summary>
    public static Parser<List<T>> Many0<T>(Parser<T> itemParser, string description = "items") {
        return ManyN_M(null, null, itemParser, description);
    }

    /// <summary>
    ///     Parser one or more items.
    /// </summary>
    public static Parser<List<T>> Many1<T>(Parser<T> itemParser, string description = "items") {
        return ManyN_M(1, null, itemParser, description);
    }

    /// <summary>
    ///     Parser n to m items separated by a delimiter.
    /// </summary>
    public static Parser<List<T>> DelimitedN_M<T, D>(int? minCount, int? maxCount, Parser<T> itemParser,
        Parser<D> delimiter, string description = "items") {
        return input => {
            var remaining = input;
            var result = new List<T>();
            var itemResult = itemParser(input);

            while (itemResult.success) {
                if (remaining.Position == itemResult.remaining.Position) break;

                result.Add(itemResult.value);
                remaining = itemResult.remaining;

                var delimiterResult = delimiter(remaining);
                if (!delimiterResult.success) break;

                itemResult = itemParser(delimiterResult.remaining);
            }

            if (minCount.HasValue && result.Count < minCount)
                return Result.Failure<List<T>>(input, $"Expected at least {minCount} {description}");
            if (maxCount.HasValue && result.Count > maxCount)
                return Result.Failure<List<T>>(input, $"Expected at most {minCount} {description}");

            return Result.Success(remaining, result);
        };
    }

    /// <summary>
    ///     Parser zero or more items separated by a delimiter.
    /// </summary>
    public static Parser<List<T>> Delimited0<T, D>(Parser<T> itemParser, Parser<D> delimiter,
        string description = "items") {
        return DelimitedN_M(null, null, itemParser, delimiter, description);
    }

    /// <summary>
    ///     Parser one or more items separated by a delimiter.
    /// </summary>
    public static Parser<List<T>> Delimited1<T, D>(Parser<T> itemParser, Parser<D> delimiter,
        string description = "items") {
        return DelimitedN_M(1, null, itemParser, delimiter, description);
    }

    /// <summary>
    ///     Parse any number of items until an end condition is met.
    /// </summary>
    public static Parser<List<T>> DelimitedUntil<T, D, E>(Parser<T> itemParser, Parser<D> delimiter, Parser<E> end,
        string description = "item") {
        return input => {
            var remaining = input;
            var result = new List<T>();
            var endResult = end(remaining);

            if (endResult.success) return Result.Success(endResult.remaining, result);

            while (remaining.Available > 0) {
                var itemResult = itemParser(remaining);
                if (!itemResult.success)
                    return Result.Failure<List<T>>(itemResult.remaining, itemResult.expected);
                if (remaining.Position == itemResult.remaining.Position)
                    return Result.Failure<List<T>>(remaining, description);

                result.Add(itemResult.value);
                remaining = itemResult.remaining;

                endResult = end(remaining);
                if (endResult.success) return Result.Success(endResult.remaining, result);

                var delimiterResult = delimiter(remaining);
                if (!delimiterResult.success) return Result.Failure<List<T>>(remaining, delimiterResult.expected);

                remaining = delimiterResult.remaining;

                endResult = end(remaining);
                if (endResult.success) return Result.Success(endResult.remaining, result);
            }

            return Result.Failure<List<T>>(remaining, endResult.expected);
        };
    }

    /// <summary>
    ///     Chain a left-associative operator.
    /// </summary>
    public static Parser<T> Chain<T, OP>(Parser<T> operantParser, Parser<OP> opParser,
        Func<T, OP, T, Position, Position, T> apply) {
        var restParser = Seq(opParser, operantParser);

        return input => {
            var firstResult = operantParser(input);

            if (!firstResult.success) return firstResult;

            var remaining = firstResult.remaining;
            var result = firstResult.value;

            var restResult = restParser(remaining);

            while (restResult.success) {
                if (remaining.Position == restResult.remaining.Position)
                    break;

                var (op, operant) = restResult.value;
                result = apply(result, op, operant, remaining.Position, restResult.Position);
                remaining = restResult.remaining;

                restResult = restParser(remaining);
            }

            return Result.Success(remaining, result);
        };
    }

    /// <summary>
    ///     Fold an initial parser with zero or more successors.
    /// </summary>
    public static Parser<T> Fold0<T, S>(this Parser<T> initial, Parser<S> suffix,
        Func<T, S, Position, Position, T> combine) {
        return input => {
            var result = initial(input);
            if (!result.success) return result;

            var suffixResult = suffix(result.remaining);
            while (suffixResult.success) {
                if (suffixResult.Position == result.Position) break;

                result = Result.Success(suffixResult.remaining,
                    combine(result.value, suffixResult.value, result.Position, suffixResult.Position));
                suffixResult = suffix(result.remaining);
            }

            return result;
        };
    }
}
