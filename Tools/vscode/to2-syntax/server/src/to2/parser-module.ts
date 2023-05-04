import { Parser, ParserFailure, ParserSuccess } from "../parser";
import { alt } from "../parser/branch";
import {
  either,
  map,
  opt,
  recognizeAs,
  withPosition,
} from "../parser/combinator";
import {
  NL,
  eof,
  spacing1,
  tag,
  whitespace0,
  whitespace1,
} from "../parser/complete";
import { delimited0, delimited1, delimitedUntil } from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import { ModuleItem } from "./ast";
import { ConstDeclaration } from "./ast/const-declaration";
import { ErrorNode } from "./ast/error-node";
import { ImplDeclaration } from "./ast/impl-declaration";
import { StructDeclaration, StructField } from "./ast/struct-declaration";
import { TO2ModuleNode } from "./ast/to2-module";
import { TypeAlias } from "./ast/type-alias";
import { UseDeclaration } from "./ast/use-declaration";
import {
  commaDelimiter,
  descriptionComment,
  eqDelimiter,
  identifier,
  identifierPath,
  lineComment,
  pubKeyword,
  typeRef,
  typeSpec,
} from "./parser-common";
import { expression } from "./parser-expression";
import {
  functionDeclaration,
  functionParameters,
  methodDeclaration,
} from "./parser-functions";

const useKeyword = terminated(withPosition(tag("use")), spacing1);

const typeKeyword = terminated(tag("type"), spacing1);

const structKeyword = terminated(withPosition(tag("struct")), spacing1);

const implKeyword = terminated(tag("impl"), spacing1);

const fromKeyword = between(spacing1, withPosition(tag("from")), spacing1);

const asKeyword = between(spacing1, tag("as"), spacing1);

const constKeyword = terminated(tag("const"), spacing1);

const useNames = alt(
  recognizeAs(tag("*"), undefined),
  between(
    terminated(tag("{"), whitespace0),
    delimited1(withPosition(identifier), commaDelimiter, "<import name>"),
    preceded(whitespace0, tag("}"))
  )
);

const useNamesDeclaration = map(
  seq(useKeyword, useNames, fromKeyword, withPosition(identifierPath)),
  ([useKeyword, names, fromKeyword, namePath], start, end) =>
    new UseDeclaration(
      useKeyword,
      names,
      undefined,
      undefined,
      fromKeyword,
      namePath,
      start,
      end
    )
);

const useAliasDeclaration = map(
  seq(
    useKeyword,
    withPosition(identifierPath),
    withPosition(asKeyword),
    withPosition(identifier)
  ),
  ([useKeyword, namePath, asKeyword, alias], start, end) =>
    new UseDeclaration(
      useKeyword,
      undefined,
      alias,
      asKeyword,
      undefined,
      namePath,
      start,
      end
    )
);

const typeAlias = map(
  seq(
    descriptionComment,
    preceded(whitespace0, opt(pubKeyword)),
    preceded(typeKeyword, withPosition(identifier)),
    preceded(eqDelimiter, typeRef)
  ),
  ([description, pub, name, type], start, end) =>
    new TypeAlias(pub !== undefined, name, description, type, start, end)
);

const structField = map(
  seq(
    descriptionComment,
    preceded(whitespace0, identifier),
    typeSpec,
    preceded(eqDelimiter, expression)
  ),
  ([description, name, type, initializer], start, end) =>
    new StructField(name, type, description, initializer, start, end)
);

const structDeclaration = map(
  seq(
    descriptionComment,
    preceded(whitespace0, opt(pubKeyword)),
    structKeyword,
    withPosition(identifier),
    opt(functionParameters),
    between(
      preceded(whitespace0, tag("{")),
      delimited0(either(lineComment, structField), whitespace1, "fields"),
      preceded(whitespace0, tag("}"))
    )
  ),
  (
    [
      description,
      pubKeyword,
      structKeyword,
      name,
      constructorParameters,
      fields,
    ],
    start,
    end
  ) =>
    new StructDeclaration(
      pubKeyword,
      structKeyword,
      name,
      description,
      constructorParameters ?? [],
      fields,
      start,
      end
    )
);

const implDeclaration = map(
  seq(
    withPosition(implKeyword),
    withPosition(identifier),
    between(
      preceded(whitespace0, tag("{")),
      delimited0(
        either(lineComment, methodDeclaration),
        whitespace1,
        "methods"
      ),
      preceded(whitespace0, tag("}"))
    )
  ),
  ([implKeyword, name, methods], start, end) =>
    new ImplDeclaration(implKeyword, name, methods, start, end)
);

const constDeclaration = map(
  seq(
    descriptionComment,
    opt(pubKeyword),
    preceded(constKeyword, withPosition(identifier)),
    typeSpec,
    preceded(eqDelimiter, expression)
  ),
  ([description, pub, name, type, expression], start, end) =>
    new ConstDeclaration(
      pub !== undefined,
      name,
      description,
      type,
      expression,
      start,
      end
    )
);

const moduleItem: Parser<ModuleItem> = alt(
  useNamesDeclaration,
  useAliasDeclaration,
  functionDeclaration,
  typeAlias,
  structDeclaration,
  implDeclaration,
  constDeclaration,
  lineComment
);

const moduleItems = preceded(
  whitespace0,
  delimitedUntil(
    moduleItem,
    whitespace1,
    eof,
    "<module item>",
    recoverModuleItem
  )
);

export function module(moduleName: string): Parser<TO2ModuleNode> {
  return map(
    seq(preceded(whitespace0, descriptionComment), moduleItems),
    ([description, items], start, end) =>
      new TO2ModuleNode(moduleName, description, items, start, end)
  );
}

function recoverModuleItem(
  failure: ParserFailure<ModuleItem | string>
): ParserSuccess<ModuleItem> {
  const remaining = failure.remaining;
  const nextNL = remaining.findNext((ch) => ch === NL);
  const recoverAt = remaining.advance(
    nextNL >= 0 ? nextNL : remaining.available
  );

  return new ParserSuccess(
    recoverAt,
    new ErrorNode(failure.expected, remaining.position, recoverAt.position)
  );
}
