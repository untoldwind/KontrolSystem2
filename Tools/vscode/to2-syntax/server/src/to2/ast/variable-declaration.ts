import { BlockItem, Expression, Node, ValidationError } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";

export class DeclarationParameter {
  constructor(
    public readonly target: WithPosition<string>,
    public readonly source: string | undefined,
    public readonly type: TO2Type | undefined
  ) {}
}

export class DeclarationPlaceholder {}

export type DeclarationParameterOrPlaceholder =
  | DeclarationParameter
  | DeclarationPlaceholder;

export class VariableDeclaration implements Node, BlockItem {
  constructor(
    public readonly declaration: DeclarationParameter,
    public readonly isConst: boolean,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public resultType(context: BlockContext): TO2Type {
    return this.declaration.type ?? this.expression.resultType(context);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.localVariables.has(this.declaration.target.value)) {
      errors.push({
        status: "error",
        message: `Duplicate variable ${this.declaration.target}`,
        start: this.declaration.target.start,
        end: this.declaration.target.end,
      });
    } else {
      context.localVariables.set(
        this.declaration.target.value,
        this.resultType(context)
      );
    }
    errors.push(...this.expression.validateBlock(context));

    return errors;
  }
}
