import { alt } from "../parser/branch";
import { map, opt, recognizeAs } from "../parser/combinator";
import { spacing1, tag, whitespace0 } from "../parser/complete";
import { delimitedUntil } from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import {
  FunctionDeclaration,
  FunctionModifier,
  FunctionParameter,
} from "./ast/function-declaration";
import {
  commaDelimiter,
  descriptionComment,
  identifier,
  pubKeyword,
  typeRef,
  typeSpec,
} from "./parser-common";
import { expression } from "./parser-expression";

const fnKeyword = terminated(tag("fn"), spacing1);

const testKeyword = terminated(tag("test"), spacing1);

const syncKeyword = terminated(tag("sync"), spacing1);

const selfKeyword = tag("self");

const functionPrefix = alt([
  recognizeAs(seq([syncKeyword, pubKeyword, fnKeyword]), {
    modifier: FunctionModifier.Public,
    async: false,
  }),
  recognizeAs(seq([syncKeyword, testKeyword, fnKeyword]), {
    modifier: FunctionModifier.Test,
    async: false,
  }),
  recognizeAs(seq([pubKeyword, fnKeyword]), {
    modifier: FunctionModifier.Public,
    async: true,
  }),
  recognizeAs(seq([pubKeyword, syncKeyword, fnKeyword]), {
    modifier: FunctionModifier.Public,
    async: false,
  }),
  recognizeAs(seq([testKeyword, fnKeyword]), {
    modifier: FunctionModifier.Test,
    async: true,
  }),
  recognizeAs(seq([testKeyword, syncKeyword, fnKeyword]), {
    modifier: FunctionModifier.Test,
    async: false,
  }),
  recognizeAs(seq([syncKeyword, fnKeyword]), {
    modifier: FunctionModifier.Private,
    async: false,
  }),
  recognizeAs(seq([fnKeyword]), {
    modifier: FunctionModifier.Private,
    async: true,
  }),
]);

const functionParameter = map(
  seq([
    identifier,
    typeSpec,
    opt(preceded(between(whitespace0, tag("="), whitespace0), expression)),
  ]),
  ([name, type, defaultValue], start, end) =>
    new FunctionParameter(name, type, defaultValue, start, end)
);

const functionParameters = preceded(
  terminated(tag("("), whitespace0),
  delimitedUntil(
    functionParameter,
    commaDelimiter,
    preceded(whitespace0, tag(")")),
    "<function parameter>"
  )
);

export const functionDeclaration = map(
  seq([
    descriptionComment,
    preceded(whitespace0, functionPrefix),
    identifier,
    preceded(whitespace0, functionParameters),
    preceded(between(whitespace0, tag("->"), whitespace0), typeRef),
    preceded(between(whitespace0, tag("="), whitespace0), expression),
  ]),
  (
    [
      description,
      { modifier, async },
      name,
      parameters,
      returnType,
      expression,
    ],
    start,
    end
  ) =>
    new FunctionDeclaration(
      modifier,
      async,
      name,
      description,
      parameters,
      returnType,
      expression,
      start,
      end
    )
);
