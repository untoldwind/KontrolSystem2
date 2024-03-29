import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import { ResultType } from "./result-type";
import { RealizedType, TO2Type, currentTypeResolver } from "./to2-type";

export class OptionType implements RealizedType {
  public readonly kind = "Option";
  public readonly name: string;
  public readonly localName: string;
  public readonly description: string;

  constructor(public readonly elementType: TO2Type) {
    this.name = this.localName = `Option<${elementType.localName}>`;
    this.description = "";
  }

  public hasGnerics(context: ModuleContext): boolean {
    return this.elementType.realizedType(context).hasGnerics(context);
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new OptionType(this.elementType.realizedType(context));
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new OptionType(
      this.elementType.realizedType(context).fillGenerics(context, genericMap),
    );
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {
    if (isOptionType(realizedType)) {
      this.elementType
        .realizedType(context)
        .guessGeneric(
          context,
          genericMap,
          realizedType.elementType.realizedType(context),
        );
    }
  }

  public findSuffixOperator(
    op: Operator,
    rightType: RealizedType,
  ): TO2Type | undefined {
    if (
      op === "|" &&
      (this.elementType as RealizedType).isAssignableFrom(rightType)
    ) {
      return this.elementType;
    } else {
      return undefined;
    }
  }

  public findPrefixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    switch (name) {
      case "defined":
        return { value: currentTypeResolver().BUILTIN_BOOL };
      case "value":
        return { value: this.elementType };
      default:
        return undefined;
    }
  }

  public allFieldNames(): string[] {
    return ["defined", "value"];
  }

  public findMethod(name: string): WithDefinitionRef<FunctionType> | undefined {
    switch (name) {
      case "ok_or":
        return {
          value: new FunctionType(
            false,
            [["error", currentTypeResolver().BUILTIN_STRING, false]],
            new ResultType(this.elementType),
          ),
        };
      default:
        return undefined;
    }
  }

  public allMethodNames(): string[] {
    return ["ok_or"];
  }

  public forInSource(): TO2Type | undefined {
    return undefined;
  }

  public supportIndexAccess(): TO2Type | undefined {
    return undefined;
  }

  public setModuleName(moduleName: string, context: ModuleContext): void {
    this.elementType.setModuleName?.(moduleName, context);
  }
}

export function isOptionType(type: RealizedType): type is OptionType {
  return type.kind === "Option";
}
