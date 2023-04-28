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
  name: string;
  description?: string;
  parameters: FunctionParameterReference[];
  returnType: TypeRef;
}

export interface FunctionParameterReference {
  name: string;
  type: TypeRef;
  hasDefault: boolean;
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
      kind: "Generic";
      module: string;
      name: string;
    }
  | {
      kind: "Standard";
      module: string;
      name: string;
    };
