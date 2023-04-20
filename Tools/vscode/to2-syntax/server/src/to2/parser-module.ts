import { Parser } from "../parser";
import { alt } from "../parser/branch";
import { map, recognize, recognizeAs } from "../parser/combinator";
import {
  eof,
  spacing1,
  tag,
  whitespace0,
  whitespace1,
} from "../parser/complete";
import { delimited1, delimitedUntil } from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import { ModuleItem } from "./ast";
import { TO2Module } from "./ast/to2-module";
import { UseDeclaration } from "./ast/use-declaration";
import {
  commaDelimiter,
  descriptionComment,
  identifier,
  identifierPath,
} from "./parser-common";
import { functionDeclaration } from "./parser-functions";

const useKeyword = terminated(tag("use"), spacing1);

const typeKeyword = terminated(tag("type"), spacing1);

const structKeyword = terminated(tag("struct"), spacing1);

const implKeyword = terminated(tag("impl"), spacing1);

const fromKeyword = between(spacing1, tag("from"), spacing1);

const asKeyword = between(spacing1, tag("as"), spacing1);

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

const moduleItem = alt<ModuleItem>([
  useNamesDeclaration,
  useAliasDeclaration,
  functionDeclaration,
]);

const moduleItems = delimitedUntil(
  moduleItem,
  whitespace1,
  eof,
  "<module item>"
);

export function module(moduleName: string): Parser<TO2Module> {
  return map(
    seq([preceded(whitespace0, descriptionComment), moduleItems]),
    ([description, items]) => new TO2Module(moduleName, description, items)
  );
}
