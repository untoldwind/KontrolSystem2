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
  chars1,
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

export const doubleQuote = tag('"');

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

const isHexDigit = (ch: number) =>
  (ch >= "0".charCodeAt(0) && ch <= "9".charCodeAt(0)) ||
  (ch >= "a".charCodeAt(0) && ch <= "f".charCodeAt(0)) ||
  (ch >= "A".charCodeAt(0) && ch <= "F".charCodeAt(0));

const isOctalDigit = (ch: number) =>
  ch >= "0".charCodeAt(0) && ch <= "7".charCodeAt(0);

const isBinaryDigit = (ch: number) =>
  ch === "0".charCodeAt(0) || ch === "1".charCodeAt(0);

const basePrefixed = alt(
  preceded(
    tag("0x"),
    map(
      recognize(
        terminated(
          chars1(isHexDigit, "<hexdigit>"),
          chars0((ch) => isHexDigit(ch) || ch === UNDERSCORE),
        ),
      ),
      (digits) => ({ radix: 16, digits }),
    ),
  ),
  preceded(
    tag("0o"),
    map(
      recognize(
        terminated(
          chars1(isOctalDigit, "<octal>"),
          chars0((ch) => isOctalDigit(ch) || ch === UNDERSCORE),
        ),
      ),
      (digits) => ({ radix: 8, digits }),
    ),
  ),
  preceded(
    tag("0b"),
    map(
      recognize(
        terminated(
          chars1(isBinaryDigit, "<binary>"),
          chars0((ch) => isBinaryDigit(ch) || ch === UNDERSCORE),
        ),
      ),
      (digits) => ({ radix: 2, digits }),
    ),
  ),
  map(
    recognize(
      terminated(
        digits1,
        chars0((ch) => isDigit(ch) || ch === UNDERSCORE),
      ),
    ),
    (digits) => ({ radix: 10, digits }),
  ),
);

export const literalInt = map(
  seq(opt(tag("-")), basePrefixed),
  ([negSign, { radix, digits }], start, end) =>
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
