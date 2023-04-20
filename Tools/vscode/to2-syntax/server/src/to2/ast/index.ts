import { Position } from "vscode-languageserver-textdocument";
import { TO2Type } from "./to2-type";

export interface Node {
  start: Position;
  end: Position;
}

export interface BlockItem extends Node {
  isComment: boolean;

  resultType(): TO2Type;
}

export interface ModuleItem {}

export abstract class Expression implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(public readonly start: Position, public readonly end: Position) {}

  public abstract resultType(): TO2Type;
}
