import { Expression, Node, ValidationError } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
import { Operator } from "./operator";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";

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

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }
}
