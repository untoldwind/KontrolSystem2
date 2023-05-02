import { Expression, Node, ValidationError } from ".";
import { FunctionParameter } from "./function-declaration";
import { BUILTIN_UNIT, RealizedType, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class Lambda extends Expression {
  constructor(
    public readonly parameters: FunctionParameter[],
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(
      combine,
      this.parameters.reduce(
        (prev, param) => param.reduceNode(combine, prev),
        combine(initialValue, this)
      )
    );
  }

  public validateBlock(
    context: BlockContext,
    typeHint?: RealizedType
  ): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.expression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
