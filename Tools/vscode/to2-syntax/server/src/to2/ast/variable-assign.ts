import { Expression, Node, ValidationError } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
import { Operator } from "./operator";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class VariableAssign extends Expression {
  constructor(
    public readonly name: WithPosition<string>,
    public readonly op: Operator,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(context: BlockContext): TO2Type {
    return this.expression.resultType(context);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.expression.validateBlock(context));

    if (!context.findVariable(this.name.value)) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.name.value}`,
        start: this.name.start,
        end: this.name.end,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "variable",
      start: this.name.start,
      length: this.name.end.offset - this.name.start.offset,
    });
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
