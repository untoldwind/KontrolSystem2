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
import { MethodDeclaration } from "./ast/method-declaration";
import {
  commaDelimiter,
  constKeyword,
  descriptionComment,
  eqDelimiter,
  identifier,
  letKeyword,
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
  seq([identifier, typeSpec, opt(preceded(eqDelimiter, expression))]),
  ([name, type, defaultValue], start, end) =>
    new FunctionParameter(name, type, defaultValue, start, end)
);

export const functionParameters = preceded(
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
    preceded(eqDelimiter, expression),
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

const methodSelfParams = preceded(
  terminated(tag("("), whitespace0),
  alt([
    recognizeAs(selfKeyword, true),
    recognizeAs(preceded(constKeyword, selfKeyword), true),
    recognizeAs(preceded(letKeyword, selfKeyword), false),
  ])
);

const methodParameters = alt([
  recognizeAs(preceded(whitespace0, tag(")")), []),
  preceded(
    commaDelimiter,
    delimitedUntil(
      functionParameter,
      commaDelimiter,
      preceded(whitespace0, tag(")")),
      "<method parameter>"
    )
  ),
]);

export const methodDeclaration = map(
  seq([
    descriptionComment,
    opt(preceded(whitespace0, syncKeyword)),
    preceded(preceded(whitespace0, fnKeyword), identifier),
    preceded(whitespace0, methodSelfParams),
    methodParameters,
    preceded(between(whitespace0, tag("->"), whitespace0), typeRef),
    preceded(eqDelimiter, expression),
  ]),
  (
    [description, sync, name, _, parameters, returnType, expression],
    start,
    end
  ) =>
    new MethodDeclaration(
      sync === undefined,
      name,
      description,
      returnType,
      expression,
      start,
      end
    )
);
