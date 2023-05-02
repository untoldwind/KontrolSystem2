import { BlockItem, Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { ArrayType } from "./array-type";

export class ArrayCreate extends Expression {
  constructor(
    public readonly elementType: TO2Type | undefined,
    public readonly elements: Expression[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return new ArrayType(this.determineElementType(context));
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.elements.reduce(
      (prev, element) => element.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    for (const element of this.elements) {
      errors.push(...element.validateBlock(context));
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const element of this.elements) {
      element.collectSemanticTokens(semanticTokens);
    }
  }

  private determineElementType(context: BlockContext) {
    if (this.elementType) return this.elementType;

    for (const element of this.elements) {
      const elementType = element.resultType(context);
      if (elementType !== UNKNOWN_TYPE) return elementType;
    }
    return UNKNOWN_TYPE;
  }
}
