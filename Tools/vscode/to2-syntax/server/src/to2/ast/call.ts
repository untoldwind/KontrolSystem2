import { Expression, Node, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { isFunctionType } from "./function-type";
import { SemanticToken } from "../../syntax-token";

export class Call extends Expression {
  constructor(
    public readonly namePath: WithPosition<string[]>,
    public readonly args: Expression[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    const variableType =
      this.namePath.value.length == 1
        ? context
            .findVariable(this.namePath.value[0])
            ?.realizedType(context.module)
        : undefined;
    return variableType && isFunctionType(variableType)
      ? variableType.returnType
      : UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.args.reduce(
      (prev, arg) => arg.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const variableType =
      this.namePath.value.length == 1
        ? context
            .findVariable(this.namePath.value[0])
            ?.realizedType(context.module)
        : undefined;
    if (!variableType) {
      errors.push({
        status: "error",
        message: `Undefined variable or function: ${this.namePath.value.join(
          "::"
        )}`,
        start: this.namePath.start,
        end: this.namePath.end,
      });
    } else if (!isFunctionType(variableType)) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath.value.join(
          "::"
        )} is not callable`,
        start: this.namePath.start,
        end: this.namePath.end,
      });
    } else {
      for (const argExpression of this.args) {
        errors.push(...argExpression.validateBlock(context));
      }
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "function",
      start: this.namePath.start,
      length: this.namePath.end.offset - this.namePath.start.offset,
    });
    for (const arg of this.args) {
      arg.collectSemanticTokens(semanticTokens);
    }
  }
}
