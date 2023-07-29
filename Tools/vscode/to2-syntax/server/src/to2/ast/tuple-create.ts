import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { TupleType } from "./tuple-type";

export class TupleCreate extends Expression {
  constructor(
    public readonly items: Expression[],
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return new TupleType(this.items.map((item) => item.resultType(context)));
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    for (const item of this.items) {
      errors.push(...item.validateBlock(context));
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const item of this.items) {
      item.collectSemanticTokens(semanticTokens);
    }
  }
}
