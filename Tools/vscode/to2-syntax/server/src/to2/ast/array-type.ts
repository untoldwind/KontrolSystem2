import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import {
  GenericParameter,
  RealizedType,
  TO2Type,
  UNKNOWN_TYPE,
  currentTypeResolver,
} from "./to2-type";
import { OptionType } from "./option-type";
import { WithDefinitionRef } from "./definition-ref";
import { OPERATORS, Operator } from "./operator";

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

  public hasGnerics(context: ModuleContext): boolean {
    return this.elementType.realizedType(context).hasGnerics(context);
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

  public findSuffixOperator(
    op: Operator,
    rightType: RealizedType,
  ): TO2Type | undefined {
    switch (op) {
      case "+":
      case "+=":
        return this.isAssignableFrom(rightType) ||
          (this.elementType as RealizedType).isAssignableFrom(rightType)
          ? this
          : undefined;
      default:
        return undefined;
    }
  }

  public findPrefixOperator(): TO2Type | undefined {
    return undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    switch (name) {
      case "length":
        return { value: currentTypeResolver().BUILTIN_INT };
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
                  currentTypeResolver().BUILTIN_BOOL,
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
                  currentTypeResolver().BUILTIN_BOOL,
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
                  currentTypeResolver().BUILTIN_BOOL,
                ),
                false,
              ],
            ],
            currentTypeResolver().BUILTIN_BOOL,
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
            "Convert array by applying a function on each element",
          ),
        };
      case "flat_map":
        return {
          value: new FunctionType(
            false,
            [
              [
                "convert",
                new FunctionType(
                  false,
                  [["item", this.elementType, false]],
                  new ArrayType(new GenericParameter("T")),
                ),
                false,
              ],
            ],
            new ArrayType(new GenericParameter("T")),
            "Convert array by applying a function on each element",
          ),
        };
      case "filter_map":
        return {
          value: new FunctionType(
            false,
            [
              [
                "convert",
                new FunctionType(
                  false,
                  [["item", this.elementType, false]],
                  new OptionType(new GenericParameter("T")),
                ),
                false,
              ],
            ],
            new ArrayType(new GenericParameter("T")),
            "Convert array by applying a function on each element",
          ),
        };
      case "slice":
        return {
          value: new FunctionType(
            false,
            [
              ["start", currentTypeResolver().BUILTIN_INT, false],
              ["end", currentTypeResolver().BUILTIN_INT, true],
            ],
            new ArrayType(this.elementType),
            "Get a slice of the array",
          ),
        };
      case "reverse":
        return {
          value: new FunctionType(
            false,
            [],
            new ArrayType(this.elementType),
            "Reverse the order of the array",
          ),
        };
      case "reduce":
        return {
          value: new FunctionType(
            false,
            [
              ["initial", new GenericParameter("T"), false],
              [
                "reducer",
                new FunctionType(
                  false,
                  [
                    ["item1", new GenericParameter("T"), false],
                    ["item2", this.elementType, false],
                  ],
                  new GenericParameter("T"),
                ),
                false,
              ],
            ],
            new GenericParameter("T"),
            "Reduce array by an operation",
          ),
        };
      case "sort":
        return {
          value: new FunctionType(
            false,
            [],
            new ArrayType(this.elementType),
            "Sort the array (if possible) and returns new sorted array",
          ),
        };
      case "sort_by":
        return {
          value: new FunctionType(
            false,
            [
              [
                "convert",
                new FunctionType(
                  false,
                  [["item", this.elementType, false]],
                  new GenericParameter("U"),
                ),
                false,
              ],
            ],
            new ArrayType(this.elementType),
            "Sort the array by value extracted from items. Sort value can be other number or string",
          ),
        };
      case "sort_with":
        return {
          value: new FunctionType(
            false,
            [
              [
                "comparator",
                new FunctionType(
                  false,
                  [
                    ["item1", this.elementType, false],
                    ["item2", this.elementType, false],
                  ],
                  currentTypeResolver().BUILTIN_INT,
                ),
                false,
              ],
            ],
            new ArrayType(this.elementType),
            "Sort the array with explicit comparator. Comparator should return -1 for less, 0 for equal and 1 for greater",
          ),
        };
      case "to_string":
        return {
          value: new FunctionType(
            false,
            [],
            currentTypeResolver().BUILTIN_STRING,
            "Get string representation of the array",
          ),
        };
    }
    return undefined;
  }

  public allMethodNames(): string[] {
    return [
      "filter",
      "find",
      "exists",
      "map",
      "flat_map",
      "filter_map",
      "slice",
      "reverse",
      "reduce",
      "sort",
      "sort_by",
      "sort_with",
      "to_string",
    ];
  }

  public forInSource(): TO2Type | undefined {
    return this.elementType;
  }

  public supportIndexAccess(): TO2Type | undefined {
    return this.elementType;
  }

  public setModuleName(moduleName: string, context: ModuleContext): void {
    this.elementType.setModuleName?.(moduleName, context);
  }
}

export function isArrayType(node: RealizedType): node is ArrayType {
  return node.kind === "Array";
}
