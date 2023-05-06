import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import { ResultType } from "./result-type";
import {
  BUILTIN_BOOL,
  BUILTIN_STRING,
  RealizedType,
  TO2Type,
} from "./to2-type";

export class OptionType implements RealizedType {
  public readonly kind = "Option";
  public readonly name: string;
  public readonly localName: string;
  public readonly description: string;

  constructor(public readonly elementType: TO2Type) {
    this.name = this.localName = `Option<${elementType}>`;
    this.description = "";
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return this;
  }

  public findSuffixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findPrefixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findField(name: string): TO2Type | undefined {
    switch (name) {
      case "defined":
        return BUILTIN_BOOL;
      case "value":
        return this.elementType;
      default:
        return undefined;
    }
  }

  public allFieldNames(): string[] {
    return ["defined", "value"];
  }

  public findMethod(name: string): FunctionType | undefined {
    switch (name) {
      case "ok_or":
        return new FunctionType(
          false,
          [["error", BUILTIN_STRING, false]],
          new ResultType(this.elementType, BUILTIN_STRING)
        );
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
}

export function isOptionType(type: RealizedType): type is OptionType {
  return type.kind === "Option";
}
