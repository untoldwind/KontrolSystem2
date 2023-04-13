import { Input, Parser, ParserFailure } from ".";

export function alt<T>(alternatives: Parser<T>[]): Parser<T> {
    return (input: Input) => {
        let expected : string[] = [];

        for(const parser of alternatives) {
            const result = parser(input);

            if(result.success) return result;

            expected.push(result.expected);
        }

        return new ParserFailure(expected.join(" or "));
    }
}