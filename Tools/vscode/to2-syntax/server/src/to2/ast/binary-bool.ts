import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { Operator } from "./operator";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";

export class BinaryBool extends Expression {
  constructor(
    public readonly left: Expression,
    public readonly op: Operator,
    public readonly right: Expression,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}
