import { ModuleItem, Node } from ".";
import { InputPosition } from "../../parser";

export class TO2Module implements Node {
  constructor(
    public readonly name: string,
    public readonly description: string,
    public readonly items: ModuleItem[],
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }
}
