import { Expression, ModuleItem, Node, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { FunctionContext, ModuleContext, isImplModuleContext } from "./context";
import { FunctionParameter } from "./function-declaration";
import { FunctionType } from "./function-type";

export class MethodDeclaration implements Node, ModuleItem {
  public readonly range: InputRange;
  constructor(
    public readonly isAsync: boolean,
    public readonly name: WithPosition<string>,
    public readonly description: string,
    public readonly parameters: FunctionParameter[],
    public readonly declaredReturn: TO2Type,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  functionType(): FunctionType {
    return new FunctionType(
      this.isAsync,
      this.parameters.map((p) => [
        p.name.value,
        p.type?.value ?? UNKNOWN_TYPE,
        p.defaultValue !== undefined,
      ]),
      this.declaredReturn,
    );
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
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
        range: this.name.range,
      });
    } else {
      const blockContext = new FunctionContext(context, this.declaredReturn);

      context.structType.methods.set(this.name.value, {
        definition: {
          moduleName: context.moduleName,
          range: this.name.range,
        },
        value: new FunctionType(
          this.isAsync,
          this.parameters.map((param) => [
            param.name.value,
            param.resultType(blockContext).realizedType(context),
            param.defaultValue !== undefined,
          ]),
          this.declaredReturn.realizedType(context),
        ),
      });
    }

    return [];
  }

  validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    if (!isImplModuleContext(context)) return [];

    const errors: ValidationError[] = [];

    const blockContext = new FunctionContext(context, this.declaredReturn);

    blockContext.localVariables.set("self", {
      definition: { moduleName: context.moduleName, range: this.name.range },
      value: context.structType,
    });
    for (const parameter of this.parameters) {
      errors.push(...parameter.validateBlock(blockContext));
      if (blockContext.localVariables.has(parameter.name.value)) {
        errors.push({
          status: "error",
          message: `Duplicate parameter name ${parameter.name}`,
          range: parameter.name.range,
        });
      } else {
        blockContext.localVariables.set(parameter.name.value, {
          definition: {
            moduleName: context.moduleName,
            range: parameter.name.range,
          },
          value: parameter.resultType(blockContext),
        });
      }
    }

    errors.push(...this.expression.validateBlock(blockContext));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(
      this.isAsync
        ? this.name.range.semanticToken("method", "async", "declaration")
        : this.name.range.semanticToken("method", "declaration"),
    );
    for (const parameter of this.parameters) {
      parameter.collectSemanticTokens(semanticTokens);
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }

  public setModuleName(moduleName: string) {}
}
