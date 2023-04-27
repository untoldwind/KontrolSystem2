import { BlockItem, Expression, Node, ValidationError } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";

export class DeclarationParameter {
  constructor(
    public readonly target: string,
    public readonly source: string | undefined,
    public readonly type: TO2Type | undefined
  ) {}
}

export class DeclarationPlaceholder {}

export type DeclarationParameterOrPlaceholder =
  | DeclarationParameter
  | DeclarationPlaceholder;

export class VariableDeclaration implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(
    public readonly declaration: DeclarationParameter,
    public readonly isConst: boolean,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

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

    if (context.localVariables.has(this.declaration.target)) {
      errors.push({
        status: "error",
        message: `Duplicate variable ${this.declaration.target}`,
        start: this.start,
        end: this.end,
      });
    } else {
      context.localVariables.set(this.declaration.target, this.resultType(context));
    }

    return errors;
  }
}
