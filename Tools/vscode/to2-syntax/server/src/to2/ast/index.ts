import { Position } from "vscode-languageserver-textdocument";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

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
  isComment: boolean;

  resultType(): TO2Type;
}

export interface ModuleItem extends Node {}

export abstract class Expression implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public abstract resultType(): TO2Type;

  public abstract reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T;
}
