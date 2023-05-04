import { uinteger } from "vscode-languageserver";
import { InputPosition } from "./parser";

export const SEMANTIC_TOKEN_TYPES = [
  "keyword",
  "operator",
  "variable",
  "parameter",
  "string",
  "number",
  "comment",
  "property",
  "type",
  "function",
  "method",
  "namespace",
  "struct",
] as const;

export type SemanticTokenType = (typeof SEMANTIC_TOKEN_TYPES)[number];

const semanticTokenMap: Record<SemanticTokenType, number> =
  SEMANTIC_TOKEN_TYPES.reduce((result, token, idx) => {
    result[token] = idx;
    return result;
  }, {} as Record<SemanticTokenType, number>);

export const SEMANTIC_TOKEN_MODIFIERS = [
  "declaration",
  "definition",
  "readonly",
  "async",
  "documentation",
  "modification",
  "defaultLibrary",
] as const;

export type SemanticTokenModifier = (typeof SEMANTIC_TOKEN_MODIFIERS)[number];

const semanticTokenModifierMap: Record<SemanticTokenModifier, number> =
  SEMANTIC_TOKEN_MODIFIERS.reduce((result, token, idx) => {
    result[token] = 1 << idx;
    return result;
  }, {} as Record<SemanticTokenModifier, number>);

export interface SemanticToken {
  type: SemanticTokenType;
  modifiers?: SemanticTokenModifier[];
  start: InputPosition;
  length: number;
}

export function convertSemanticTokens(tokens: SemanticToken[]): uinteger[] {
  tokens.sort((a, b) => a.start.offset - b.start.offset);
  const result: uinteger[] = new Array(5 * tokens.length);
  let line = 0;
  let column = 0;

  for (let i = 0, j = 0; i < result.length; i += 5, j++) {
    const token = tokens[j];
    result[i] = token.start.line - line;
    result[i + 1] =
      result[i] == 0 ? token.start.character - column : token.start.character;
    result[i + 2] = token.length;
    result[i + 3] = semanticTokenMap[token.type];
    result[i + 4] = token.modifiers
      ? token.modifiers.reduce(
          (acc, modifer) => (acc |= semanticTokenModifierMap[modifer]),
          0
        )
      : 0;

    line = token.start.line;
    column = token.start.character;
  }

  return result;
}
