import { isDigit } from "unicode-properties";
import { recognize, where } from "../parser/combinator";
import { char, chars0, tag, whitespace0 } from "../parser/complete";
import { between, preceded } from "../parser/sequence";
import { isAlphabetic } from "unicode-properties";
import { delimited1 } from "../parser/multi";

const RESERVED_KEYWORDS = new Set<string>(["pub", "fn", "let", "const", "if", "else", "return", "break", "continue", "while", "_", "for", "in", "as", "sync", "type", "struct", "impl"]);

export const pubKeyword = tag("pub");

export const letKeyword = tag("let");

export const UNDERSCORE = "_".charCodeAt(0);

export const identifier = where(
    recognize(preceded(char(ch => isAlphabetic(ch) || ch === UNDERSCORE, "letter or _"), chars0(ch => isAlphabetic(ch) || isDigit(ch) || ch === UNDERSCORE))),
    result => !RESERVED_KEYWORDS.has(result),
    "Not a keyword"
);

export const identifierPath = delimited1(identifier, tag("::"), "<identifier>");

export const commaDelimiter = between(whitespace0, tag(","), whitespace0);
