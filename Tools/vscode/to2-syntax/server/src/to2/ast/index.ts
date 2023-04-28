import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext, ModuleContext } from "./context";

export interface Node {
  isError?: boolean;
  start: InputPosition;
  end: InputPosition;

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T;
}

export interface BlockItem extends Node {
  isComment?: boolean;

  resultType(context: BlockContext): TO2Type;

  validateBlock(context: BlockContext): ValidationError[];
}

export interface ModuleItem extends Node {
  isConstDecl?: boolean;
  isFunctionDecl?: boolean;
  validateModule(context: ModuleContext): ValidationError[];
}

export abstract class Expression implements Node, BlockItem {
  constructor(
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public abstract resultType(context: BlockContext): TO2Type;

  public abstract reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T;

  public abstract validateBlock(context: BlockContext): ValidationError[];
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
