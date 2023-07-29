import { isAlphabetic, isDigit } from "unicode-properties";
import { Input, ParserResult } from "../parser";
import { alt } from "../parser/branch";
import {
  map,
  opt,
  recognize,
  recognizeAs,
  where,
  withPosition,
} from "../parser/combinator";
import {
  char,
  chars0,
  charsExcept0,
  peekLineEnd,
  spacing0,
  spacing1,
  tag,
  whitespace0,
} from "../parser/complete";
import {
  delimited0,
  delimited1,
  delimitedM_N,
  delimitedUntil,
  many0,
} from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import { FunctionType } from "./ast/function-type";
import { LineComment } from "./ast/line-comment";
import { RecordType } from "./ast/record-type";
import { TO2Type, findLibraryType } from "./ast/to2-type";
import { TupleType } from "./ast/tuple-type";
import { LookupTypeReference } from "./ast/type-reference";
import {
  DeclarationParameter,
  DeclarationPlaceholder,
} from "./ast/variable-declaration";
import { ArrayType } from "./ast/array-type";

const RESERVED_KEYWORDS = new Set<string>([
  "pub",
  "fn",
  "let",
  "const",
  "if",
  "else",
  "return",
  "break",
  "continue",
  "while",
  "_",
  "for",
  "in",
  "as",
  "sync",
  "type",
  "struct",
  "impl",
]);

export const pubKeyword = terminated(withPosition(tag("pub")), spacing1);

export const letKeyword = terminated(withPosition(tag("let")), spacing1);

export const constKeyword = terminated(withPosition(tag("const")), spacing1);

export const lineComment = map(
  between(preceded(whitespace0, tag("//")), charsExcept0("\r\n"), peekLineEnd),
  (comment, start, end) => new LineComment(comment, start, end),
);

export const lineComments = delimited0(
  lineComment,
  whitespace0,
  "<line comments>",
);

export const UNDERSCORE = "_".charCodeAt(0);

export const identifier = where(
  recognize(
    preceded(
      char((ch) => isAlphabetic(ch) || ch === UNDERSCORE, "letter or _"),
      chars0((ch) => isAlphabetic(ch) || isDigit(ch) || ch === UNDERSCORE),
    ),
  ),
  (result) => !RESERVED_KEYWORDS.has(result),
  "Not a keyword",
);

export const identifierPath = delimited1(
  identifier,
  tag("::"),
  "<identifier path>",
);

export const commaDelimiter = between(whitespace0, tag(","), whitespace0);

export const eqDelimiter = between(whitespace0, tag("="), whitespace0);

export const typeSpec = preceded(
  between(whitespace0, tag(":"), whitespace0),
  withPosition(typeRef),
);

const functionTypeParameters = preceded(
  terminated(tag("("), whitespace0),
  delimitedUntil(
    typeRef,
    commaDelimiter,
    preceded(whitespace0, tag(")")),
    "<function parameter type",
  ),
);

const functionType = map(
  seq(
    preceded(terminated(tag("fn"), whitespace0), functionTypeParameters),
    preceded(between(whitespace0, tag("->"), whitespace0), typeRef),
  ),
  ([parameterTypes, returnType]) =>
    new FunctionType(
      false,
      parameterTypes.map((type, idx) => [`param${idx + 1}`, type, false]),
      returnType,
    ),
);

const tupleType = map(
  between(
    terminated(tag("("), whitespace0),
    delimitedM_N(
      2,
      undefined,
      preceded(opt(terminated(lineComment, whitespace0)), typeRef),
      commaDelimiter,
      "<type>",
    ),
    preceded(whitespace0, tag(")")),
  ),
  (items) => new TupleType(items),
);

const recordType = map(
  between(
    terminated(tag("("), whitespace0),
    delimited1(
      preceded(
        opt(terminated(lineComment, whitespace0)),
        seq(withPosition(identifier), typeSpec),
      ),
      commaDelimiter,
      "<identifier : type>",
    ),
    preceded(whitespace0, tag(")")),
  ),
  (items) => new RecordType(items.map(([name, type]) => [name, type.value])),
);

const typeReference = map(
  seq(
    identifierPath,
    opt(
      between(
        preceded(whitespace0, tag("<")),
        delimited0(typeRef, commaDelimiter, "<type ref>"),
        preceded(whitespace0, tag(">")),
      ),
    ),
  ),
  ([name, typeArguments], start, end) =>
    (!typeArguments ? findLibraryType(name, []) : undefined) ??
    new LookupTypeReference(name, typeArguments ?? [], start, end),
);

const topLevelTypeRef = map(
  seq(
    alt(functionType, typeReference, tupleType, recordType),
    many0(
      preceded(spacing0, between(tag("["), spacing0, tag("]"))),
      "<array type>",
    ),
  ),
  ([baseType, arrayDim]) =>
    arrayDim.length > 0 ? new ArrayType(baseType, arrayDim.length) : baseType,
);

export function typeRef(input: Input): ParserResult<TO2Type> {
  return topLevelTypeRef(input);
}

export const declarationParameter = map(
  seq(
    withPosition(identifier),
    opt(preceded(between(whitespace0, tag("@"), whitespace0), identifier)),
    opt(typeSpec),
  ),
  ([target, source, type]) => new DeclarationParameter(target, source, type),
);

export const declarationParameterOrPlaceholder = alt(
  declarationParameter,
  recognizeAs(tag("_"), new DeclarationPlaceholder()),
);

export const descriptionComment = map(
  many0(
    between(
      preceded(whitespace0, tag("///")),
      charsExcept0("\r\n"),
      peekLineEnd,
    ),
    "<description>",
  ),
  (lines) => lines.join("\n"),
);
