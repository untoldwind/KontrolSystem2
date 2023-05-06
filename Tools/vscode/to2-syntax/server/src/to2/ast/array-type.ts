import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import {
  BUILTIN_BOOL,
  BUILTIN_INT,
  RealizedType,
  TO2Type,
  UNKNOWN_TYPE,
} from "./to2-type";
import { OptionType } from "./option-type";

export class ArrayType implements RealizedType {
  public readonly kind = "Array";
  public readonly elementType: TO2Type;
  public name: string;
  public localName: string;
  public description: string;

  constructor(element: TO2Type, dimension: number = 1) {
    this.elementType =
      dimension > 1 ? new ArrayType(element, dimension - 1) : element;
    this.name = this.localName = `${this.elementType.name}[]`;
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
      case "length":
        return BUILTIN_INT;
      default:
        return undefined;
    }
  }

  public allFieldNames(): string[] {
    return ["length"];
  }

  public findMethod(name: string): FunctionType | undefined {
    switch (name) {
      case "filter":
        return new FunctionType(
          false,
          [
            [
              "predicate",
              new FunctionType(
                false,
                [["item", this.elementType, false]],
                BUILTIN_BOOL
              ),
              false,
            ],
          ],
          this,
          "Filter array based on a predicate"
        );
      case "find":
        return new FunctionType(
          false,
          [
            [
              "predicate",
              new FunctionType(
                false,
                [["item", this.elementType, false]],
                BUILTIN_BOOL
              ),
              false,
            ],
          ],
          new OptionType(this.elementType),
          "Find an item in the array based on a predicate"
        );
      case "exists":
        return new FunctionType(
          false,
          [
            [
              "predicate",
              new FunctionType(
                false,
                [["item", this.elementType, false]],
                BUILTIN_BOOL
              ),
              false,
            ],
          ],
          BUILTIN_BOOL,
          "Check if an item satisfying a predicate exists"
        );
      case "map":
        return new FunctionType(
          false,
          [
            [
              "convert",
              new FunctionType(
                false,
                [["item", this.elementType, false]],
                UNKNOWN_TYPE
              ),
              false,
            ],
          ],
          new ArrayType(UNKNOWN_TYPE),
          "Check if an item satisfying a predicate exists"
        );
    }
    return undefined;
  }

  public allMethodNames(): string[] {
    return ["filter", "find", "exists", "map"];
  }

  public forInSource(): TO2Type | undefined {
    return this.elementType;
  }
}

export function isArrayType(node: RealizedType): node is ArrayType {
  return node.kind === "Array";
}
