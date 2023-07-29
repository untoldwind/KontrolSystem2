import { Expression, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class ForIn extends Expression {
  constructor(
    private readonly forKeyword: WithPosition<"for">,
    public readonly variableName: WithPosition<string>,
    public readonly variableType: WithPosition<TO2Type> | undefined,
    public readonly inKeyword: WithPosition<"in">,
    public readonly sourceExpression: Expression,
    public readonly loopExpression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.loopExpression.reduceNode(
      combine,
      this.sourceExpression.reduceNode(combine, combine(initialValue, this)),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.sourceExpression.validateBlock(context));

    const loopContext = new BlockContext(context.module, context);

    if (errors.length === 0) {
      const loopVarType = this.sourceExpression
        .resultType(context)
        .realizedType(context.module)
        .forInSource()
        ?.realizedType(context.module);

      if (!loopVarType) {
        errors.push({
          status: "error",
          message: `${
            this.sourceExpression.resultType(context).name
          } can not be used as for-in source`,
          range: this.sourceExpression.range,
        });
      } else if (
        this.variableType &&
        this.variableType.value
          .realizedType(context.module)
          .isAssignableFrom(loopVarType)
      ) {
        errors.push({
          status: "error",
          message: `${this.variableType.value.name} is not assignable from ${loopVarType.name}`,
          range: this.sourceExpression.range,
        });
      }

      loopContext.localVariables.set(this.variableName.value, {
        definition: {
          moduleName: context.module.moduleName,
          range: this.variableName.range,
        },
        value: this.variableType?.value ?? loopVarType ?? UNKNOWN_TYPE,
      });
    }

    errors.push(...this.loopExpression.validateBlock(loopContext));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.forKeyword.range.semanticToken("keyword"));
    semanticTokens.push(
      this.variableName.range.semanticToken("variable", "definition"),
    );
    semanticTokens.push(this.inKeyword.range.semanticToken("keyword"));
    this.sourceExpression.collectSemanticTokens(semanticTokens);
    this.loopExpression.collectSemanticTokens(semanticTokens);
  }
}
