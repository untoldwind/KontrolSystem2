import { Expression, ModuleItem, Node } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export enum FunctionModifier {
  Public,
  Private,
  Test,
}

export class FunctionParameter implements Node {
  constructor(
    public readonly name: string,
    public readonly type: TO2Type | undefined,
    public readonly defaultValue: Expression | undefined,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    if (this.defaultValue)
      return this.defaultValue.reduceNode(combine, combine(initialValue, this));
    return combine(initialValue, this);
  }
}

export class FunctionDeclaration implements Node, ModuleItem {
  constructor(
    public readonly modifier: FunctionModifier,
    public readonly isAsync: boolean,
    public readonly name: string,
    public readonly description: string,
    public readonly parameters: FunctionParameter[],
    public readonly declaredReturn: TO2Type,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(
      combine,
      this.parameters.reduce(
        (prev, param) => param.reduceNode(combine, prev),
        combine(initialValue, this)
      )
    );
  }
}
