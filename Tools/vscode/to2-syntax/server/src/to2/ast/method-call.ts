import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class MethodCall extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly methodName: WithPosition<string>,
    public readonly args: Expression[],
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
    return this.args.reduce(
      (prev, arg) => arg.reduceNode(combine, prev),
      this.target.reduceNode(combine, combine(initialValue, this))
    );
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    for (const argExpression of this.args) {
      errors.push(...argExpression.validateBlock(context));
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
    semanticTokens.push({
      type: "method",
      start: this.methodName.start,
      length: this.methodName.end.offset - this.methodName.start.offset,
    });
    for (const arg of this.args) {
      arg.collectSemanticTokens(semanticTokens);
    }
  }
}
