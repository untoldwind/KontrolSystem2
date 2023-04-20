import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class IfThen extends Expression {
  constructor(
    public readonly condition: Expression,
    public readonly thenExpression: Expression,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }
  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}

export class IfThenElse extends Expression {
  constructor(
    public readonly condition: Expression,
    public readonly thenExpression: Expression,
    public readonly elseExpression: Expression,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }
  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
