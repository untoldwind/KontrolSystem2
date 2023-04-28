import { BlockItem, Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";

export class Block extends Expression {
  constructor(
    public readonly items: BlockItem[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return (
      this.items
        .filter((item) => !item.isComment)
        .pop()
        ?.resultType(context) ?? BUILTIN_UNIT
    );
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];
    const blockContext = new BlockContext(context.module, context);

    for(const item of this.items) {
      errors.push(...item.validateBlock(blockContext));
    }

    return errors;
  }
}
