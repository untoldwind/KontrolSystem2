import { Position } from "vscode-languageserver-textdocument";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export interface Node {
  start: InputPosition;
  end: InputPosition;
}

export interface BlockItem extends Node {
  isComment: boolean;

  resultType(): TO2Type;
}

export interface ModuleItem {}

export abstract class Expression implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public abstract resultType(): TO2Type;
}
