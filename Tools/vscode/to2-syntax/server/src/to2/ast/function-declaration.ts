import { Expression, ModuleItem, Node, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { BlockContext, FunctionContext, ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import { SemanticToken } from "../../syntax-token";

export enum FunctionModifier {
  Public,
  Private,
  Test,
}

export class FunctionParameter implements Node {
  public readonly range: InputRange;

  constructor(
    public readonly name: WithPosition<string>,
    public readonly type: WithPosition<TO2Type> | undefined,
    public readonly defaultValue: Expression | undefined,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return (
      this.type?.value ?? this.defaultValue?.resultType(context) ?? UNKNOWN_TYPE
    );
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    if (this.defaultValue)
      return this.defaultValue.reduceNode(combine, combine(initialValue, this));
    return combine(initialValue, this);
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(
      this.name.range.semanticToken("parameter", "declaration"),
    );
  }
}

export class FunctionDeclaration implements Node, ModuleItem {
  public isFunctionDecl: true = true;
  public functionType: FunctionType;
  public readonly modifier: FunctionModifier;
  public readonly isAsync: boolean;
  public readonly range: InputRange;

  constructor(
    private readonly prefix: WithPosition<"fn" | "sync" | "test" | "pub">[],
    public readonly name: WithPosition<string>,
    public readonly description: string,
    public readonly parameters: FunctionParameter[],
    public readonly declaredReturn: TO2Type,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
    this.isAsync = prefix.findIndex((p) => p.value === "sync") < 0;
    this.modifier =
      prefix.findIndex((p) => p.value === "pub") >= 0
        ? FunctionModifier.Public
        : prefix.findIndex((p) => p.value === "test") >= 0
          ? FunctionModifier.Test
          : FunctionModifier.Private;
    this.functionType = new FunctionType(
      this.isAsync,
      parameters.map((param) => [
        param.name.value,
        param.type?.value ?? UNKNOWN_TYPE,
        param.defaultValue !== undefined,
      ]),
      declaredReturn,
      description,
    );
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.expression.reduceNode(
      combine,
      this.parameters.reduce(
        (prev, param) => param.reduceNode(combine, prev),
        combine(initialValue, this),
      ),
    );
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.mappedFunctions.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate function ${this.name}`,
        range: this.name.range,
      });
    } else {
      const blockContext = new FunctionContext(context, this.declaredReturn);

      this.functionType = new FunctionType(
        this.isAsync,
        this.parameters.map((param) => [
          param.name.value,
          param.resultType(blockContext).realizedType(context),
          param.defaultValue !== undefined,
        ]),
        this.declaredReturn.realizedType(context),
      );
      context.mappedFunctions.set(this.name.value, {
        definition: { moduleName: context.moduleName, range: this.name.range },
        value: this.functionType,
      });
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const blockContext = new FunctionContext(context, this.declaredReturn);

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
    for (const keyword of this.prefix) {
      semanticTokens.push(keyword.range.semanticToken("keyword"));
    }
    semanticTokens.push(
      this.isAsync
        ? this.name.range.semanticToken("function", "async", "declaration")
        : this.name.range.semanticToken("function", "declaration"),
    );
    for (const parameter of this.parameters) {
      parameter.collectSemanticTokens(semanticTokens);
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }

  public setModuleName(moduleName: string) {}
}

export function isFunctionDeclaration(
  node: ModuleItem,
): node is FunctionDeclaration {
  return !!node.isFunctionDecl;
}
