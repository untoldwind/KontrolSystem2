import { Expression, Node, ValidationError } from ".";
import { BUILTIN_BOOL, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";

export class VariableGet extends Expression {
  constructor(
    public readonly namePath: string[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return this.namePath.length == 1 ? context.findVariable(this.namePath[0]) ?? UNKNOWN_TYPE : UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if(this.namePath.length == 1 && !context.findVariable(this.namePath[0])) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath[0]}`,
        start: this.start,
        end: this.end,
      })
    }
    return errors;
  }
}
