import { Expression, ModuleItem, Node, ValidationError } from ".";
import { InputPosition } from "../../parser";
import { ModuleContext } from "./context";
import { TO2Type } from "./to2-type";

export class ConstDeclaration implements Node, ModuleItem {
  constructor(
    public readonly isPublic: boolean,
    public readonly name: string,
    public readonly description: string,
    public readonly type: TO2Type,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateModule(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.mappedConstants.has(this.name)) {
      errors.push({
        status: "error",
        message: `Duplicate constant ${this.name}`,
        start: this.start,
        end: this.end,
      });
    } else {
      context.mappedConstants.set(this.name, this.type);
    }

    return errors;
  }
}
