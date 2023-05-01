import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import { RealizedType, TO2Type } from "./to2-type";

export class ArrayType implements RealizedType {
  public readonly kind = "Array";
  public readonly elementType: TO2Type;
  public name: string;
  public localName: string;
  public description: string;

  constructor(element: TO2Type, dimension: number = 1) {
    this.elementType =
      dimension > 1 ? new ArrayType(element, dimension - 1) : element;
    this.name = this.localName = `${this.elementType}[]`;
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
    return undefined;
  }

  public findMethod(name: string): FunctionType | undefined {
    return undefined;
  }

  public forInSource(): TO2Type | undefined {
    return this.elementType;
  }
}
