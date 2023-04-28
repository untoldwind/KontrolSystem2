import { ModuleContext } from "./context";
import { RealizedType, TO2Type } from "./to2-type";

export class FunctionType implements RealizedType {
  public isFunction: boolean = true;
  public name: string;
  public localName: string;

  constructor(
    public readonly isAsync: boolean,
    public readonly parameterTypes: [string, TO2Type][],
    public readonly returnType: TO2Type,
    public readonly description: string = ""
  ) {
    this.name = `${isAsync ? "" : "sync "}fn(${parameterTypes.join(
      ", "
    )}) -> ${returnType}`;
    this.localName = this.name;
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
}

export function isFunctionType(node: RealizedType): node is FunctionType {
  return node.isFunction === true;
}
