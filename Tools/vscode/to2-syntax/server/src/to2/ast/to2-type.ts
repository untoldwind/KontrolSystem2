import { REFERENCE, TypeRef, TypeReference } from "../../reference";
import { ModuleContext } from "./context";
import { Operator } from "./operator";

export interface TO2Type {
  name: string;
  localName: string;

  realizedType(context: ModuleContext): RealizedType;
}

export interface RealizedType extends TO2Type {
  isFunction?: boolean;
  description: string;

  isAssignableFrom(otherType: RealizedType): boolean;

  findSuffixOperator(
    op: Operator,
    rightType: RealizedType
  ): RealizedType | undefined;

  findPrefixOperator(
    op: Operator,
    leftType: RealizedType
  ): RealizedType | undefined;
}

export class ReferencedType implements RealizedType {
  public readonly name: string;
  public readonly localName: string;
  public readonly description: string;

  constructor(
    private readonly typeReference: TypeReference,
    moduleName?: string
  ) {
    this.localName = typeReference.name;
    this.name = moduleName
      ? `${moduleName}::${typeReference.name}`
      : typeReference.name;
    this.description = typeReference.description || "";
  }

  public isAssignableFrom(otherType: RealizedType): boolean {
    if (this.name === otherType.name) return true;
    for (const typeRef of this.typeReference.assignableFrom) {
      if (otherType.name === resolveTypeRef(typeRef)?.name) return true;
    }
    return false;
  }

  public realizedType(): RealizedType {
    return this;
  }

  public findSuffixOperator(
    op: Operator,
    rightType: RealizedType
  ): RealizedType | undefined {
    const opRef = this.typeReference.suffixOperators?.[op]?.find((opRef) =>
      resolveTypeRef(opRef.otherType)?.isAssignableFrom(rightType)
    );

    return opRef ? resolveTypeRef(opRef.resultType) : undefined;
  }

  public findPrefixOperator(
    op: Operator,
    leftType: RealizedType
  ): RealizedType | undefined {
    const opRef = this.typeReference.suffixOperators?.[op]?.find((opRef) =>
      resolveTypeRef(opRef.otherType)?.isAssignableFrom(leftType)
    );

    return opRef ? resolveTypeRef(opRef.resultType) : undefined;
  }
}

const referencedTypes: Record<string, ReferencedType> = [
  ...Object.values(REFERENCE.builtin).map(
    (typeReference) => new ReferencedType(typeReference)
  ),
  ...Object.values(REFERENCE.modules).flatMap((module) =>
    Object.values(module.types).map(
      (typeReference) => new ReferencedType(typeReference, module.name)
    )
  ),
].reduce((acc, referencedType) => {
  acc[referencedType.name] = referencedType;
  return acc;
}, {} as Record<string, ReferencedType>);

export const UNKNOWN_TYPE: RealizedType = {
  name: "<unknown>",
  localName: "<unknown>",
  description: "Undeterminded type",

  isAssignableFrom(): boolean {
    return false;
  },

  realizedType(): RealizedType {
    return this;
  },

  findSuffixOperator() {
    return undefined;
  },

  findPrefixOperator() {
    return undefined;
  },
};

export const BUILTIN_UNIT = new ReferencedType(REFERENCE.builtin["Unit"]);
export const BUILTIN_BOOL = new ReferencedType(REFERENCE.builtin["bool"]);
export const BUILTIN_INT = new ReferencedType(REFERENCE.builtin["int"]);
export const BUILTIN_FLOAT = new ReferencedType(REFERENCE.builtin["float"]);
export const BUILTIN_STRING = new ReferencedType(REFERENCE.builtin["string"]);
export const BUILTIN_RANGE = new ReferencedType(REFERENCE.builtin["Range"]);

export function findLibraryType(
  namePath: string[],
  typeArguments: TO2Type[]
): RealizedType | undefined {
  const fullName = namePath.join("::");
  return referencedTypes[fullName];
}

export function resolveTypeRef(typeRef: TypeRef): RealizedType | undefined {
  switch (typeRef.kind) {
    case "Builtin":
      return findLibraryType([typeRef.name], []);
    case "Generic":
    case "Standard":
      return findLibraryType([typeRef.module, typeRef.name], []);
  }
}
