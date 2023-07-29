import { Expression, Node, ValidationError } from ".";
import { InputPosition, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext } from "./context";
import { RecordType } from "./record-type";
import { TO2Type } from "./to2-type";

export class RecordCreate extends Expression {
  constructor(
    public readonly declaredResult: TO2Type | undefined,
    public readonly items: [WithPosition<string>, Expression][],
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return new RecordType(
      this.items.map(([name, item]) => [name, item.resultType(context)]),
    );
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.items.reduce(
      (prev, item) => item[1].reduceNode(combine, prev),
      combine(initialValue, this),
    );
  }
  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}
}
