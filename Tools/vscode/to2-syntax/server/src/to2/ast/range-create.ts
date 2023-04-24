import { Expression, Node } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class RangeCreate extends Expression {
  constructor(
    public readonly from: Expression,
    public readonly to: Expression,
    public readonly inclusive: boolean,
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
    return this.from.reduceNode(
      combine,
      this.to.reduceNode(combine, combine(initialValue, this))
    );
  }
}
