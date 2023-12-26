import { ModuleContext } from "./context";
import { RealizedType, TO2Type } from "./to2-type";
import { WithDefinitionRef } from "./definition-ref";

export class FunctionType implements RealizedType {
  public readonly kind = "Function";
  public readonly name: string;
  public readonly localName: string;
  public readonly maxParams: number;
  public readonly requiredParams: number;

  constructor(
    public readonly isAsync: boolean,
    public readonly parameterTypes: [string, TO2Type, boolean][],
    public readonly returnType: TO2Type,
    public readonly description: string = "",
  ) {
    this.name = `${isAsync ? "" : "sync "}fn(${parameterTypes
      .map((parameter) => parameter[1].localName)
      .join(", ")}) -> ${returnType.localName}`;
    this.localName = this.name;
    this.maxParams = parameterTypes.length;
    this.requiredParams = parameterTypes.filter((param) => !param[2]).length;
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): FunctionType {
    return new FunctionType(
      this.isAsync,
      this.parameterTypes.map(([name, type, hasDefault]) => [
        name,
        type.realizedType(context),
        hasDefault,
      ]),
      this.returnType.realizedType(context),
      this.description,
    );
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new FunctionType(
      this.isAsync,
      this.parameterTypes.map(([name, type, hasDefault]) => [
        name,
        type.realizedType(context).fillGenerics(context, genericMap),
        hasDefault,
      ]),
      this.returnType.realizedType(context).fillGenerics(context, genericMap),
      this.description,
    );
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {
    if (isFunctionType(realizedType)) {
      this.returnType
        .realizedType(context)
        .guessGeneric(
          context,
          genericMap,
          realizedType.returnType.realizedType(context),
        );
      for (
        let i = 0;
        i < this.parameterTypes.length &&
        i < realizedType.parameterTypes.length;
        i++
      ) {
        this.parameterTypes[i][1]
          .realizedType(context)
          .guessGeneric(
            context,
            genericMap,
            realizedType.parameterTypes[i][1].realizedType(context),
          );
      }
    }
  }

  public findSuffixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findPrefixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    return undefined;
  }

  public allFieldNames(): string[] {
    return [];
  }

  public findMethod(name: string): WithDefinitionRef<FunctionType> | undefined {
    return undefined;
  }

  public allMethodNames(): string[] {
    return [];
  }

  public forInSource(): TO2Type | undefined {
    return undefined;
  }

  public guessReturnType(
    context: ModuleContext,
    args: TO2Type[],
    typeHint: RealizedType | undefined,
  ): RealizedType {
    const genericMap: Record<string, RealizedType> = {};

    if (typeHint)
      this.returnType
        .realizedType(context)
        .guessGeneric(context, genericMap, typeHint);
    for (let i = 0; i < args.length && i < this.parameterTypes.length; i++) {
      this.parameterTypes[i][1]
        .realizedType(context)
        .guessGeneric(context, genericMap, args[i].realizedType(context));
    }

    return this.returnType
      .realizedType(context)
      .fillGenerics(context, genericMap);
  }
}

export function isFunctionType(
  node: RealizedType | undefined,
): node is FunctionType {
  return node !== undefined && node.kind === "Function";
}
