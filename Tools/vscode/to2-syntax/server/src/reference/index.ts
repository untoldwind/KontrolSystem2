import { Operator } from "../to2/ast/operator";
import referenceJson from "./reference.json";

export const REFERENCE: Reference = referenceJson as unknown as Reference;

export interface Reference {
  builtin: Record<string, TypeReference>;
  modules: Record<string, ModuleReference>;
}

export interface ModuleReference {
  name: string;
  description?: string;
  types: Record<string, TypeReference>;
  typeAliases: Record<string, TypeRef>;
  constants: Record<string, ConstantReference>;
  functions: Record<string, FunctionReference>;
}

export interface TypeReference {
  name: string;
  description?: string;
  genericParameters?: string[];
  fields: Record<string, FieldReference>;
  methods: Record<string, FunctionReference>;
  prefixOperators?: Record<Operator, OperatorReference[]>;
  suffixOperators?: Record<Operator, OperatorReference[]>;
  assignableFromAny: boolean;
  assignableFrom: TypeRef[];
}

export interface FieldReference {
  name: string;
  description?: string;
  readOnly: boolean;
  type: TypeRef;
}

export interface OperatorReference {
  op: Operator;
  otherType: TypeRef;
  resultType: TypeRef;
}

export interface FunctionReference {
  isAsync: boolean;
  name: string;
  description?: string;
  parameters: FunctionParameterReference[];
  returnType: TypeRef;
}

export interface FunctionParameterReference {
  name: string;
  type: TypeRef;
  hasDefault: boolean;
  description: string;
}

export interface ConstantReference {
  name: string;
  description?: string;
  type: TypeRef;
}

export type TypeRef =
  | {
      kind: "Builtin";
      name: string;
    }
  | {
      kind: "Array";
      parameters: [TypeRef];
    }
  | {
      kind: "Option";
      parameters: [TypeRef];
    }
  | {
      kind: "Result";
      parameters: [TypeRef, TypeRef];
    }
  | {
      kind: "Tuple";
      parameters: TypeRef[];
    }
  | {
      kind: "Record";
      names: string[];
      parameters: TypeRef[];
    }
  | {
      kind: "Function";
      parameters: TypeRef[];
      returnType: TypeRef;
      isAsync: boolean;
    }
  | {
      kind: "Generic";
      name: string;
    }
  | {
      kind: "Standard";
      module: string;
      name: string;
    };
