import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class Break extends Expression {
  constructor(start: Position, end: Position) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}

export class Continue extends Expression {
  constructor(start: Position, end: Position) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
