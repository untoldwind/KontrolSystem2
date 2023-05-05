import { Expression, Node, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class FieldGet extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly fieldName: WithPosition<string>,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return this.findField(context) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.target.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.target.validateBlock(context));

    if (errors.length === 0 && !this.findField(context)) {
      errors.push({
        status:
          this.target.resultType(context) === UNKNOWN_TYPE ? "warn" : "error",
        message: `Undefined field ${this.fieldName.value} for type ${
          this.target.resultType(context).name
        }`,
        range: this.fieldName.range,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
    semanticTokens.push(this.range.semanticToken("property"));
  }

  private findField(context: BlockContext): TO2Type | undefined {
    const targetType = this.target
      .resultType(context)
      .realizedType(context.module);

    return targetType.findField(this.fieldName.value);
  }
}
