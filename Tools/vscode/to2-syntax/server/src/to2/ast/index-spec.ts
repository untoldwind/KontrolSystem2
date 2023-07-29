import { Expression, Node, ValidationError } from ".";

export class IndexSpec {
  constructor(public readonly start: Expression) {}

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.start.reduceNode(combine, initialValue);
  }
  public validate(): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }
}
