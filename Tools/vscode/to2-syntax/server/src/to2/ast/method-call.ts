import { Expression, Node, ValidationError } from ".";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext } from "./context";
import { DefinitionRef, WithDefinitionRef } from "./definition-ref";
import { FunctionType, isFunctionType } from "./function-type";
import { RealizedType, TO2Type, UNKNOWN_TYPE } from "./to2-type";

export class MethodCall extends Expression {
  public reference?: { sourceRange: InputRange; definition: DefinitionRef };

  constructor(
    public readonly target: Expression,
    public readonly methodName: WithPosition<string>,
    public readonly args: Expression[],
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    return (
      this.findMethod(context)?.value.guessReturnType(
        context.module,
        this.args.map((arg) => arg.resultType(context)),
        typeHint,
      ) ?? UNKNOWN_TYPE
    );
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.args.reduce(
      (prev, arg) => arg.reduceNode(combine, prev),
      this.target.reduceNode(combine, combine(initialValue, this)),
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    errors.push(...this.target.validateBlock(context));

    if (errors.length > 0) return errors;

    const { definition, value: methodType } = this.findMethod(context) ?? {};
    if (definition) {
      this.reference = {
        sourceRange: this.methodName.range,
        definition,
      };
    }

    if (methodType) {
      if (this.args.length > methodType.maxParams) {
        errors.push({
          status:
            this.target.resultType(context) === UNKNOWN_TYPE ? "warn" : "error",
          message: `${this.methodName.value} only takes ${methodType.maxParams} arguments`,
          range: this.methodName.range,
        });
      } else if (this.args.length < methodType.requiredParams) {
        errors.push({
          status: "error",
          message: `${this.methodName.value} at least requires ${methodType.requiredParams} arguments`,
          range: this.methodName.range,
        });
      } else {
        for (let i = 0; i < this.args.length; i++) {
          errors.push(
            ...this.args[i].validateBlock(
              context,
              methodType.parameterTypes[i][1].realizedType(context.module),
            ),
          );
        }
      }
      const targetType = this.target
        .resultType(context)
        .realizedType(context.module);
      this.documentation = [
        this.methodName.range.with(
          `Method \`${targetType.name}.${
            this.methodName.value
          }(${methodType.parameterTypes
            .map(([name, type]) => `${name} : ${type.name}`)
            .join(", ")}) -> ${methodType.returnType.name}\``,
        ),
      ];
      if (methodType.description)
        this.documentation.push(
          this.methodName.range.with(methodType.description),
        );
    } else {
      errors.push({
        status: "error",
        message: `Undefined method ${this.methodName.value} for type ${
          this.target.resultType(context).name
        }`,
        range: this.methodName.range,
      });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.target.collectSemanticTokens(semanticTokens);
    semanticTokens.push(this.methodName.range.semanticToken("method"));
    for (const arg of this.args) {
      arg.collectSemanticTokens(semanticTokens);
    }
  }

  private findMethod(
    context: BlockContext,
  ): WithDefinitionRef<FunctionType> | undefined {
    const targetType = this.target
      .resultType(context)
      .realizedType(context.module);

    const method = targetType.findMethod(this.methodName.value);
    if (method) return method;

    const { definition, value: field } =
      targetType.findField(this.methodName.value) ?? {};
    const fieldRealized = field?.realizedType(context.module);

    return fieldRealized && isFunctionType(fieldRealized)
      ? { definition, value: fieldRealized }
      : undefined;
  }
}
