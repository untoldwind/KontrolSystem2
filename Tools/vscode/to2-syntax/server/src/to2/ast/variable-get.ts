import { Expression, Node, ValidationError } from ".";
import { RealizedType, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class VariableGet extends Expression {
  constructor(
    public readonly namePath: WithPosition<string[]>,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    return context.findVariable(this.namePath.value, typeHint) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateBlock(
    context: BlockContext,
    typeHint?: RealizedType
  ): ValidationError[] {
    const errors: ValidationError[] = [];

    const variableType = context
      .findVariable(this.namePath.value, typeHint)
      ?.realizedType(context.module);
    if (!variableType) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath.value[0]}`,
        range: this.namePath.range,
      });
    } else {
      this.documentation = [
        this.namePath.range.with(
          `Variable \`${this.namePath.value.join("::")} : ${
            variableType.name
          }\``
        ),
      ];
      if (variableType.description)
        this.documentation.push(
          this.namePath.range.with(variableType.description)
        );
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.namePath.range.semanticToken("variable"));
  }
}
