export interface TO2Type {
  name: string;
  localName: string;
}

export interface RealizedType extends TO2Type {
  description: string;
}

export const BUILTIN_BOOL: RealizedType = {
  name: "bool",
  description: "boolean",
  localName: "bool",
};

export const BUILTIN_INT: RealizedType = {
  name: "int",
  description: "integer",
  localName: "int",
};

export const BUILTIN_FLOAT: RealizedType = {
  name: "float",
  description: "floating point",
  localName: "float",
};

export const BUILTIN_STRING: RealizedType = {
  name: "string",
  description: "character string",
  localName: "string",
};

export const BUILTIN_UNIT: RealizedType = {
  name: "Unit",
  description: "unit type",
  localName: "Unit",
};

export function getBuiltinType(
  namePath: string[],
  typeArguments: TO2Type[]
): TO2Type | undefined {
  return undefined;
}
