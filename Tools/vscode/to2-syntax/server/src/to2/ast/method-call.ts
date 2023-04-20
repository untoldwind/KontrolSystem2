import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class MethodCall extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly methodName: string,
    args: Expression[],
    start: Position,
    end: Position
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
