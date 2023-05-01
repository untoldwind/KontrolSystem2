import { TO2Type } from "./to2-type";
import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition, WithPosition } from "../../parser";
import { ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class TypeAlias implements Node, ModuleItem {
  constructor(
    public readonly exported: boolean,
    public readonly name: WithPosition<string>,
    public readonly description: string,
    public readonly type: TO2Type,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}
}
