import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import { RealizedType, TO2Type } from "./to2-type";

export class ResultType implements RealizedType {
  public readonly kind = "Result";
  public readonly name: string;
  public readonly localName: string;
  public readonly description: string;

  constructor(
    public readonly successType: TO2Type,
    public readonly errorType: TO2Type,
  ) {
    this.name =
      this.localName = `Result<${successType.localName}, ${errorType.localName}>`;
    this.description = "";
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new ResultType(
      this.successType.realizedType(context),
      this.errorType.realizedType(context),
    );
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new ResultType(
      this.successType.realizedType(context).fillGenerics(context, genericMap),
      this.errorType.realizedType(context).fillGenerics(context, genericMap),
    );
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {
    if (isResultType(realizedType)) {
      this.successType
        .realizedType(context)
        .guessGeneric(
          context,
          genericMap,
          realizedType.successType.realizedType(context),
        );
      this.errorType
        .realizedType(context)
        .guessGeneric(
          context,
          genericMap,
          realizedType.errorType.realizedType(context),
        );
    }
  }

  findSuffixOperator(op: Operator): TO2Type | undefined {
    return op === "?" ? this.successType : this.errorType;
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
}

export function isResultType(type: RealizedType): type is ResultType {
  return type.kind === "Result";
}
