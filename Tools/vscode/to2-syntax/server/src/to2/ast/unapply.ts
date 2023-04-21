import { Expression } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class Unapply extends Expression {
  constructor(
    public readonly pattern: string,
    public readonly extractNames: string[],
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
