import { isAlphabetic, isDigit } from "unicode-properties";
import { Input, ParserResult } from "../parser";
import { alt } from "../parser/branch";
import { map, opt, recognize, recognizeAs, where } from "../parser/combinator";
import {
  char,
  chars0,
  charsExcept0,
  peekLineEnd,
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
import { TO2Type, getBuiltinType } from "./ast/to2-type";
import { TupleType } from "./ast/tuple-type";
import { LookupTypeReference } from "./ast/type-reference";
import { DeclarationParameter } from "./ast/variable-declaration";

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

export const pubKeyword = terminated(tag("pub"), spacing1);

export const letKeyword = terminated(tag("let"), spacing1);

export const constKeyword = terminated(tag("const"), spacing1);

export const lineComment = map(
  between(preceded(whitespace0, tag("//")), charsExcept0("\r\n"), peekLineEnd),
  (comment, start, end) => new LineComment(comment, start, end)
);

export const lineComments = delimited0(
  lineComment,
  whitespace0,
  "<line comments>"
);

export const UNDERSCORE = "_".charCodeAt(0);

export const identifier = where(
  recognize(
    preceded(
      char((ch) => isAlphabetic(ch) || ch === UNDERSCORE, "letter or _"),
      chars0((ch) => isAlphabetic(ch) || isDigit(ch) || ch === UNDERSCORE)
    )
  ),
  (result) => !RESERVED_KEYWORDS.has(result),
  "Not a keyword"
);

export const identifierPath = delimited1(
  identifier,
  tag("::"),
  "<identifier path>"
);

export const commaDelimiter = between(whitespace0, tag(","), whitespace0);

export const eqDelimiter = between(whitespace0, tag("="), whitespace0);

export const typeSpec = preceded(
  between(whitespace0, tag(":"), whitespace0),
  typeRef
);

const functionTypeParameters = preceded(
  terminated(tag("("), whitespace0),
  delimitedUntil(
    typeRef,
    commaDelimiter,
    preceded(whitespace0, tag(")")),
    "<function parameter type"
  )
);

const functionType = map(
  seq([
    preceded(terminated(tag("fn"), whitespace0), functionTypeParameters),
    preceded(between(whitespace0, tag("->"), whitespace0), typeRef),
  ]),
  ([parameterTypes, returnType]) =>
    new FunctionType(false, parameterTypes, returnType)
);

const tupleType = map(
  between(
    terminated(tag("("), whitespace0),
    delimitedM_N(
      2,
      undefined,
      preceded(opt(terminated(lineComment, whitespace0)), typeRef),
      commaDelimiter,
      "<type>"
    ),
    preceded(whitespace0, tag(")"))
  ),
  (items) => new TupleType(items)
);

const recordType = map(
  between(
    terminated(tag("("), whitespace0),
    delimited1(
      preceded(
        opt(terminated(lineComment, whitespace0)),
        seq([identifier, typeRef])
      ),
      commaDelimiter,
      "<identifier : type>"
    ),
    preceded(whitespace0, tag(")"))
  ),
  (items) => new RecordType(items)
);

const typeReference = map(
  seq([
    identifierPath,
    opt(
      between(
        preceded(whitespace0, tag("<")),
        delimited0(typeRef, commaDelimiter, "<type ref>"),
        preceded(whitespace0, tag(">"))
      )
    ),
  ]),
  ([name, typeArguments], start, end) =>
    getBuiltinType(name, typeArguments ?? []) ??
    new LookupTypeReference(name, typeArguments ?? [], start, end)
);

const topLevelTypeRef = alt([
  functionType,
  typeReference,
  tupleType,
  recordType,
]);

export function typeRef(input: Input): ParserResult<TO2Type> {
  return topLevelTypeRef(input);
}

export const declarationParameter = map(
  seq([
    identifier,
    opt(preceded(between(whitespace0, tag("@"), whitespace0), identifier)),
    opt(typeSpec),
  ]),
  ([target, source, type]) => new DeclarationParameter(target, source, type)
);

export const declarationParameterOrPlaceholder = alt([
  declarationParameter,
  recognizeAs(
    tag("_"),
    new DeclarationParameter(undefined, undefined, undefined)
  ),
]);

export const descriptionComment = map(
  many0(
    between(
      preceded(whitespace0, tag("///")),
      charsExcept0("\r\n"),
      peekLineEnd
    ),
    "<description>"
  ),
  (lines) => lines.join("\n")
);
