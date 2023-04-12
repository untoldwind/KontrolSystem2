import { Input, Parser, ParserSuccess } from ".";

export function value<T>(value: T) : Parser<T> {
    return (input: Input) => new ParserSuccess<T>(input, value);
}