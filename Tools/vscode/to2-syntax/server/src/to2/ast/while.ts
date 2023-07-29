import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class While extends Expression {
  constructor(
    private readonly whileKeyword: WithPosition<string>,
    public readonly condition: Expression,
    public readonly loopExpression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.loopExpression.reduceNode(
      combine,
      this.condition.reduceNode(combine, combine(initialValue, this)),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.condition.validateBlock(context));
    errors.push(...this.loopExpression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.whileKeyword.range.semanticToken("keyword"));

    this.condition.collectSemanticTokens(semanticTokens);
    this.loopExpression.collectSemanticTokens(semanticTokens);
  }
}
