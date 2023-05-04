import { Expression, ModuleItem, Node, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext, FunctionContext, ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import { SemanticToken } from "../../syntax-token";

export enum FunctionModifier {
  Public,
  Private,
  Test,
}

export class FunctionParameter implements Node {
  constructor(
    public readonly name: WithPosition<string>,
    public readonly type: TO2Type | undefined,
    public readonly defaultValue: Expression | undefined,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public resultType(context: BlockContext): TO2Type {
    return this.type ?? this.defaultValue?.resultType(context) ?? UNKNOWN_TYPE;
  }

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
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
    semanticTokens.push({
      type: "parameter",
      modifiers: ["definition"],
      start: this.name.start,
      length: this.name.end.offset - this.name.start.offset,
    });
  }
}

export class FunctionDeclaration implements Node, ModuleItem {
  public isFunctionDecl: boolean = true;
  public functionType: FunctionType;
  public readonly modifier: FunctionModifier;
  public readonly isAsync: boolean;

  constructor(
    private readonly prefix: WithPosition<"fn" | "sync" | "test" | "pub">[],
    public readonly name: WithPosition<string>,
    public readonly description: string,
    public readonly parameters: FunctionParameter[],
    public readonly declaredReturn: TO2Type,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {
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
        param.type ?? UNKNOWN_TYPE,
        param.defaultValue !== undefined,
      ]),
      declaredReturn,
      description
    );
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(
      combine,
      this.parameters.reduce(
        (prev, param) => param.reduceNode(combine, prev),
        combine(initialValue, this)
      )
    );
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.mappedFunctions.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate function ${this.name}`,
        start: this.name.start,
        end: this.name.end,
      });
    } else {
      const blockContext = new FunctionContext(context, this.declaredReturn);

      context.mappedFunctions.set(
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
    for (const keyword of this.prefix) {
      semanticTokens.push({
        type: "keyword",
        start: keyword.start,
        length: keyword.end.offset - keyword.start.offset,
      });
    }
    semanticTokens.push({
      type: "function",
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

export function isFunctionDeclaration(
  node: ModuleItem
): node is FunctionDeclaration {
  return node.isFunctionDecl === true;
}
