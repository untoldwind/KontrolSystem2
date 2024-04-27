import { WithPosition } from "../../parser";
import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import { RealizedType, TO2Type } from "./to2-type";

type OperatorMap = Map<
  Operator,
  { otherType: RealizedType; resultType: RealizedType }[]
>;

export class RecordType implements RealizedType {
  public readonly kind = "Record";
  public name: string;
  public localName: string;
  public description: string;
  public methods: Map<string, WithDefinitionRef<FunctionType>>;
  public suffixOperators: OperatorMap;
  public prefixOperators: OperatorMap;

  constructor(
    public readonly itemTypes: [WithPosition<string>, TO2Type][],
    methods?: Map<string, WithDefinitionRef<FunctionType>>,
    suffixOperators?: OperatorMap,
    prefixOperators?: OperatorMap,
    public readonly structName?: string,
    private moduleName: string = "<unknown>",
  ) {
    this.name = this.localName =
      structName ??
      `(${itemTypes
        .map((item) => `${item[0].value} : ${item[1].localName}`)
        .join(", ")})`;
    this.description = "";
    this.methods = methods ?? new Map();
    this.suffixOperators = suffixOperators ?? new Map();
    this.prefixOperators = prefixOperators ?? new Map();
  }

  public hasGenerics(context: ModuleContext): boolean {
    return (
      this.itemTypes.find((item) =>
        item[1].realizedType(context).hasGenerics(context),
      ) !== undefined
    );
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new RecordType(
      this.itemTypes.map(([name, type]) => [name, type.realizedType(context)]),
      this.methods,
      this.suffixOperators,
      this.prefixOperators,
      this.structName,
      this.moduleName,
    );
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new RecordType(
      this.itemTypes.map(([name, type]) => [
        name,
        type.realizedType(context).fillGenerics(context, genericMap),
      ]),
      this.methods,
      this.suffixOperators,
      this.prefixOperators,
      this.structName,
      this.moduleName,
    );
  }

  public fillGenericArguments(typeParameters: RealizedType[]): RealizedType {
    return this;
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {
    if (isRecordType(realizedType)) {
      for (
        let i = 0;
        i < this.itemTypes.length && i < realizedType.itemTypes.length;
        i++
      ) {
        this.itemTypes[i][1]
          .realizedType(context)
          .guessGeneric(
            context,
            genericMap,
            realizedType.itemTypes[i][1].realizedType(context),
          );
      }
    }
  }

  public findSuffixOperator(
    op: Operator,
    rightType: RealizedType,
  ): TO2Type | undefined {
    const opRef = this.suffixOperators
      .get(op)
      ?.find((opRef) => opRef.otherType.isAssignableFrom(rightType));

    return opRef?.resultType;
  }

  public findPrefixOperator(
    op: Operator,
    leftType: RealizedType,
  ): TO2Type | undefined {
    const opRef = this.prefixOperators
      .get(op)
      ?.find((opRef) => opRef.otherType.isAssignableFrom(leftType));

    return opRef?.resultType;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    const field = this.itemTypes.find((item) => item[0].value === name);

    if (!field) return undefined;

    return {
      definition: { moduleName: this.moduleName, range: field[0].range },
      value: field[1],
    };
  }

  public allFieldNames(): string[] {
    return this.itemTypes.map((item) => item[0].value);
  }

  public findMethod(name: string): WithDefinitionRef<FunctionType> | undefined {
    return this.methods.get(name);
  }

  public allMethodNames(): string[] {
    return [...this.methods.keys()];
  }

  public forInSource(): TO2Type | undefined {
    return undefined;
  }

  public supportIndexAccess(): TO2Type | undefined {
    return undefined;
  }

  public setModuleName(moduleName: string, context: ModuleContext): void {
    this.moduleName = moduleName;
    this.itemTypes.forEach((item) =>
      item[1].setModuleName?.(moduleName, context),
    );
    this.methods?.forEach((f) => f.value.setModuleName?.(moduleName, context));
  }
}

export function isRecordType(node: RealizedType): node is RecordType {
  return node.kind === "Record";
}
