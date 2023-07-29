import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { DeclarationParameterOrPlaceholder } from "./variable-declaration";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class ForInDeconstruct extends Expression {
  constructor(
    public readonly forKeyword: WithPosition<"for">,
    public readonly declarations: DeclarationParameterOrPlaceholder[],
    public readonly inKeyword: WithPosition<"in">,
    public readonly sourceExpression: Expression,
    public readonly loopExpression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.loopExpression.reduceNode(
      combine,
      this.sourceExpression.reduceNode(combine, combine(initialValue, this)),
    );
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.sourceExpression.validateBlock(context));
    errors.push(...this.loopExpression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.forKeyword.range.semanticToken("keyword"));
    semanticTokens.push(this.inKeyword.range.semanticToken("keyword"));
    this.sourceExpression.collectSemanticTokens(semanticTokens);
    this.loopExpression.collectSemanticTokens(semanticTokens);
  }
}
