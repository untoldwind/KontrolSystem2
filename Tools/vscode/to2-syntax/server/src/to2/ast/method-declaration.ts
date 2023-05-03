import { Expression, ModuleItem, Node, ValidationError } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext, FunctionContext, ModuleContext } from "./context";
import { FunctionParameter } from "./function-declaration";

export class MethodDeclaration implements Node, ModuleItem {
  constructor(
    public readonly isAsync: boolean,
    public readonly name: WithPosition<string>,
    public readonly description: string,
    public readonly parameters: FunctionParameter[],
    public readonly declaredReturn: TO2Type,
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

  validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    return [];
  }

  validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const blockContext = new FunctionContext(context, this.declaredReturn);

    errors.push(...this.expression.validateBlock(blockContext));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "method",
      modifiers: this.isAsync ? ["async", "declaration"] : ["declaration"],
      start: this.name.start,
      length: this.name.end.offset - this.name.start.offset,
    });
    for (const parameter of this.parameters) {
      parameter.collectSemanticTokens(semanticTokens);
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
