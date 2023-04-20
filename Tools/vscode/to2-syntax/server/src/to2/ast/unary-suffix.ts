import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { Operator } from "./operator";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";

export class UnarySuffix extends Expression {
  constructor(
    public readonly left: Expression,
    public readonly op: Operator,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}
