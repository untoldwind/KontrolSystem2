import { Expression, ModuleItem, Node } from ".";
import { InputPosition } from "../../parser";
import { TO2Type } from "./to2-type";

export class ConstDeclaration implements Node, ModuleItem {
  constructor(
    public readonly isPublic: boolean,
    public readonly name: string,
    public readonly description: string,
    public readonly type: TO2Type,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }
}
