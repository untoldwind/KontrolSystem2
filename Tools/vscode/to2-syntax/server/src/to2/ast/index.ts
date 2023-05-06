import { RealizedType, TO2Type } from "./to2-type";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { BlockContext, ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { CompletionItem } from "vscode-languageserver";
import { Position } from "vscode-languageserver-textdocument";
import { Registry } from "./registry";

export interface Node {
  readonly isComment?: boolean;
  readonly isError?: boolean;
  readonly documentation?: WithPosition<string>[];
  readonly range: InputRange;

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T;

  collectSemanticTokens(semanticTokens: SemanticToken[]): void;

  completionsAt?(position: Position): CompletionItem[];
}

export interface BlockItem extends Node {
  resultType(context: BlockContext, typeHint?: RealizedType): TO2Type;

  validateBlock(
    context: BlockContext,
    typeHint?: RealizedType
  ): ValidationError[];
}

export interface ModuleItem extends Node {
  isConstDecl?: true;
  isFunctionDecl?: true;
  isTypeDecl?: true;

  validateModuleFirstPass(context: ModuleContext): ValidationError[];

  validateModuleSecondPass(context: ModuleContext): ValidationError[];
}

export interface TypeDeclaration extends ModuleItem {
  isTypeDecl: true;
  name: string;
  type: TO2Type;
}

export function isTypeDeclaration(item: ModuleItem): item is TypeDeclaration {
  return !!item.isTypeDecl;
}

export abstract class Expression implements Node, BlockItem {
  public documentation?: WithPosition<string>[];
  public readonly range: InputRange;

  constructor(start: InputPosition, end: InputPosition) {
    this.range = new InputRange(start, end);
  }

  public abstract resultType(
    context: BlockContext,
    typeHint?: RealizedType
  ): TO2Type;

  public abstract reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T;

  public abstract validateBlock(
    context: BlockContext,
    typeHint?: RealizedType
  ): ValidationError[];

  public abstract collectSemanticTokens(semanticTokens: SemanticToken[]): void;
}

export interface VariableContainer {
  findVariable(name: string): TO2Type | undefined;
}

export interface ValidationError {
  status: "warn" | "error";
  message: string;
  range: InputRange;
}
