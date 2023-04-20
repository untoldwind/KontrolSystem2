import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";

export class RangeCreate implements Expression {
  constructor(
    public readonly from: Expression,
    public readonly to: Expression,
    public readonly inclusive: boolean,
    public readonly start: Position,
    public readonly end: Position
  ) {}

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}
