import { InputRange, Parser, ParserFailure, ParserSuccess } from "../parser";
import { alt } from "../parser/branch";
import {
  map,
  opt,
  recognize,
  recognizeAs,
  recover,
  withPosition,
} from "../parser/combinator";
import {
  NL,
  charsExcept1,
  digits1,
  tag,
  whitespace0,
} from "../parser/complete";
import { many0, many1 } from "../parser/multi";
import {
  between,
  ifPreceded,
  preceded,
  seq,
  terminated,
} from "../parser/sequence";
import { Expression } from "./ast";
import { ErrorNode } from "./ast/error-node";
import {
  StringInterpolation,
  StringInterpolationPart,
} from "./ast/string-interpolation";
import { doubleQuote } from "./parser-literals";

export const stringInterpolationStart = tag('$"');

export const extendedEscapedStringChar = alt(
  charsExcept1('\\"\r\n{}'),
  recognizeAs(tag("\\\\"), "\\"),
  recognizeAs(tag('\\"'), '"'),
  recognizeAs(tag("\\t"), "\t"),
  recognizeAs(tag("\\r"), "\r"),
  recognizeAs(tag("\\n"), "\n"),
  recognizeAs(tag("{{"), "{{"),
  recognizeAs(tag("}}"), "}}"),
);

export const alignOrFormat = recognize(
  seq(
    ifPreceded(
      tag(","),
      between(whitespace0, preceded(opt(tag("-")), digits1), whitespace0),
    ),
    ifPreceded(tag(":"), charsExcept1('\\"\r\n{}')),
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
      preceded(
        terminated(tag("{"), whitespace0),
        recover(
          terminated(
            map(
              seq(expression, alignOrFormat),
              ([expression, alignOrFormat]) => ({
                expression,
                alignOrFormat,
              }),
            ),
            preceded(whitespace0, tag("}")),
          ),
          recoverStringInterpolationContent,
        ),
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
    ([start, parts, end]) => {
      return new StringInterpolation(parts, start.range, end.range);
    },
  );
}

function recoverStringInterpolationContent(
  failure: ParserFailure<StringInterpolationPart>,
): ParserSuccess<StringInterpolationPart> {
  const remaining = failure.remaining;
  const nextNL = remaining.findNext(
    (ch) => ch === "}".codePointAt(0) || ch == '"'.codePointAt(0) || ch === NL,
  );
  let recoverAt = remaining.advance(nextNL >= 0 ? nextNL : remaining.available);
  if (recoverAt.take(1) === "}") recoverAt = recoverAt.advance(1);

  return new ParserSuccess(recoverAt, {
    expression: new ErrorNode(
      failure.expected,
      remaining.position,
      recoverAt.position,
    ),
  });
}
