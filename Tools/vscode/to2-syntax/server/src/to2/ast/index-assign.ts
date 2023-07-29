import { Expression, Node, ValidationError } from ".";
import { IndexSpec } from "./index-spec";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { Operator } from "./operator";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class IndexAssign extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly indexSpec: IndexSpec,
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
      this.indexSpec.reduceNode(
        combine,
        this.target.reduceNode(combine, combine(initialValue, this)),
      ),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.target.validateBlock(context));
    errors.push(...this.expression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
  }
}
