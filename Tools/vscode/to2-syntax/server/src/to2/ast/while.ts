import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class While extends Expression {
  constructor(
    public readonly condition: Expression,
    public readonly loopExpression: Expression,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
