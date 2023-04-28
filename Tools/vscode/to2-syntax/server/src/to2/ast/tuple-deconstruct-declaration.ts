import { BlockItem, Expression, Node, ValidationError } from ".";
import { DeclarationParameterOrPlaceholder } from "./variable-declaration";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";

export class TupleDeconstructDeclaration implements Node, BlockItem {
  constructor(
    public readonly declarations: DeclarationParameterOrPlaceholder[],
    public readonly isConst: boolean,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}
  public resultType(context: BlockContext): TO2Type {
    return this.expression.resultType(context);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }
}
