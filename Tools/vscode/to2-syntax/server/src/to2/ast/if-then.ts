import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { OptionType } from "./option-type";

export class IfThen extends Expression {
  constructor(
    private readonly ifKeyword: WithPosition<"if">,
    public readonly condition: Expression,
    public readonly thenExpression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    const thenContext = new BlockContext(context.module, context);

    this.condition.validateBlock(thenContext);

    return new OptionType(this.thenExpression.resultType(thenContext));
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.thenExpression.reduceNode(
      combine,
      this.condition.reduceNode(combine, combine(initialValue, this)),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const thenContext = new BlockContext(context.module, context);

    errors.push(...this.condition.validateBlock(thenContext));
    errors.push(...this.thenExpression.validateBlock(thenContext));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.ifKeyword.range.semanticToken("keyword"));
    this.condition.collectSemanticTokens(semanticTokens);
    this.thenExpression.collectSemanticTokens(semanticTokens);
  }
}

export class IfThenElse extends Expression {
  constructor(
    private readonly ifKeyword: WithPosition<"if">,
    public readonly condition: Expression,
    public readonly thenExpression: Expression,
    public readonly elseExpression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }
  public resultType(context: BlockContext): TO2Type {
    const thenContext = new BlockContext(context.module, context);

    this.condition.validateBlock(thenContext);

    const thenType = this.thenExpression.resultType(thenContext);
    return thenType === UNKNOWN_TYPE
      ? this.elseExpression.resultType(context)
      : thenType;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.elseExpression.reduceNode(
      combine,
      this.thenExpression.reduceNode(
        combine,
        this.condition.reduceNode(combine, combine(initialValue, this)),
      ),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const thenContext = new BlockContext(context.module, context);

    errors.push(...this.condition.validateBlock(thenContext));
    errors.push(...this.thenExpression.validateBlock(thenContext));
    errors.push(...this.elseExpression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.ifKeyword.range.semanticToken("keyword"));
    this.condition.collectSemanticTokens(semanticTokens);
    this.thenExpression.collectSemanticTokens(semanticTokens);
    this.elseExpression.collectSemanticTokens(semanticTokens);
  }
}
