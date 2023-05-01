import { REFERENCE, TypeRef, TypeReference } from "../../reference";
import { ArrayType } from "./array-type";
import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import { OptionType } from "./option-type";
import { RecordType } from "./record-type";
import { ResultType } from "./result-type";
import { TupleType } from "./tuple-type";

export interface TO2Type {
  name: string;
  localName: string;

  realizedType(context: ModuleContext): RealizedType;
}

export interface RealizedType extends TO2Type {
  kind:
    | "Unknown"
    | "Standard"
    | "Function"
    | "Array"
    | "Tuple"
    | "Record"
    | "Result"
    | "Option";
  description: string;

  isAssignableFrom(otherType: RealizedType): boolean;

  findSuffixOperator(
    op: Operator,
    rightType: RealizedType
  ): TO2Type | undefined;

  findPrefixOperator(op: Operator, leftType: RealizedType): TO2Type | undefined;

  findField(name: string): TO2Type | undefined;

  findMethod(name: string): FunctionType | undefined;
}

export class ReferencedType implements RealizedType {
  public readonly kind = "Standard";
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
  ): TO2Type | undefined {
    const opRef = this.typeReference.suffixOperators?.[op]?.find((opRef) =>
      resolveTypeRef(opRef.otherType)?.isAssignableFrom(rightType)
    );

    return opRef ? resolveTypeRef(opRef.resultType) : undefined;
  }

  public findPrefixOperator(
    op: Operator,
    leftType: RealizedType
  ): TO2Type | undefined {
    const opRef = this.typeReference.suffixOperators?.[op]?.find((opRef) =>
      resolveTypeRef(opRef.otherType)?.isAssignableFrom(leftType)
    );

    return opRef ? resolveTypeRef(opRef.resultType) : undefined;
  }

  public findField(name: string): TO2Type | undefined {
    const fieldReference = this.typeReference.fields[name];
    if (!fieldReference) return undefined;

    return resolveTypeRef(fieldReference.type);
  }

  public findMethod(name: string): FunctionType | undefined {
    const methodReference = this.typeReference.methods[name];
    if (!methodReference) return undefined;

    return new FunctionType(
      methodReference.isAsync,
      methodReference.parameters.map((paramRef) => [
        paramRef.name,
        resolveTypeRef(paramRef.type) ?? UNKNOWN_TYPE,
      ]),
      resolveTypeRef(methodReference.returnType) ?? UNKNOWN_TYPE,
      methodReference.description
    );
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
  kind: "Unknown",
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

  findField() {
    return undefined;
  },

  findMethod() {
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
    case "Array":
      return new ArrayType(
        resolveTypeRef(typeRef.parameters[0]) ?? UNKNOWN_TYPE
      );
    case "Option":
      return new OptionType(
        resolveTypeRef(typeRef.parameters[0]) ?? UNKNOWN_TYPE
      );
    case "Result":
      return new ResultType(
        resolveTypeRef(typeRef.parameters[0]) ?? UNKNOWN_TYPE,
        resolveTypeRef(typeRef.parameters[1]) ?? UNKNOWN_TYPE
      );
    case "Tuple":
      return new TupleType(
        typeRef.parameters.map((param) => resolveTypeRef(param) ?? UNKNOWN_TYPE)
      );
    case "Record":
      return new RecordType(
        typeRef.parameters.map((param, idx) => [
          typeRef.names[idx],
          resolveTypeRef(param) ?? UNKNOWN_TYPE,
        ])
      );
  }
}
