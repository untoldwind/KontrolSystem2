import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import {
  BUILTIN_BOOL,
  BUILTIN_INT,
  GenericParameter,
  RealizedType,
  TO2Type,
  UNKNOWN_TYPE,
} from "./to2-type";
import { OptionType } from "./option-type";
import { WithDefinitionRef } from "./definition-ref";

export class ArrayType implements RealizedType {
  public readonly kind = "Array";
  public readonly elementType: TO2Type;
  public name: string;
  public localName: string;
  public description: string;

  constructor(element: TO2Type, dimension: number = 1) {
    this.elementType =
      dimension > 1 ? new ArrayType(element, dimension - 1) : element;
    this.name = this.localName = `${this.elementType.localName}[]`;
    this.description = "";
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new ArrayType(this.elementType.realizedType(context));
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new ArrayType(
      this.elementType.realizedType(context).fillGenerics(context, genericMap),
    );
  }

  guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {
    if (isArrayType(realizedType)) {
      this.elementType
        .realizedType(context)
        .guessGeneric(
          context,
          genericMap,
          realizedType.elementType.realizedType(context),
        );
    }
  }

  public findSuffixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findPrefixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    switch (name) {
      case "length":
        return { value: BUILTIN_INT };
      default:
        return undefined;
    }
  }

  public allFieldNames(): string[] {
    return ["length"];
  }

  public findMethod(name: string): WithDefinitionRef<FunctionType> | undefined {
    switch (name) {
      case "filter":
        return {
          value: new FunctionType(
            false,
            [
              [
                "predicate",
                new FunctionType(
                  false,
                  [["item", this.elementType, false]],
                  BUILTIN_BOOL,
                ),
                false,
              ],
            ],
            this,
            "Filter array based on a predicate",
          ),
        };
      case "find":
        return {
          value: new FunctionType(
            false,
            [
              [
                "predicate",
                new FunctionType(
                  false,
                  [["item", this.elementType, false]],
                  BUILTIN_BOOL,
                ),
                false,
              ],
            ],
            new OptionType(this.elementType),
            "Find an item in the array based on a predicate",
          ),
        };
      case "exists":
        return {
          value: new FunctionType(
            false,
            [
              [
                "predicate",
                new FunctionType(
                  false,
                  [["item", this.elementType, false]],
                  BUILTIN_BOOL,
                ),
                false,
              ],
            ],
            BUILTIN_BOOL,
            "Check if an item satisfying a predicate exists",
          ),
        };
      case "map":
        return {
          value: new FunctionType(
            false,
            [
              [
                "convert",
                new FunctionType(
                  false,
                  [["item", this.elementType, false]],
                  new GenericParameter("T"),
                ),
                false,
              ],
            ],
            new ArrayType(new GenericParameter("T")),
            "Check if an item satisfying a predicate exists",
          ),
        };
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
