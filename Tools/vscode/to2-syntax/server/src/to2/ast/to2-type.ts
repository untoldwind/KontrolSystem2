export interface TO2Type {
    name: string
    description: string
    localName: string
}

export const BUILTIN_BOOL : TO2Type = {
    name: "bool",
    description: "boolean",
    localName: "bool",
};

export const BUILTIN_INT : TO2Type = {
    name: "int",
    description: "integer",
    localName: "int",
};

export const BUILTIN_FLOAT : TO2Type = {
    name: "float",
    description: "floating point",
    localName: "float",
};

export const BUILTIN_STRING : TO2Type = {
    name: "string",
    description: "character string",
    localName: "string",
};

export const BUILTIN_UNIT : TO2Type = {
    name: "Unit",
    description: "unit type",
    localName: "Unit",
};
