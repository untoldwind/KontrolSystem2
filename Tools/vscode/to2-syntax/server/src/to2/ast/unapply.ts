import { Expression, Node, ValidationError } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { isOptionType } from "./option-type";

export class Unapply extends Expression {
  constructor(
    public readonly pattern: WithPosition<string>,
    public readonly extractNames: string[],
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return BUILTIN_BOOL;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const expressionType = this.expression
      .resultType(context)
      .realizedType(context.module);

    switch (this.pattern.value) {
      case "Some":
        if (this.extractNames.length !== 1) {
          errors.push({
            status: "error",
            message: "Some requires one argument",
            range: this.pattern.range,
          });
        } else if (isOptionType(expressionType)) {
          context.localVariables.set(
            this.extractNames[0],
            expressionType.elementType
          );
        } else {
          errors.push({
            status: "error",
            message: "Expected option type",
            range: this.expression.range,
          });
        }
        break;
      default:
        errors.push({
          status: "error",
          message: `Undefined pattern ${this.pattern}`,
          range: this.pattern.range,
        });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
