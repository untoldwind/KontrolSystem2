import { BlockItem, Expression, Node, ValidationError } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export interface TupleTarget {
  target: WithPosition<string>;
  source: string;
}

export class TupleDeconstructAssign implements Node, BlockItem {
  public readonly range: InputRange;

  constructor(
    public readonly targets: TupleTarget[],
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    this.range = new InputRange(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
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

    for (const target of this.targets) {
      if (!context.findVariable([target.target.value])) {
        errors.push({
          status: "error",
          message: `Undefined variable: ${target.target.value}`,
          range: target.target.range,
        });
      }
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const target of this.targets) {
      if (target.target.value !== "") {
        semanticTokens.push(
          target.target.range.semanticToken("variable", "modification")
        );
      }
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
