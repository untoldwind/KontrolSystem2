import { Expression, Node } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class ForIn extends Expression {
  constructor(
    public readonly variableName: string,
    public readonly variableType: TO2Type | undefined,
    public readonly sourceExpression: Expression,
    public readonly loopExpression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.loopExpression.reduceNode(
      combine,
      this.sourceExpression.reduceNode(combine, combine(initialValue, this))
    );
  }
}
