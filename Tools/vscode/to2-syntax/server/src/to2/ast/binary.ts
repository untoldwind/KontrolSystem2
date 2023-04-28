import { Expression, Node, ValidationError } from ".";
import { Operator } from "./operator";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";

export class Binary extends Expression {
  constructor(
    public readonly left: Expression,
    public readonly op: Operator,
    public readonly right: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(context: BlockContext): TO2Type {
    return this.findOperator(context) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.left.reduceNode(
      combine,
      this.right.reduceNode(combine, combine(initialValue, this))
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];
    
    errors.push(...this.left.validateBlock(context));
    errors.push(...this.right.validateBlock(context));

    if (errors.length === 0 && !this.findOperator(context)) {
      const leftType = this.left.resultType(context);
      const rightType = this.right.resultType(context);
      errors.push({
        status: "error",
        message: `Invalid operator: ${leftType.name} ${this.op} ${rightType.name} is not definied`,
        start: this.start,
        end: this.end,
      });
    }

    return errors;
  }

  private findOperator(context: BlockContext): TO2Type | undefined {
    const leftType = this.left.resultType(context).realizedType(context.module);
    const rightType = this.right
      .resultType(context)
      .realizedType(context.module);

    return (
      leftType.findSuffixOperator(this.op, rightType) ??
      rightType.findPrefixOperator(this.op, leftType)
    );
  }
}
