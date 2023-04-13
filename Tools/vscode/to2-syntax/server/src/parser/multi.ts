import { Input, Parser, ParserFailure, ParserResult, ParserSuccess } from ".";

export function manyM_N<T>(minCount: number | undefined, maxCount: number | undefined,
    item: Parser<T>, description: string): Parser<T[]> {
    return (input: Input) => {
        let remaining = input;
        let result: T[] = [];
        let itemResult: ParserResult<T> = item(input);

        while (itemResult.success) {
            if (remaining.offset === itemResult.remaining.offset) break;

            result.push(itemResult.result);
            remaining = itemResult.remaining;

            itemResult = item(remaining);
        }

        if (minCount && result.length < minCount) {
            return new ParserFailure(`Expected at least ${minCount} ${description}`);
        }
        if (maxCount && result.length > maxCount) {
            return new ParserFailure(`Expected at most ${maxCount} ${description}`);
        }

        return new ParserSuccess(remaining, result);
    }
}

export function many0<T>(item: Parser<T>, description: string): Parser<T[]> {
    return manyM_N(undefined, undefined, item, description);
}

export function many1<T>(item: Parser<T>, description: string): Parser<T[]> {
    return manyM_N(1, undefined, item, description);
}

export function delimitedM_N<T, D>(minCount: number | undefined, maxCount: number | undefined,
    item: Parser<T>, delimiter: Parser<D>, description: string): Parser<T[]> {
    return (input: Input) => {
        let remaining = input;
        let result: T[] = [];
        let itemResult: ParserResult<T> = item(input);

        while (itemResult.success) {
            if (remaining.offset === itemResult.remaining.offset) break;

            result.push(itemResult.result);
            remaining = itemResult.remaining;

            const delimiterResult = delimiter(remaining);
            if (!delimiterResult.success) break;

            itemResult = item(delimiterResult.remaining);
        }

        if (minCount && result.length < minCount) {
            return new ParserFailure(`Expected at least ${minCount} ${description}`);
        }
        if (maxCount && result.length > maxCount) {
            return new ParserFailure(`Expected at most ${maxCount} ${description}`);
        }

        return new ParserSuccess(remaining, result);
    }
}

export function delimited0<T, D>(item: Parser<T>, delimiter: Parser<D>, description: string): Parser<T[]> {
    return delimitedM_N(undefined, undefined, item, delimiter, description);
}

export function delimited1<T, D>(item: Parser<T>, delimiter: Parser<D>, description: string): Parser<T[]> {
    return delimitedM_N(1, undefined, item, delimiter, description);
}

