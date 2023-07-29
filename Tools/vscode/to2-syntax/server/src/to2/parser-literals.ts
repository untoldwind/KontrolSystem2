import { isDigit } from "unicode-properties";
import { alt } from "../parser/branch";
import {
  recognizeAs,
  map,
  value,
  opt,
  recognize,
  where,
} from "../parser/combinator";
import {
  chars0,
  charsExcept1,
  digits0,
  digits1,
  oneOf,
  tag,
} from "../parser/complete";
import { many0 } from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import {
  LiteralBool,
  LiteralFloat,
  LiteralInt,
  LiteralString,
} from "./ast/literal";
import { UNDERSCORE, identifier } from "./parser-common";

const doubleQuote = tag('"');

const escapedStringChar = alt(
  charsExcept1('\\"\r\n'),
  recognizeAs(tag("\\\\"), "\\"),
  recognizeAs(tag('\\"'), '"'),
  recognizeAs(tag("\\t"), "\t"),
  recognizeAs(tag("\\r"), "\r"),
  recognizeAs(tag("\\n"), "\n"),
);

export const literalString = map(
  between(doubleQuote, many0(escapedStringChar, "string char"), doubleQuote),
  (escaped, start, end) => new LiteralString(escaped.join(""), start, end),
);

const basePrefix = alt(
  recognizeAs(tag("0x"), 16),
  recognizeAs(tag("0o"), 8),
  recognizeAs(tag("0b"), 2),
  value(10),
);

export const literalInt = map(
  seq(
    opt(tag("-")),
    basePrefix,
    recognize(
      terminated(
        digits1,
        chars0((ch) => isDigit(ch) || ch === UNDERSCORE),
      ),
    ),
  ),
  ([negSign, radix, digits], start, end) =>
    new LiteralInt(
      negSign
        ? -parseInt(digits.replaceAll("_", ""), radix)
        : parseInt(digits.replaceAll("_", ""), radix),
      start,
      end,
    ),
);

const exponentSuffix = seq(oneOf("eE"), opt(oneOf("-+")), digits1);

export const literalFloat = map(
  recognize(
    preceded(
      opt(oneOf("-+")),
      alt(
        terminated(digits0, seq(tag("."), digits1, opt(exponentSuffix))),
        terminated(digits1, exponentSuffix),
      ),
    ),
  ),
  (digits, start, end) => new LiteralFloat(parseFloat(digits), start, end),
);

export const literalBool = alt(
  map(
    where(identifier, (str) => str === "true", "true"),
    (_, start, end) => new LiteralBool(true, start, end),
  ),
  map(
    where(identifier, (str) => str === "false", "false"),
    (_, start, end) => new LiteralBool(false, start, end),
  ),
);
