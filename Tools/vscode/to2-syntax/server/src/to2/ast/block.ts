import { BlockItem, Expression, Node } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class Block extends Expression {
  public isComment: boolean = false;

  constructor(
    public readonly items: BlockItem[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return (
      this.items
        .filter((item) => !item.isComment)
        .pop()
        ?.resultType() ?? BUILTIN_UNIT
    );
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }
}
