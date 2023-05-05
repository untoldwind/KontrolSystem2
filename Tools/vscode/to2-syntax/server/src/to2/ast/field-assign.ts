import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { Operator } from "./operator";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class FieldAssign extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly fieldName: WithPosition<string>,
    public readonly op: Operator,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return this.expression.resultType(context);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(
      combine,
      this.target.reduceNode(combine, combine(initialValue, this))
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const targetType = this.target
      .resultType(context)
      .realizedType(context.module);

    const fieldType = targetType.findField(this.fieldName.value)?.realizedType(context.module);
    if (!fieldType) {
      errors.push({
        status:
        targetType === UNKNOWN_TYPE ? "warn" : "error",
        message: `Undefined field ${this.fieldName.value} for type ${
          targetType.name
        }`,
        range: this.fieldName.range,
      });
    } else {
      const targetType = this.target
        .resultType(context)
        .realizedType(context.module);
      this.documentation = [
        this.fieldName.range.with(
          `Field \`${targetType.name}.${this.fieldName.value} : ${fieldType.name}\``
        ),
      ];
      if(fieldType.description)
        this.documentation.push(this.fieldName.range.with(fieldType.description));
    }

    errors.push(...this.target.validateBlock(context));
    errors.push(...this.expression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
    semanticTokens.push(this.fieldName.range.semanticToken("property", "modification"));
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
