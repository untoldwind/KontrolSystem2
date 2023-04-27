import {
  getCategory,
  isDigit,
  isWhiteSpace as utf8IsWhiteSpace,
} from "unicode-properties";
import { Input, Parser, ParserFailure, ParserResult, ParserSuccess } from ".";

export function char(
  predicate: (charCode: number) => boolean,
  expected: string
): Parser<string> {
  return (input: Input) => {
    if (input.available < 1) return new ParserFailure(input, expected, "");
    const current = input.take(1);
    if (!predicate(current.charCodeAt(0)))
      return new ParserFailure(input, expected, "");
    return new ParserSuccess(input.advance(1), current);
  };
}

export function oneOf(candidates: string): Parser<string> {
  const candidateCodes = toCharCodes(candidates);
  return char((ch) => candidateCodes.indexOf(ch) >= 0, `one of ${candidates}`);
}

export function chars0(
  predicate: (charCode: number) => boolean
): Parser<string> {
  return (input: Input) => {
    let count = input.findNext((ch) => !predicate(ch));
    if (count < 0) count = input.available;
    return new ParserSuccess(input.advance(count), input.take(count));
  };
}

export function chars1(
  predicate: (charCode: number) => boolean,
  expected: string
): Parser<string> {
  return (input: Input) => {
    let count = input.findNext((ch) => !predicate(ch));
    if (count < 0) count = input.available;
    if (count === 0) return new ParserFailure(input, expected, "");
    return new ParserSuccess(input.advance(count), input.take(count));
  };
}

export function charsExcept0(forbidden: string): Parser<string> {
  const forbiddenCodes = toCharCodes(forbidden);
  return chars0((ch) => forbiddenCodes.indexOf(ch) < 0);
}

export function charsExcept1(forbidden: string): Parser<string> {
  const forbiddenCodes = toCharCodes(forbidden);
  return chars1(
    (ch) => forbiddenCodes.indexOf(ch) < 0,
    `any character except ${forbidden}`
  );
}

const ADDITIONAL_WHITESPACE_CHARS = toCharCodes("\n\t\r\f\v");

export function isWhitespace(ch: number): boolean {
  return utf8IsWhiteSpace(ch) || ADDITIONAL_WHITESPACE_CHARS.indexOf(ch) >= 0;
}

export const whitespace0 = chars0(isWhitespace);

export const whitespace1 = chars1(isWhitespace, "<whitespace>");

export const digits0 = chars0(isDigit);

export const digits1 = chars1(isDigit, "<digit>");

export const NL = "\n".charCodeAt(0);

export const TAB = "\t".charCodeAt(0);

export const spacing0 = chars0((ch) => ch === TAB || getCategory(ch) === "Zs");

export const spacing1 = chars1(
  (ch) => ch === TAB || getCategory(ch) === "Zs",
  "<space>"
);

export function tag<T extends string>(tag: T): Parser<T> {
  return (input: Input) => {
    if (input.available < tag.length)
      return new ParserFailure<T>(input, tag, undefined);
    const content = input.take(tag.length);
    if (content !== tag)
      return new ParserFailure<T>(input, `Expected ${tag}`, undefined);
    return new ParserSuccess(input.advance(tag.length), content as T);
  };
}

export function peekLineEnd(input: Input): ParserResult<boolean> {
  if (input.available === 0) return new ParserSuccess(input, false);
  if (input.take(1) === "\n" || input.take(2) == "\r\n")
    return new ParserSuccess(input, true);
  return new ParserFailure(input, "<end of line>", false);
}

export function eof(input: Input): ParserResult<void> {
  if (input.available > 0) return new ParserFailure(input, "<EOF>", undefined);
  return new ParserSuccess(input, undefined);
}

export function toCharCodes(chars: string): number[] {
  const result = new Array<number>(chars.length);
  for (let i = 0; i < chars.length; i++) {
    result[i] = chars.charCodeAt(i);
  }
  return result;
}
