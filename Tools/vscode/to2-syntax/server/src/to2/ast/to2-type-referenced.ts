import { TypeReference } from "../../reference";
import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import {
  BUILTIN_INT,
  RealizedType,
  TO2Type,
  UNKNOWN_TYPE,
  resolveTypeRef,
} from "./to2-type";

export class ReferencedType implements RealizedType {
  public readonly kind = "Standard";
  public readonly name: string;
  public readonly localName: string;
  public readonly description: string;

  constructor(
    private readonly typeReference: TypeReference,
    private readonly moduleName?: string,
    private readonly genericMap?: Record<string, RealizedType>,
  ) {
    this.localName = typeReference.name;
    this.name = moduleName
      ? `${moduleName}::${typeReference.name}`
      : typeReference.name;
    if (typeReference.genericParameters) {
      this.localName += `<${typeReference.genericParameters
        .map((p) => genericMap?.[p]?.localName ?? p)
        .join(", ")}>`;
    }
    this.description = typeReference.description || "";
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    if (this.name === otherType.name || this.typeReference.assignableFromAny)
      return true;
    for (const typeRef of this.typeReference.assignableFrom) {
      if (otherType.name === resolveTypeRef(typeRef, this.genericMap)?.name)
        return true;
    }
    return false;
  }

  public realizedType(): RealizedType {
    return this;
  }

  public findSuffixOperator(
    op: Operator,
    rightType: RealizedType,
  ): TO2Type | undefined {
    const opRef = this.typeReference.suffixOperators?.[op]?.find(
      (opRef) =>
        resolveTypeRef(opRef.otherType, this.genericMap)?.isAssignableFrom(
          rightType,
        ),
    );

    return opRef
      ? resolveTypeRef(opRef.resultType, this.genericMap)
      : undefined;
  }

  public findPrefixOperator(
    op: Operator,
    leftType: RealizedType,
  ): TO2Type | undefined {
    const opRef = this.typeReference.prefixOperators?.[op]?.find(
      (opRef) =>
        resolveTypeRef(opRef.otherType, this.genericMap)?.isAssignableFrom(
          leftType,
        ),
    );

    return opRef
      ? resolveTypeRef(opRef.resultType, this.genericMap)
      : undefined;
  }

  public findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    const fieldReference = this.typeReference.fields[name];
    if (!fieldReference) return undefined;

    const resolved = resolveTypeRef(fieldReference.type, this.genericMap);
    return resolved ? { value: resolved } : undefined;
  }

  public allFieldNames(): string[] {
    return Object.keys(this.typeReference.fields);
  }

  public findMethod(name: string): WithDefinitionRef<FunctionType> | undefined {
    const methodReference = this.typeReference.methods[name];
    if (!methodReference) return undefined;

    return {
      value: new FunctionType(
        methodReference.isAsync,
        methodReference.parameters.map((paramRef) => [
          paramRef.name,
          resolveTypeRef(paramRef.type, this.genericMap) ?? UNKNOWN_TYPE,
          paramRef.hasDefault,
        ]),
        resolveTypeRef(methodReference.returnType, this.genericMap) ??
          UNKNOWN_TYPE,
        methodReference.description,
      ),
    };
  }

  public allMethodNames(): string[] {
    return Object.keys(this.typeReference.methods);
  }

  public forInSource(): TO2Type | undefined {
    return this.name === "Range" ? BUILTIN_INT : undefined;
  }

  public fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return new ReferencedType(this.typeReference, this.moduleName, {
      ...this.genericMap,
      ...genericMap,
    });
  }

  public fillGenericArguments(typeParameters: RealizedType[]): RealizedType {
    if (
      !this.typeReference.genericParameters ||
      this.typeReference.genericParameters.length !== typeParameters.length
    ) {
      return this;
    }
    const genericMap: Record<string, RealizedType> = {};
    for (let i = 0; i < typeParameters.length; i++) {
      genericMap[this.typeReference.genericParameters[i]] = typeParameters[i];
    }
    return new ReferencedType(this.typeReference, this.moduleName, genericMap);
  }

  public guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void {}
}
