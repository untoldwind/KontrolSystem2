import { RealizedType, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext, ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export interface Node {
  isError?: boolean;
  start: InputPosition;
  end: InputPosition;

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T;

  collectSemanticTokens(semanticTokens: SemanticToken[]): void;
}

export interface BlockItem extends Node {
  isComment?: boolean;

  resultType(context: BlockContext, typeHint?: RealizedType): TO2Type;

  validateBlock(
    context: BlockContext,
    typeHint?: RealizedType
  ): ValidationError[];
}

export interface ModuleItem extends Node {
  isConstDecl?: boolean;
  isFunctionDecl?: boolean;

  validateModuleFirstPass(context: ModuleContext): ValidationError[];

  validateModuleSecondPass(context: ModuleContext): ValidationError[];
}

export abstract class Expression implements Node, BlockItem {
  constructor(
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

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
  start: InputPosition;
  end: InputPosition;
}
