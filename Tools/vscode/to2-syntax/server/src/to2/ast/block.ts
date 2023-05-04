import { BlockItem, Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class Block extends Expression {
  private validateionResult?: [TO2Type, ValidationError[]];

  constructor(
    public readonly items: BlockItem[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return this.validate(context)[0];
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return this.validate(context)[1];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const item of this.items) {
      item.collectSemanticTokens(semanticTokens);
    }
  }

  private validate(context: BlockContext): [TO2Type, ValidationError[]] {
    if (this.validateionResult) return this.validateionResult;

    const errors: ValidationError[] = [];
    const blockContext = new BlockContext(context.module, context);
    let resultType: TO2Type = BUILTIN_UNIT;

    for (const item of this.items) {
      errors.push(...item.validateBlock(blockContext));
      if (!item.isComment) resultType = item.resultType(blockContext);
    }

    return [resultType, errors];
  }
}
