import { Expression, Node, ValidationError } from ".";
import { IndexSpec } from "./index-spec";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class IndexGet extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly indexSpec: IndexSpec,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return this.findElementType(context) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.indexSpec.reduceNode(
      combine,
      this.target.reduceNode(combine, combine(initialValue, this)),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.target.validateBlock(context));
    if (!this.findElementType(context)) {
      const targetType = this.target.resultType(context);
      errors.push({
        status: targetType === UNKNOWN_TYPE ? "warn" : "error",
        message: `${targetType.name} is not array-like`,
        range: this.target.range,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
  }

  private findElementType(context: BlockContext): TO2Type | undefined {
    const targetType = this.target
      .resultType(context)
      .realizedType(context.module);

    return targetType.supportIndexAccess();
  }
}
