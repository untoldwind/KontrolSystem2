import { WithPosition } from "../../parser";
import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import { RealizedType, TO2Type } from "./to2-type";

export class RecordType implements RealizedType {
  public readonly kind = "Record";
  public name: string;
  public localName: string;
  public description: string;
  public methods: Map<string, FunctionType>;

  constructor(public readonly itemTypes: [string, WithPosition<TO2Type>][]) {
    this.name = this.localName = `(${itemTypes
      .map((item) => `${item[0]} : ${item[1].value.localName}`)
      .join(", ")})`;
    this.description = "";
    this.methods = new Map();
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new RecordType(
      this.itemTypes.map(([name, type]) => [
        name,
        { range: type.range, value: type.value.realizedType(context) },
      ])
    );
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>
  ): RealizedType {
    return new RecordType(
      this.itemTypes.map(([name, type]) => [
        name,
        {
          range: type.range,
          value: type.value
            .realizedType(context)
            .fillGenerics(context, genericMap),
        },
      ])
    );
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType
  ): void {
    if (isRecordType(realizedType)) {
      for (
        let i = 0;
        i < this.itemTypes.length && i < realizedType.itemTypes.length;
        i++
      ) {
        this.itemTypes[i][1].value
          .realizedType(context)
          .guessGeneric(
            context,
            genericMap,
            realizedType.itemTypes[i][1].value.realizedType(context)
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

  public findField(name: string): TO2Type | undefined {
    return this.itemTypes.find((item) => item[0] === name)?.[1].value;
  }

  public allFieldNames(): string[] {
    return this.itemTypes.map((item) => item[0]);
  }

  public findMethod(name: string): FunctionType | undefined {
    return this.methods.get(name);
  }

  public allMethodNames(): string[] {
    return [...this.methods.keys()];
  }

  public forInSource(): TO2Type | undefined {
    return undefined;
  }
}

export function isRecordType(node: RealizedType): node is RecordType {
  return node.kind === "Record";
}
