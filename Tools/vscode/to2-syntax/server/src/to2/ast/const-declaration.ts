import { Expression, ModuleItem, Node, ValidationError } from ".";
import { InputPosition, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { ModuleContext } from "./context";
import { TO2Type } from "./to2-type";

export class ConstDeclaration implements Node, ModuleItem {
  public isConstDecl: boolean = true;

  constructor(
    public readonly isPublic: boolean,
    public readonly name: WithPosition<string>,
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

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.mappedConstants.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate constant ${this.name}`,
        start: this.start,
        end: this.end,
      });
    } else {
      context.mappedConstants.set(this.name.value, this.type);
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.expression.collectSemanticTokens(semanticTokens);
  }
}

export function isConstDeclaration(node: ModuleItem): node is ConstDeclaration {
  return node.isConstDecl === true;
}
