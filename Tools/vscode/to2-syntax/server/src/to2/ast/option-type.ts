import { ModuleContext } from "./context";
import { RealizedType, TO2Type } from "./to2-type";

export class OptionType implements RealizedType {
  public name: string;
  public localName: string;
  public description: string;

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
}
