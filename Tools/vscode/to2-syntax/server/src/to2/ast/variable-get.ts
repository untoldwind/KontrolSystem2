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

    if (!context.findVariable(this.namePath.value, typeHint)) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath.value[0]}`,
        start: this.namePath.start,
        end: this.namePath.end,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "variable",
      start: this.start,
      length: this.end.offset - this.start.offset,
    });
  }
}
