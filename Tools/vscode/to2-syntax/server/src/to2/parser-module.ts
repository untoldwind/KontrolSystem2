import { Parser, ParserFailure, ParserSuccess } from "../parser";
import { alt } from "../parser/branch";
import { either, map, opt, recognizeAs } from "../parser/combinator";
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
import { TO2Module } from "./ast/to2-module";
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

const useKeyword = terminated(tag("use"), spacing1);

const typeKeyword = terminated(tag("type"), spacing1);

const structKeyword = terminated(tag("struct"), spacing1);

const implKeyword = terminated(tag("impl"), spacing1);

const fromKeyword = between(spacing1, tag("from"), spacing1);

const asKeyword = between(spacing1, tag("as"), spacing1);

const constKeyword = terminated(tag("const"), spacing1);

const useNames = alt([
  recognizeAs(tag("*"), undefined),
  between(
    terminated(tag("{"), whitespace0),
    delimited1(identifier, commaDelimiter, "<import name>"),
    preceded(whitespace0, tag("}"))
  ),
]);

const useNamesDeclaration = map(
  seq([preceded(useKeyword, useNames), preceded(fromKeyword, identifierPath)]),
  ([names, namePath], start, end) =>
    new UseDeclaration(names, undefined, namePath, start, end)
);

const useAliasDeclaration = map(
  seq([preceded(useKeyword, identifierPath), preceded(asKeyword, identifier)]),
  ([namePath, alias], start, end) =>
    new UseDeclaration(undefined, alias, namePath, start, end)
);

const typeAlias = map(
  seq([
    descriptionComment,
    preceded(whitespace0, opt(pubKeyword)),
    preceded(typeKeyword, identifier),
    preceded(eqDelimiter, typeRef),
  ]),
  ([description, pub, name, type], start, end) =>
    new TypeAlias(pub !== undefined, name, description, type, start, end)
);

const structField = map(
  seq([
    descriptionComment,
    preceded(whitespace0, identifier),
    typeSpec,
    preceded(eqDelimiter, expression),
  ]),
  ([description, name, type, initializer], start, end) =>
    new StructField(name, type, description, initializer, start, end)
);

const structDeclaration = map(
  seq([
    descriptionComment,
    preceded(whitespace0, opt(pubKeyword)),
    preceded(structKeyword, identifier),
    opt(functionParameters),
    between(
      preceded(whitespace0, tag("{")),
      delimited0(either(lineComment, structField), whitespace1, "fields"),
      preceded(whitespace0, tag("}"))
    ),
  ]),
  ([description, pub, name, constructorParameters, fields], start, end) =>
    new StructDeclaration(
      pub !== undefined,
      name,
      description,
      constructorParameters ?? [],
      fields,
      start,
      end
    )
);

const implDeclaration = map(
  seq([
    preceded(implKeyword, identifier),
    between(
      preceded(whitespace0, tag("{")),
      delimited0(
        either(lineComment, methodDeclaration),
        whitespace1,
        "methods"
      ),
      preceded(whitespace0, tag("}"))
    ),
  ]),
  ([name, methods], start, end) =>
    new ImplDeclaration(name, methods, start, end)
);

const constDeclaration = map(
  seq([
    descriptionComment,
    opt(pubKeyword),
    preceded(constKeyword, identifier),
    typeSpec,
    preceded(eqDelimiter, expression),
  ]),
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

const moduleItem = alt<ModuleItem>([
  useNamesDeclaration,
  useAliasDeclaration,
  functionDeclaration,
  typeAlias,
  structDeclaration,
  implDeclaration,
  constDeclaration,
  lineComment,
]);

const moduleItems = delimitedUntil(
  moduleItem,
  whitespace1,
  eof,
  "<module item>",
  recoverModuleItem
);

export function module(moduleName: string): Parser<TO2Module> {
  return map(
    seq([preceded(whitespace0, descriptionComment), moduleItems]),
    ([description, items], start, end) =>
      new TO2Module(moduleName, description, items, start, end)
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
