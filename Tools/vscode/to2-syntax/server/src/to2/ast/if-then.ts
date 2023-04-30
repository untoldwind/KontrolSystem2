import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class IfThen extends Expression {
  constructor(
    public readonly ifKeyword: WithPosition<"if">,
    public readonly condition: Expression,
    public readonly thenExpression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }
  public resultType(context: BlockContext): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.thenExpression.reduceNode(
      combine,
      this.condition.reduceNode(combine, combine(initialValue, this))
    );
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.condition.validateBlock(context));
    errors.push(...this.thenExpression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "keyword",
      start: this.ifKeyword.start,
      length: this.ifKeyword.end.offset - this.ifKeyword.start.offset,
    });
    this.condition.collectSemanticTokens(semanticTokens);
    this.thenExpression.collectSemanticTokens(semanticTokens);
  }
}

export class IfThenElse extends Expression {
  constructor(
    public readonly ifKeyword: WithPosition<"if">,
    public readonly condition: Expression,
    public readonly thenExpression: Expression,
    public readonly elseExpression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }
  public resultType(context: BlockContext): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.elseExpression.reduceNode(
      combine,
      this.thenExpression.reduceNode(
        combine,
        this.condition.reduceNode(combine, combine(initialValue, this))
      )
    );
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.condition.validateBlock(context));
    errors.push(...this.thenExpression.validateBlock(context));
    errors.push(...this.elseExpression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "keyword",
      start: this.ifKeyword.start,
      length: this.ifKeyword.end.offset - this.ifKeyword.start.offset,
    });
    this.condition.collectSemanticTokens(semanticTokens);
    this.thenExpression.collectSemanticTokens(semanticTokens);
    this.elseExpression.collectSemanticTokens(semanticTokens);
  }
}
