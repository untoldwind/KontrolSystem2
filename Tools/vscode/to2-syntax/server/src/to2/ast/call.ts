import { Expression, Node, ValidationError } from ".";
import { RealizedType, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { FunctionType, isFunctionType } from "./function-type";
import { SemanticToken } from "../../syntax-token";

export class Call extends Expression {
  constructor(
    public readonly namePath: WithPosition<string[]>,
    public readonly args: Expression[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    const variableType = context
      .findVariable(
        this.namePath.value,
        new FunctionType(
          false,
          this.args.map((arg, idx) => [
            `param${idx}`,
            arg.resultType(context),
            false,
          ]),
          typeHint ?? UNKNOWN_TYPE
        )
      )
      ?.realizedType(context.module);
    return variableType && isFunctionType(variableType)
      ? variableType.returnType
      : UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.args.reduce(
      (prev, arg) => arg.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validateBlock(
    context: BlockContext,
    typeHint?: RealizedType
  ): ValidationError[] {
    const errors: ValidationError[] = [];

    const variableType = context
      .findVariable(
        this.namePath.value,
        new FunctionType(
          false,
          this.args.map((arg, idx) => [
            `param${idx}`,
            arg.resultType(context),
            false,
          ]),
          typeHint ?? UNKNOWN_TYPE
        )
      )
      ?.realizedType(context.module);
    if (!variableType) {
      errors.push({
        status: "error",
        message: `Undefined variable or function: ${this.namePath.value.join(
          "::"
        )}`,
        start: this.namePath.start,
        end: this.namePath.end,
      });
    } else if (!isFunctionType(variableType)) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath.value.join(
          "::"
        )} is not callable`,
        start: this.namePath.start,
        end: this.namePath.end,
      });
    } else {
      if (this.args.length > variableType.maxParams) {
        errors.push({
          status: "error",
          message: `${this.namePath.value.join("::")} only takes ${
            variableType.maxParams
          } arguments`,
          start: this.namePath.start,
          end: this.namePath.end,
        });
      } else if (this.args.length < variableType.requiredParams) {
        errors.push({
          status: "error",
          message: `${this.namePath.value.join("::")} at least requires ${
            variableType.requiredParams
          } arguments`,
          start: this.namePath.start,
          end: this.namePath.end,
        });
      } else {
        for (let i = 0; i < this.args.length; i++) {
          errors.push(
            ...this.args[i].validateBlock(
              context,
              variableType.parameterTypes[i][1].realizedType(context.module)
            )
          );
        }
      }
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "function",
      start: this.namePath.start,
      length: this.namePath.end.offset - this.namePath.start.offset,
    });
    for (const arg of this.args) {
      arg.collectSemanticTokens(semanticTokens);
    }
  }
}
