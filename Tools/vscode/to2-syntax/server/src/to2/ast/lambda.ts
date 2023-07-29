import { Expression, Node, ValidationError } from ".";
import { InputPosition, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext } from "./context";
import { FunctionParameter } from "./function-declaration";
import { FunctionType, isFunctionType } from "./function-type";
import { RealizedType, TO2Type, UNKNOWN_TYPE } from "./to2-type";

export class Lambda extends Expression {
  constructor(
    public readonly parameters: FunctionParameter[],
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    const lambdaContext = new BlockContext(context.module, context);

    const resolved = this.resolveParameters(typeHint);
    for (const parameter of resolved) {
      lambdaContext.localVariables.set(parameter[0].value, {
        definition: {
          moduleName: context.module.moduleName,
          range: parameter[0].range,
        },
        value: parameter[1],
      });
    }
    const returnType =
      typeHint &&
      isFunctionType(typeHint) &&
      typeHint.returnType !== UNKNOWN_TYPE
        ? typeHint.returnType
        : this.expression.resultType(lambdaContext);
    return new FunctionType(
      false,
      resolved.map(([name, type, hasDefault]) => [
        name.value,
        type,
        hasDefault,
      ]),
      returnType,
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

  public validateBlock(
    context: BlockContext,
    typeHint?: RealizedType,
  ): ValidationError[] {
    const errors: ValidationError[] = [];

    const lambdaContext = new BlockContext(context.module, context);

    const resolved = this.resolveParameters(typeHint);
    for (const parameter of resolved) {
      lambdaContext.localVariables.set(parameter[0].value, {
        definition: {
          moduleName: context.module.moduleName,
          range: parameter[0].range,
        },
        value: parameter[1],
      });
    }

    errors.push(...this.expression.validateBlock(lambdaContext));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    this.expression.collectSemanticTokens(semanticTokens);
  }

  private resolveParameters(
    typeHint?: RealizedType,
  ): [WithPosition<string>, TO2Type, boolean][] {
    if (typeHint && isFunctionType(typeHint)) {
      const resolveParameters: [WithPosition<string>, TO2Type, boolean][] = [];
      for (let i = 0; i < this.parameters.length; i++) {
        const parameter = this.parameters[i];
        resolveParameters.push([
          parameter.name,
          parameter.type?.value ??
            (i < typeHint.parameterTypes.length
              ? typeHint.parameterTypes[i][1]
              : UNKNOWN_TYPE),
          false,
        ]);
      }
      return resolveParameters;
    }
    return this.parameters.map((parameter) => [
      parameter.name,
      parameter.type?.value ?? UNKNOWN_TYPE,
      false,
    ]);
  }
}
