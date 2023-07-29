import { Expression, Node, ValidationError } from ".";
import { Operator } from "./operator";
import { BUILTIN_BOOL, BUILTIN_UNIT, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class UnarySuffix extends Expression {
  constructor(
    public readonly left: Expression,
    public readonly op: WithPosition<Operator>,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  resultType(context: BlockContext): TO2Type {
    return this.findOperator(context) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.left.reduceNode(combine, combine(initialValue, this));
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.left.validateBlock(context));

    if (errors.length === 0 && !this.findOperator(context)) {
      const leftType = this.left.resultType(context);
      errors.push({
        status: "error",
        message: `Invalid operator: ${leftType.name} ${this.op.value}  is not definied`,
        range: this.range,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.left.collectSemanticTokens(semanticTokens);
    semanticTokens.push(this.op.range.semanticToken("operator"));
  }

  private findOperator(context: BlockContext): TO2Type | undefined {
    const leftType = this.left.resultType(context).realizedType(context.module);

    return leftType.findSuffixOperator(this.op.value, BUILTIN_UNIT);
  }
}
