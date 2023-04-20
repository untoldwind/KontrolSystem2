import { Position } from "vscode-languageserver-textdocument";
import { BlockItem, Node } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class LineComment implements Node, BlockItem {
  public isComment: boolean = true;

  constructor(
    public readonly comment: string,
    public readonly start: Position,
    public readonly end: Position
  ) {}

  resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
