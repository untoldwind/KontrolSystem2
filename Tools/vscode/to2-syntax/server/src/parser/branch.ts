import { Input, Parser, ParserFailure } from ".";

export function alt<P extends any[]>(
  ...alternatives: { [k in keyof P]: Parser<P[k]> }
): Parser<P[number]> {
  return (input: Input) => {
    let longest = input;
    let expected = new Set<string>();

    for (const parser of alternatives) {
      const result = parser(input);

      if (result.success) return result;

      const longestAt = longest.position.offset;
      const errorAt = result.remaining.position.offset;
      if (errorAt > longestAt) longest = result.remaining;

      expected.add(result.expected);
    }

    return new ParserFailure<P[keyof P]>(
      longest,
      [...expected].sort().join(" or "),
      undefined,
    );
  };
}
