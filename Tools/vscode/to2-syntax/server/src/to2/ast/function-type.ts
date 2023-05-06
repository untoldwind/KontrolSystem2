import { ModuleContext } from "./context";
import { RealizedType, TO2Type } from "./to2-type";

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
    public readonly description: string = ""
  ) {
    this.name = `${isAsync ? "" : "sync "}fn(${parameterTypes.join(
      ", "
    )}) -> ${returnType}`;
    this.localName = this.name;
    this.maxParams = parameterTypes.length;
    this.requiredParams = parameterTypes.filter((param) => param[2]).length;
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
    return undefined;
  }

  public allFieldNames(): string[] {
    return [];
  }

  public findMethod(name: string): FunctionType | undefined {
    return undefined;
  }

  public allMethodNames(): string[] {
    return [];
  }

  public forInSource(): TO2Type | undefined {
    return undefined;
  }
}

export function isFunctionType(node: RealizedType): node is FunctionType {
  return node.kind === "Function";
}
