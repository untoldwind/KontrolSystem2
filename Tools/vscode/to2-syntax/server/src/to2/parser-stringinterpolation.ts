import { InputRange, Parser } from "../parser";
import { alt } from "../parser/branch";
import {
  map,
  opt,
  recognize,
  recognizeAs,
  withPosition,
} from "../parser/combinator";
import { charsExcept1, digits1, tag, whitespace0 } from "../parser/complete";
import { many0, many1 } from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import { Expression } from "./ast";
import { StringInterpolation } from "./ast/string-interpolation";
import { doubleQuote } from "./parser-literals";

export const stringInterpolationStart = tag('$"');

export const extendedEscapedStringChar = alt(
  charsExcept1('\\"\r\n{}'),
  recognizeAs(tag("\\\\"), "\\"),
  recognizeAs(tag('\\"'), '"'),
  recognizeAs(tag("\\t"), "\t"),
  recognizeAs(tag("\\r"), "\r"),
  recognizeAs(tag("\\n"), "\n"),
  recognizeAs(tag("\\{"), "{"),
  recognizeAs(tag("\\}"), "}"),
);

export const alignOrFormat = recognize(
  seq(
    opt(
      preceded(
        tag(","),
        between(whitespace0, preceded(opt(tag("-")), digits1), whitespace0),
      ),
    ),
    opt(preceded(tag(":"), charsExcept1('\\"\r\n{}'))),
  ),
);

function stringInterpolationContent(expression: Parser<Expression>) {
  return many0(
    alt(
      map(
        many1(extendedEscapedStringChar, "string char"),
        (value, start, end) => ({
          value: value.join(""),
          range: new InputRange(start, end),
        }),
      ),
      between(
        terminated(tag("{"), whitespace0),
        map(
          seq(expression, opt(alignOrFormat)),
          ([expression, alignOrFormat]) => ({
            expression,
            alignOrFormat,
          }),
        ),
        preceded(whitespace0, tag("}")),
      ),
    ),
    "string char",
  );
}

export function stringInterpolation(expression: Parser<Expression>) {
  return map(
    seq(
      withPosition(stringInterpolationStart),
      stringInterpolationContent(expression),
      withPosition(doubleQuote),
    ),
    ([start, parts, end]) =>
      new StringInterpolation(parts, start.range, end.range),
  );
}
