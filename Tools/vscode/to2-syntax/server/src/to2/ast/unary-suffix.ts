import { Expression, Node } from ".";
import { Operator } from "./operator";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class UnarySuffix extends Expression {
  constructor(
    public readonly left: Expression,
    public readonly op: Operator,
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
    return this.left.reduceNode(combine, combine(initialValue, this));
  }
}
