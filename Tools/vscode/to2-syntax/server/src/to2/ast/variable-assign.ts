import { Expression } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
import { Operator } from "./operator";
import { InputPosition } from "../../parser";

export class VariableAssign extends Expression {
  constructor(
    public readonly name: string,
    public readonly op: Operator,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}
