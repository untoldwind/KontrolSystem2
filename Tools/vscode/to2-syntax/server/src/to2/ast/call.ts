import { Expression, Node, ValidationError } from ".";
import { RealizedType, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { FunctionType, isFunctionType } from "./function-type";
import { SemanticToken } from "../../syntax-token";
import { DefinitionRef } from "./definition-ref";

export class Call extends Expression {
  public reference?: { sourceRange: InputRange; definition: DefinitionRef };

  constructor(
    public readonly namePath: WithPosition<string[]>,
    public readonly args: Expression[],
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    const { definition, value: variableType } =
      context.findVariable(
        this.namePath.value,
        new FunctionType(
          false,
          this.args.map((arg, idx) => [
            `param${idx}`,
            arg.resultType(context),
            false,
          ]),
          typeHint ?? UNKNOWN_TYPE,
        ),
      ) ?? {};

    if (definition) {
      this.reference = {
        sourceRange: this.namePath.range,
        definition,
      };
    }

    const realizedType = variableType?.realizedType(context.module);
    return isFunctionType(realizedType)
      ? realizedType.guessReturnType(
          context.module,
          this.args.map((arg) => arg.resultType(context)),
          typeHint,
        )
      : UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.args.reduce(
      (prev, arg) => arg.reduceNode(combine, prev),
      combine(initialValue, this),
    );
  }

  public validateBlock(
    context: BlockContext,
    typeHint?: RealizedType,
  ): ValidationError[] {
    const errors: ValidationError[] = [];

    const { definition, value: variableType } =
      context.findVariable(
        this.namePath.value,
        new FunctionType(
          false,
          this.args.map((arg, idx) => [
            `param${idx}`,
            arg.resultType(context),
            false,
          ]),
          typeHint ?? UNKNOWN_TYPE,
        ),
      ) ?? {};
    const realizedType = variableType?.realizedType(context.module);
    if (!realizedType) {
      errors.push({
        status: "error",
        message: `Undefined variable or function: ${this.namePath.value.join(
          "::",
        )}`,
        range: this.namePath.range,
      });
    } else if (!isFunctionType(realizedType)) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath.value.join(
          "::",
        )} is not callable`,
        range: this.namePath.range,
      });
    } else {
      if (this.args.length > realizedType.maxParams) {
        errors.push({
          status: "error",
          message: `${this.namePath.value.join("::")} only takes ${
            realizedType.maxParams
          } arguments`,
          range: this.namePath.range,
        });
      } else if (this.args.length < realizedType.requiredParams) {
        errors.push({
          status: "error",
          message: `${this.namePath.value.join("::")} at least requires ${
            realizedType.requiredParams
          } arguments`,
          range: this.namePath.range,
        });
      } else {
        for (let i = 0; i < this.args.length; i++) {
          const parameterType = realizedType.parameterTypes[i][1].realizedType(
            context.module,
          );
          errors.push(...this.args[i].validateBlock(context, parameterType));
          const argResult = this.args[i]
            .resultType(context, parameterType)
            .realizedType(context.module);
          if (!parameterType.isAssignableFrom(argResult)) {
            errors.push({
              status: "error",
              message: `Parameter ${realizedType.parameterTypes[i][0]}: ${argResult.name} is not assignable to ${parameterType.name}`,
              range: this.args[i].range,
            });
          }
        }
      }
      this.documentation = [
        this.namePath.range.with(
          `Function \`${this.namePath.value.join(
            "::",
          )}(${realizedType.parameterTypes
            .map(([name, type]) => `${name} : ${type.localName}`)
            .join(", ")}) -> ${realizedType.returnType.localName}\``,
        ),
      ];
      if (realizedType.description)
        this.documentation.push(
          this.namePath.range.with(realizedType.description),
        );
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.namePath.range.semanticToken("function"));
    for (const arg of this.args) {
      arg.collectSemanticTokens(semanticTokens);
    }
  }
}
