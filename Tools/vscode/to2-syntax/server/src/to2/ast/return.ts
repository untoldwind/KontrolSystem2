import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class ReturnEmpty extends Expression {
  constructor(
    public readonly returnKeyword: WithPosition<"return">,
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
    return combine(initialValue, this);
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.returnKeyword.range.semanticToken("keyword"));
  }
}

export class ReturnValue extends Expression {
  constructor(
    public readonly returnKeyword: WithPosition<"return">,
    public readonly returnValue: Expression,
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
    return this.returnValue.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.returnValue.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.returnKeyword.range.semanticToken("keyword"));
    this.returnValue.collectSemanticTokens(semanticTokens);
  }
}
