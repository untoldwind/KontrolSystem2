import { Operator } from "../to2/ast/operator";
import referenceJson from "./reference.json";
import { z } from "zod";

export const REFERENCE: Reference = referenceJson as unknown as Reference;

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
      parameters: [TypeRef];
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

export const TypeRef: z.ZodSchema<TypeRef> = z.lazy(() =>
  z.union([
    z.object({
      kind: z.literal("Builtin"),
      name: z.string(),
    }),
    z.object({
      kind: z.literal("Array"),
      parameters: z.tuple([TypeRef]),
    }),
    z.object({
      kind: z.literal("Option"),
      parameters: z.tuple([TypeRef]),
    }),
    z.object({
      kind: z.literal("Result"),
      parameters: z.tuple([TypeRef]),
    }),
    z.object({
      kind: z.literal("Tuple"),
      parameters: z.array(TypeRef),
    }),
    z.object({
      kind: z.literal("Record"),
      names: z.array(z.string()),
      parameters: z.array(TypeRef),
    }),
    z.object({
      kind: z.literal("Function"),
      parameters: z.array(TypeRef),
      returnType: TypeRef,
      isAsync: z.boolean(),
    }),
    z.object({
      kind: z.literal("Generic"),
      name: z.string(),
    }),
    z.object({
      kind: z.literal("Standard"),
      module: z.string(),
      name: z.string(),
    }),
  ]),
);

export const ConstantReference = z.object({
  name: z.string(),
  description: z.string().optional(),
  type: TypeRef,
});

export type ConstantReference = z.infer<typeof ConstantReference>;

export const FunctionParameterReference = z.object({
  name: z.string(),
  type: TypeRef,
  hasDefault: z.boolean(),
  description: z.string().optional(),
});

export type FunctionParameterReference = z.infer<
  typeof FunctionParameterReference
>;

export const FunctionReference = z.object({
  isAsync: z.boolean(),
  name: z.string(),
  description: z.string().optional(),
  parameters: z.array(FunctionParameterReference),
  returnType: TypeRef,
});

export type FunctionReference = z.infer<typeof FunctionReference>;

export const OperatorReference = z.object({
  op: Operator,
  otherType: TypeRef,
  resultType: TypeRef,
});

export type OperatorReference = z.infer<typeof Operator>;

export const FieldReference = z.object({
  name: z.string(),
  description: z.string().optional(),
  readOnly: z.boolean(),
  type: TypeRef,
});

export type FieldReference = z.infer<typeof FieldReference>;

export const TypeReference = z.object({
  name: z.string(),
  description: z.string().optional(),
  genericParameters: z.array(z.string()).optional(),
  fields: z.record(FieldReference),
  methods: z.record(FunctionReference),
  prefixOperators: z.record(Operator, z.array(OperatorReference)).optional(),
  suffixOperators: z.record(Operator, z.array(OperatorReference)).optional(),
  assignableFromAny: z.boolean(),
  assignableFrom: z.array(TypeRef),
  forInSource: TypeRef.optional(),
  indexAccess: TypeRef.optional(),
});

export type TypeReference = z.infer<typeof TypeReference>;

export const ModuleReference = z.object({
  name: z.string(),
  description: z.string().optional(),
  types: z.record(TypeReference),
  typeAliases: z.record(TypeRef),
  constants: z.record(ConstantReference),
  functions: z.record(FunctionReference),
});

export type ModuleReference = z.infer<typeof ModuleReference>;

export const Reference = z.object({
  builtin: z.record(TypeReference),
  modules: z.record(ModuleReference),
});

export type Reference = z.infer<typeof Reference>;
