import { Input, Parser } from ".";

function then<T, U>(first: Parser<T>, second: (result: T) => Parser<U>) : Parser<U> {
    return (input: Input) => first(input).select(s => second(s.result)(s.remaining));
}

function preceded<T, U>(prefix: Parser<T>, parser: Parser<U>) : Parser<U> {
    return (input: Input) => prefix(input).select(s => parser(s.remaining));
}

function terminated<T, U>(parser: Parser<T>, suffix: Parser<U>) : Parser<T> {
    return (input: Input) => parser(input).select(s => suffix(s.remaining).map(_ => s.result));
}