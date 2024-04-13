import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import { RealizedType, TO2Type, isRealizedType } from "./to2-type";

export class BoundValueType implements RealizedType {
  public readonly kind = "Bound";
  public readonly elementType: TO2Type;
  public name: string;
  public localName: string;
  public description: string;

  constructor(element: TO2Type) {
    this.elementType = element;
    this.name = this.localName = `Bound<${this.elementType.localName}>`;
    this.description = "";
  }

  public hasGenerics(context: ModuleContext): boolean {
    return this.elementType.realizedType(context).hasGenerics(context);
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    return this.name === otherType.name;
  }

  public realizedType(context: ModuleContext): RealizedType {
    return new BoundValueType(this.elementType.realizedType(context));
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new BoundValueType(
      this.elementType.realizedType(context).fillGenerics(context, genericMap),
    );
  }

  public fillGenericArguments(typeParameters: RealizedType[]): RealizedType {
    return typeParameters.length == 1
      ? new BoundValueType(typeParameters[0])
      : this;
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {
    if (isBouldValueType(realizedType)) {
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
    return isRealizedType(this.elementType)
      ? this.elementType.findSuffixOperator(op, rightType)
      : undefined;
  }

  public findPrefixOperator(
    op: Operator,
    leftType: RealizedType,
  ): TO2Type | undefined {
    return isRealizedType(this.elementType)
      ? this.elementType.findPrefixOperator(op, leftType)
      : undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    return isRealizedType(this.elementType)
      ? this.elementType.findField(name)
      : undefined;
  }

  public allFieldNames(): string[] {
    return isRealizedType(this.elementType)
      ? this.elementType.allFieldNames()
      : [];
  }

  public findMethod(name: string): WithDefinitionRef<FunctionType> | undefined {
    return isRealizedType(this.elementType)
      ? this.elementType.findMethod(name)
      : undefined;
  }

  public allMethodNames(): string[] {
    return isRealizedType(this.elementType)
      ? this.elementType.allMethodNames()
      : [];
  }

  public forInSource(): TO2Type | undefined {
    return undefined;
  }

  public supportIndexAccess(): TO2Type | undefined {
    return undefined;
  }

  public setModuleName(moduleName: string, context: ModuleContext): void {
    this.elementType.setModuleName?.(moduleName, context);
  }
}

export function isBouldValueType(node: RealizedType): node is BoundValueType {
  return node.kind === "Bound";
}
