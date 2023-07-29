import { Expression, ModuleItem, Node, ValidationError } from ".";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { ModuleContext } from "./context";
import { TO2Type } from "./to2-type";

export class ConstDeclaration implements Node, ModuleItem {
  public isConstDecl: true = true;
  public readonly range: InputRange;

  constructor(
    public readonly isPublic: boolean,
    public readonly name: WithPosition<string>,
    public readonly description: string,
    public readonly type: WithPosition<TO2Type>,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.mappedConstants.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate constant ${this.name}`,
        range: this.range,
      });
    } else {
      context.mappedConstants.set(this.name.value, {
        definition: { moduleName: context.moduleName, range: this.name.range },
        value: this.type.value,
      });
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.expression.collectSemanticTokens(semanticTokens);
  }

  public setModuleName(moduleName: string) {
    this.type.value.setModuleName?.(moduleName);
  }
}

export function isConstDeclaration(node: ModuleItem): node is ConstDeclaration {
  return !!node.isConstDecl;
}
