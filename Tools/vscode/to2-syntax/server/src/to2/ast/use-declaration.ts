import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition, WithPosition } from "../../parser";
import { ModuleContext } from "./context";

export class UseDeclaration implements Node, ModuleItem {
  constructor(
    public readonly names: WithPosition<string>[] | undefined,
    public readonly alias: WithPosition<string> | undefined,
    public readonly moduleNamePath: WithPosition<string[]>,
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

    if (!context.findModule(this.moduleNamePath.value)) {
      errors.push({
        status: "error",
        message: `Module not found: ${this.moduleNamePath.value}`,
        start: this.moduleNamePath.start,
        end: this.moduleNamePath.end,
      });
    }
    
    return errors;
  }
}
