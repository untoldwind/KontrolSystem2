import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class ReturnEmpty extends Expression {
  constructor(start: Position, end: Position) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}

export class ReturnValue extends Expression {
  constructor(
    public readonly returnValue: Expression,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }
  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
