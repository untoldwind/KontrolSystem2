import { Expression, Node, ValidationError } from ".";
import { InputPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext } from "./context";
import { BUILTIN_RANGE, TO2Type } from "./to2-type";

export class RangeCreate extends Expression {
  constructor(
    public readonly from: Expression,
    public readonly to: Expression,
    public readonly inclusive: boolean,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_RANGE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.from.reduceNode(
      combine,
      this.to.reduceNode(combine, combine(initialValue, this))
    );
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}
}
