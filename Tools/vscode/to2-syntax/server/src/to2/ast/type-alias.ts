import { TO2Type } from "./to2-type";
import { ModuleItem, Node } from ".";
import { InputPosition } from "../../parser";

export class TypeAlias implements Node, ModuleItem {
  constructor(
    public readonly exported: boolean,
    public readonly name: string,
    public readonly description: string,
    public readonly type: TO2Type,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}
