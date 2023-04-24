import { BlockItem, Expression, Node } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class DeclarationParameter {
  constructor(
    public readonly target: string | undefined,
    public readonly source: string | undefined,
    public readonly type: TO2Type | undefined
  ) {}
}

export class VariableDeclaration implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(
    public readonly declaration: DeclarationParameter,
    public readonly isConst: boolean,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  resultType(): TO2Type {
    return this.expression.resultType();
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }
}
