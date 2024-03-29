import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import { RealizedType, TO2Type, currentTypeResolver } from "./to2-type";

export class ResultType implements RealizedType {
  public readonly kind = "Result";
  public readonly name: string;
  public readonly localName: string;
  public readonly description: string;

  constructor(public readonly successType: TO2Type) {
    this.name = this.localName = `Result<${successType.localName}>`;
    this.description = "";
  }

  public hasGnerics(context: ModuleContext): boolean {
    return this.successType.realizedType(context).hasGnerics(context);
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new ResultType(this.successType.realizedType(context));
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new ResultType(
      this.successType.realizedType(context).fillGenerics(context, genericMap),
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
    }
  }

  findSuffixOperator(op: Operator): TO2Type | undefined {
    return op === "?" ? this.successType : currentTypeResolver().BUILTIN_ERROR;
  }

  public findPrefixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    switch (name) {
      case "success":
        return { value: currentTypeResolver().BUILTIN_BOOL };
      case "value":
        return { value: this.successType };
      case "error":
        return { value: currentTypeResolver().BUILTIN_ERROR };
      default:
        return undefined;
    }
  }

  public allFieldNames(): string[] {
    return ["success", "value", "error"];
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

  public supportIndexAccess(): TO2Type | undefined {
    return undefined;
  }

  public setModuleName(moduleName: string, context: ModuleContext): void {
    this.successType.setModuleName?.(moduleName, context);
  }
}

export function isResultType(type: RealizedType): type is ResultType {
  return type.kind === "Result";
}
