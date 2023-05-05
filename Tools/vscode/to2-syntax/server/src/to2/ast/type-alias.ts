import { TO2Type } from "./to2-type";
import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class TypeAlias implements Node, ModuleItem {
  public readonly range: InputRange;

  constructor(
    public readonly exported: boolean,
    public readonly name: WithPosition<string>,
    public readonly description: string,
    public readonly type: TO2Type,
    start: InputPosition,
    end: InputPosition
  ) {
    this.range = new InputRange(start, end);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.typeAliases.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate type name ${this.name.value}`,
        range: this.name.range,
      });
    } else {
      context.typeAliases.set(this.name.value, this.type.realizedType(context));
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}
}
