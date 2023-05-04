import { Expression, ModuleItem, Node, ValidationError } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { FunctionContext, ModuleContext, isImplModuleContext } from "./context";
import { FunctionParameter } from "./function-declaration";
import { FunctionType } from "./function-type";

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
    if (!isImplModuleContext(context)) return [];

    const errors: ValidationError[] = [];

    if (context.structType.methods.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate method ${this.name}`,
        start: this.name.start,
        end: this.name.end,
      });
    } else {
      const blockContext = new FunctionContext(context, this.declaredReturn);

      context.structType.methods.set(
        this.name.value,
        new FunctionType(
          this.isAsync,
          this.parameters.map((param) => [
            param.name.value,
            param.resultType(blockContext),
            param.defaultValue !== undefined,
          ]),
          this.declaredReturn
        )
      );
    }

    return [];
  }

  validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    if (!isImplModuleContext(context)) return [];

    const errors: ValidationError[] = [];

    const blockContext = new FunctionContext(context, this.declaredReturn);

    blockContext.localVariables.set("self", context.structType);
    for (const parameter of this.parameters) {
      errors.push(...parameter.validateBlock(blockContext));
      if (blockContext.localVariables.has(parameter.name.value)) {
        errors.push({
          status: "error",
          message: `Duplicate parameter name ${parameter.name}`,
          start: parameter.name.start,
          end: parameter.name.end,
        });
      } else {
        blockContext.localVariables.set(
          parameter.name.value,
          parameter.resultType(blockContext)
        );
      }
    }

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
