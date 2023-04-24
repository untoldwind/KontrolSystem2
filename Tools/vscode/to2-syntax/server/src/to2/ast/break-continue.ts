import { Expression, Node } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class Break extends Expression {
  constructor(start: InputPosition, end: InputPosition) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}

export class Continue extends Expression {
  constructor(start: InputPosition, end: InputPosition) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}
