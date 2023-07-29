import { Expression, Node, ValidationError } from ".";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext } from "./context";
import { DefinitionRef } from "./definition-ref";
import { Operator } from "./operator";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";

export class FieldAssign extends Expression {
  public reference?: { sourceRange: InputRange; definition: DefinitionRef };

  constructor(
    public readonly target: Expression,
    public readonly fieldName: WithPosition<string>,
    public readonly op: Operator,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return this.expression.resultType(context);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.expression.reduceNode(
      combine,
      this.target.reduceNode(combine, combine(initialValue, this)),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const targetType = this.target
      .resultType(context)
      .realizedType(context.module);

    const { definition, value: fieldType } =
      targetType.findField(this.fieldName.value) ?? {};
    if (definition) {
      this.reference = {
        sourceRange: this.fieldName.range,
        definition,
      };
    }
    const fieldRealized = fieldType?.realizedType(context.module);
    if (!fieldRealized) {
      errors.push({
        status: targetType === UNKNOWN_TYPE ? "warn" : "error",
        message: `Undefined field ${this.fieldName.value} for type ${targetType.name}`,
        range: this.fieldName.range,
      });
    } else {
      const targetType = this.target
        .resultType(context)
        .realizedType(context.module);
      this.documentation = [
        this.fieldName.range.with(
          `Field \`${targetType.name}.${this.fieldName.value} : ${fieldRealized.name}\``,
        ),
      ];
      if (fieldRealized.description)
        this.documentation.push(
          this.fieldName.range.with(fieldRealized.description),
        );
    }

    errors.push(...this.target.validateBlock(context));
    errors.push(...this.expression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
    semanticTokens.push(
      this.fieldName.range.semanticToken("property", "modification"),
    );
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
