import { Expression } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { Operator } from "./operator";
import { InputPosition } from "../../parser";

export class FieldAssign extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly fieldName: string,
    public readonly op: Operator,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
