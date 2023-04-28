import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";

export class Call extends Expression {
  constructor(
    public readonly namePath: WithPosition<string[]>,
    public readonly args: Expression[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.args.reduce(
      (prev, arg) => arg.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    for (const argExpression of this.args) {
      errors.push(...argExpression.validateBlock(context));
    }

    if (
      this.namePath.value.length == 1 &&
      !context.findVariable(this.namePath.value[0])
    ) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath.value[0]}`,
        start: this.start,
        end: this.end,
      });
    }
    
    return errors;
  }
}
