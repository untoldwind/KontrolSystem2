import { REFERENCE, TypeReference } from "../../reference";
import { OptionType } from "./option-type";
import { ResultType } from "./result-type";

export interface TO2Type {
  name: string;
  localName: string;
}

export interface RealizedType extends TO2Type {
  description: string;
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

export const UNKNOWN_TYPE : RealizedType = {
  name: "<unknown>",
  localName: "<unknown>",
  description: "Undeterminded type",
};

export const BUILTIN_UNIT = new ReferencedType(REFERENCE.builtin["Unit"]);
export const BUILTIN_BOOL = new ReferencedType(REFERENCE.builtin["bool"]);
export const BUILTIN_INT = new ReferencedType(REFERENCE.builtin["int"]);
export const BUILTIN_FLOAT = new ReferencedType(REFERENCE.builtin["float"]);
export const BUILTIN_STRING = new ReferencedType(REFERENCE.builtin["string"]);
export const BUILTIN_RANGE = new ReferencedType(REFERENCE.builtin["Range"]);

export function findType(
  namePath: string[],
  typeArguments: TO2Type[]
): TO2Type | undefined {
  const fullName = namePath.join("::");
  return referencedTypes[fullName];
}
