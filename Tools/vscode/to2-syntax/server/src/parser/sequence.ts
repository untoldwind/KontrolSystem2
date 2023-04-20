import { Input, Parser, ParserFailure, ParserSuccess } from ".";

export function then<T, U>(
  first: Parser<T>,
  second: (result: T) => Parser<U>
): Parser<U> {
  return (input: Input) =>
    first(input).select((s) => second(s.result)(s.remaining));
}

export function preceded<T, U>(
  prefix: Parser<T>,
  parser: Parser<U>
): Parser<U> {
  return (input: Input) => prefix(input).select((s) => parser(s.remaining));
}

export function terminated<T, U>(
  parser: Parser<T>,
  suffix: Parser<U>
): Parser<T> {
  return (input: Input) =>
    parser(input).select((s) => suffix(s.remaining).map((_) => s.result));
}

export function between<T, P, S>(
  prefix: Parser<P>,
  parser: Parser<T>,
  suffix: Parser<S>
): Parser<T> {
  return (input: Input) =>
    prefix(input)
      .select((s) => parser(s.remaining))
      .select((s) => suffix(s.remaining).map((_) => s.result));
}

export function seq<P extends any[]>(
  items: [...{ [k in keyof P]: Parser<P[k]> }]
): Parser<P> {
  return (input: Input) => {
    let remaining = input;
    let result: any[] = [];

    for (const item of items) {
      const itemResult = item(remaining);
      if (!itemResult.success)
        return new ParserFailure(remaining, itemResult.expected);
      result.push(itemResult.result);
      remaining = itemResult.remaining;
    }

    return new ParserSuccess(remaining, result as P);
  };
}
