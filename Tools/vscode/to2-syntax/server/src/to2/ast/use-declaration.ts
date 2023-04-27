import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition } from "../../parser";
import { ModuleContext } from "./context";

export class UseDeclaration implements Node, ModuleItem {
  constructor(
    public readonly names: string[] | undefined,
    public readonly alias: string | undefined,
    public readonly moduleNamePath: string[],
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
  public validateModule(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }
}
