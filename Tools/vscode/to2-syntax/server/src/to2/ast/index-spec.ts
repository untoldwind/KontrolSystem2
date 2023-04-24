import { Expression, Node } from ".";

export class IndexSpec {
  constructor(public readonly start: Expression) {}

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.start.reduceNode(combine, initialValue);
  }
}
