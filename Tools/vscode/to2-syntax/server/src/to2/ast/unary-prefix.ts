import { Expression, Node, ValidationError } from ".";
import { Operator } from "./operator";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";
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

  resultType(): TO2Type {
    return BUILTIN_BOOL;
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

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.right.collectSemanticTokens(semanticTokens);
  }
}
