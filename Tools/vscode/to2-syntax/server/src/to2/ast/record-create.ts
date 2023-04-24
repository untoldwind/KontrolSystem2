import { Expression, Node } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class RecordCreate extends Expression {
  constructor(
    public readonly declaredResult: TO2Type | undefined,
    public readonly items: [string, Expression][],
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
    return this.items.reduce(
      (prev, item) => item[1].reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }
}
