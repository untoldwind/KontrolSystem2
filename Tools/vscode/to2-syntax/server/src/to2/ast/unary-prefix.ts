import { Expression, Node, ValidationError } from ".";
import { Operator } from "./operator";
import { BUILTIN_UNIT, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class UnaryPrefix extends Expression {
  constructor(
    public readonly op: WithPosition<Operator>,
    public readonly right: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(context: BlockContext): TO2Type {
    return this.findOperator(context) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.right.reduceNode(combine, combine(initialValue, this));
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.right.validateBlock(context));

    if (errors.length === 0 && !this.findOperator(context)) {
      const rightType = this.right.resultType(context);
      errors.push({
        status: "error",
        message: `Invalid operator: ${this.op.value} ${rightType.name} is not definied`,
        range: this.range,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.op.range.semanticToken("operator"));
    this.right.collectSemanticTokens(semanticTokens);
  }

  private findOperator(context: BlockContext): TO2Type | undefined {
    const rightType = this.right
      .resultType(context)
      .realizedType(context.module);

    return rightType.findPrefixOperator(this.op.value, BUILTIN_UNIT);
  }
}
