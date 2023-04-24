import { Input, Parser, ParserFailure } from ".";

export function alt<T>(alternatives: Parser<T>[]): Parser<T> {
  return (input: Input) => {
    let longest = input;
    let expected: string[] = [];

    for (const parser of alternatives) {
      const result = parser(input);

      if (result.success) return result;

      const longestAt = longest.position.offset;
      const errorAt = result.remaining.position.offset;
      if (errorAt > longestAt) longest = result.remaining;

      expected.push(result.expected);
    }

    return new ParserFailure<T>(longest, expected.join(" or "), undefined);
  };
}
