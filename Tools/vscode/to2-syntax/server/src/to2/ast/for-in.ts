import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class ForIn extends Expression {
  constructor(
    public readonly forKeyword: WithPosition<"for">,
    public readonly variableName: WithPosition<string>,
    public readonly variableType: TO2Type | undefined,
    public readonly inKeyword: WithPosition<"in">,
    public readonly sourceExpression: Expression,
    public readonly loopExpression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.loopExpression.reduceNode(
      combine,
      this.sourceExpression.reduceNode(combine, combine(initialValue, this))
    );
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.sourceExpression.validateBlock(context));
    errors.push(...this.loopExpression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "keyword",
      start: this.forKeyword.start,
      length: this.forKeyword.end.offset - this.forKeyword.start.offset,
    });
    semanticTokens.push({
      type: "variable",
      modifiers: ["definition"],
      start: this.variableName.start,
      length: this.variableName.end.offset - this.variableName.start.offset,
    });
    semanticTokens.push({
      type: "keyword",
      start: this.inKeyword.start,
      length: this.inKeyword.end.offset - this.inKeyword.start.offset,
    });
    this.sourceExpression.collectSemanticTokens(semanticTokens);
    this.loopExpression.collectSemanticTokens(semanticTokens);
  }
}
