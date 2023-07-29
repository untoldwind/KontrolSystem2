import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionType } from "./function-type";
import { RealizedType, TO2Type } from "./to2-type";

export class TupleType implements RealizedType {
  public readonly kind = "Tuple";
  public name: string;
  public localName: string;
  public description: string;

  constructor(public readonly itemTypes: TO2Type[]) {
    this.name = this.localName = `(${itemTypes
      .map((item) => item.name)
      .join(", ")})`;
    this.description = "";
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new TupleType(this.itemTypes.map((t) => t.realizedType(context)));
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new TupleType(
      this.itemTypes.map((t) =>
        t.realizedType(context).fillGenerics(context, genericMap),
      ),
    );
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {
    if (isTupleType(realizedType)) {
      for (
        let i = 0;
        i < this.itemTypes.length && i < realizedType.itemTypes.length;
        i++
      ) {
        this.itemTypes[i]
          .realizedType(context)
          .guessGeneric(
            context,
            genericMap,
            realizedType.itemTypes[i].realizedType(context),
          );
      }
    }
  }

  public findSuffixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findPrefixOperator(): RealizedType | undefined {
    return undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    return undefined;
  }

  public allFieldNames(): string[] {
    return [];
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
}

export function isTupleType(node: RealizedType): node is TupleType {
  return node.kind === "Tuple";
}
