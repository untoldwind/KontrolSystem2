import { BlockItem, Expression, Node } from ".";
import { DeclarationParameterOrPlaceholder } from "./variable-declaration";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class TupleDeconstructDeclaration implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(
    public readonly declarations: DeclarationParameterOrPlaceholder[],
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
