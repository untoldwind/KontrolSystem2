import { Expression } from ".";
import { FunctionParameter } from "./function-declaration";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class Lambda extends Expression {
  constructor(
    public readonly parameters: FunctionParameter[],
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
