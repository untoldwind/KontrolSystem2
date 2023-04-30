import { Expression, ModuleItem, Node, ValidationError } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext, FunctionContext, ModuleContext } from "./context";

export class MethodDeclaration implements Node, ModuleItem {
  constructor(
    public readonly isAsync: boolean,
    public readonly name: string,
    public readonly description: string,
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

  validateModule(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];
    
    const blockContext = new FunctionContext(context);

    errors.push(...this.expression.validateBlock(blockContext));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
