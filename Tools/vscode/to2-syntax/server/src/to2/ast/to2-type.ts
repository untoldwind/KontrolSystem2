import { UNKNOWN_RANGE } from "../../parser";
import { REFERENCE, TypeRef } from "../../reference";
import { ArrayType } from "./array-type";
import { ModuleContext } from "./context";
import { FunctionType } from "./function-type";
import { Operator } from "./operator";
import { OptionType } from "./option-type";
import { RecordType } from "./record-type";
import { ResultType } from "./result-type";
import { ReferencedType } from "./to2-type-referenced";
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

  allFieldNames(): string[];

  findMethod(name: string): FunctionType | undefined;

  allMethodNames(): string[];

  forInSource(): TO2Type | undefined;
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

const referencedTypeAliases = Object.values(REFERENCE.modules)
  .flatMap((module) =>
    Object.entries(module.typeAliases).map(([name, typeRef]) => ({
      name: `${module.name}::${name}`,
      typeRef,
    }))
  )
  .reduce((acc, { name, typeRef }) => {
    acc[name] = typeRef;
    return acc;
  }, {} as Record<string, TypeRef>);

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

  allFieldNames() {
    return [];
  },

  findMethod() {
    return undefined;
  },

  allMethodNames() {
    return [];
  },

  forInSource() {
    return undefined;
  },
};

export const BUILTIN_UNIT = new ReferencedType(REFERENCE.builtin["Unit"]);
export const BUILTIN_BOOL = new ReferencedType(REFERENCE.builtin["bool"]);
export const BUILTIN_INT = new ReferencedType(REFERENCE.builtin["int"]);
export const BUILTIN_FLOAT = new ReferencedType(REFERENCE.builtin["float"]);
export const BUILTIN_STRING = new ReferencedType(REFERENCE.builtin["string"]);
export const BUILTIN_RANGE = new ReferencedType(REFERENCE.builtin["Range"]);
export const BUILTIN_CELL = new ReferencedType(REFERENCE.builtin["Cell"]);
export const BUILTIN_ARRAYBUILDER = new ReferencedType(
  REFERENCE.builtin["ArrayBuilder"]
);

export function findLibraryTypeOrAlias(
  namePath: string[],
  typeArguments: RealizedType[]
): RealizedType | undefined {
  const aliased = referencedTypeAliases[namePath.join("::")];
  if (aliased) return resolveTypeRef(aliased);
  return findLibraryType(namePath, typeArguments);
}

export function findLibraryType(
  namePath: string[],
  typeArguments: RealizedType[]
): RealizedType | undefined {
  const fullName = namePath.join("::");
  switch (fullName) {
    case "Option":
      if (typeArguments.length === 1) return new OptionType(typeArguments[0]);
    case "Result":
      if (typeArguments.length === 2)
        return new ResultType(typeArguments[0], typeArguments[1]);
  }

  return referencedTypes[fullName]?.fillGenerics(typeArguments);
}

export function resolveTypeRef(
  typeRef: TypeRef,
  genericMap?: Record<string, RealizedType>
): RealizedType | undefined {
  switch (typeRef.kind) {
    case "Builtin":
      return findLibraryType([typeRef.name], []);
    case "Generic":
      return genericMap?.[typeRef.name] ?? UNKNOWN_TYPE;
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
          {
            range: UNKNOWN_RANGE,
            value: resolveTypeRef(param) ?? UNKNOWN_TYPE,
          },
        ])
      );
    case "Function":
      return new FunctionType(
        typeRef.isAsync,
        typeRef.parameters.map((param, idx) => [
          `param${idx}`,
          resolveTypeRef(param) ?? UNKNOWN_TYPE,
          false,
        ]),
        resolveTypeRef(typeRef.returnType) ?? UNKNOWN_TYPE
      );
  }
}
