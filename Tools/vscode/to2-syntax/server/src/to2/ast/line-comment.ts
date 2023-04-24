import { BlockItem, Node } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class LineComment implements Node, BlockItem {
  public isComment: boolean = true;

  constructor(
    public readonly comment: string,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}
