import { UNKNOWN_RANGE } from "../../parser";
import { REFERENCE, Reference, TypeRef } from "../../reference";
import { ArrayType } from "./array-type";
import { ModuleContext } from "./context";
import { WithDefinitionRef } from "./definition-ref";
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

  setModuleName?(moduleName: string, context: ModuleContext): void;
}

export interface RealizedType extends TO2Type {
  kind:
    | "Unknown"
    | "Generic"
    | "Standard"
    | "Function"
    | "Array"
    | "Tuple"
    | "Record"
    | "Result"
    | "Option";
  description: string;

  hasGnerics(context: ModuleContext): boolean;

  isAssignableFrom(otherType: RealizedType): boolean;

  findSuffixOperator(
    op: Operator,
    rightType: RealizedType,
  ): TO2Type | undefined;

  findPrefixOperator(op: Operator, leftType: RealizedType): TO2Type | undefined;

  findField(name: string): WithDefinitionRef<TO2Type> | undefined;

  allFieldNames(): string[];

  findMethod(name: string): WithDefinitionRef<FunctionType> | undefined;

  allMethodNames(): string[];

  forInSource(): TO2Type | undefined;

  supportIndexAccess(): TO2Type | undefined;

  fillGenerics(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType;

  guessGeneric(
    context: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ): void;
}

export function isRealizedType(type: TO2Type): type is RealizedType {
  return (type as RealizedType).kind !== undefined;
}

export class GenericParameter implements RealizedType {
  public readonly kind = "Generic";
  public readonly description = "";
  public readonly localName: string;

  constructor(public readonly name: string) {
    this.localName = name;
  }

  hasGnerics(_: ModuleContext): boolean {
    return true;
  }

  isAssignableFrom(_: RealizedType): boolean {
    return true;
  }

  findSuffixOperator(_: Operator): TO2Type | undefined {
    return undefined;
  }

  public findPrefixOperator(): RealizedType | undefined {
    return undefined;
  }

  findField(name: string): WithDefinitionRef<TO2Type> | undefined {
    return undefined;
  }

  allFieldNames(): string[] {
    return [];
  }

  findMethod(name: string): WithDefinitionRef<FunctionType> | undefined {
    return undefined;
  }

  allMethodNames(): string[] {
    return [];
  }

  forInSource(): TO2Type | undefined {
    return undefined;
  }

  supportIndexAccess(): TO2Type | undefined {
    return undefined;
  }

  realizedType(_: ModuleContext): RealizedType {
    return this;
  }

  fillGenerics(
    _: ModuleContext,
    genericMap: Record<string, RealizedType>,
  ): RealizedType {
    return genericMap.hasOwnProperty(this.name) ? genericMap[this.name] : this;
  }

  guessGeneric(
    _: ModuleContext,
    genericMap: Record<string, RealizedType>,
    realizedType: RealizedType,
  ) {
    if (realizedType === UNKNOWN_TYPE) return;
    if (isGenericParameter(realizedType)) {
      if (genericMap.hasOwnProperty(realizedType.name)) {
        genericMap[this.name] = genericMap[realizedType.name];
      }
    } else {
      genericMap[this.name] = realizedType;
    }
  }
}

export function isGenericParameter(
  node: RealizedType | undefined,
): node is GenericParameter {
  return node !== undefined && node.kind === "Generic";
}

export const UNKNOWN_TYPE: RealizedType = {
  kind: "Unknown",
  name: "<unknown>",
  localName: "<unknown>",
  description: "Undeterminded type",

  hasGnerics(context: ModuleContext): boolean {
    return false;
  },

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

  supportIndexAccess() {
    return undefined;
  },

  fillGenerics() {
    return this;
  },

  guessGeneric() {},
};

export class TypeResolver {
  private referencedTypes: Record<string, ReferencedType>;
  private referencedTypeAliases: Record<string, TypeRef>;
  public BUILTIN_UNIT: ReferencedType;
  public BUILTIN_BOOL: ReferencedType;
  public BUILTIN_INT: ReferencedType;
  public BUILTIN_FLOAT: ReferencedType;
  public BUILTIN_STRING: ReferencedType;
  public BUILTIN_RANGE: ReferencedType;
  public BUILTIN_CELL: ReferencedType;
  public BUILTIN_ERROR: ReferencedType;
  public BUILTIN_ARRAYBUILDER: ReferencedType;

  constructor(private reference: Reference) {
    this.referencedTypes = [
      ...Object.values(REFERENCE.builtin).map(
        (typeReference) => new ReferencedType(typeReference),
      ),
      ...Object.values(REFERENCE.modules).flatMap((module) =>
        Object.values(module.types).map(
          (typeReference) => new ReferencedType(typeReference, module.name),
        ),
      ),
    ].reduce(
      (acc, referencedType) => {
        acc[referencedType.name] = referencedType;
        return acc;
      },
      {} as Record<string, ReferencedType>,
    );

    this.referencedTypeAliases = Object.values(REFERENCE.modules)
      .flatMap((module) =>
        Object.entries(module.typeAliases).map(([name, typeRef]) => ({
          name: `${module.name}::${name}`,
          typeRef,
        })),
      )
      .reduce(
        (acc, { name, typeRef }) => {
          acc[name] = typeRef;
          return acc;
        },
        {} as Record<string, TypeRef>,
      );

    this.BUILTIN_UNIT = new ReferencedType(reference.builtin["Unit"]);
    this.BUILTIN_BOOL = new ReferencedType(reference.builtin["bool"]);
    this.BUILTIN_INT = new ReferencedType(reference.builtin["int"]);
    this.BUILTIN_FLOAT = new ReferencedType(reference.builtin["float"]);
    this.BUILTIN_STRING = new ReferencedType(reference.builtin["string"]);
    this.BUILTIN_RANGE = new ReferencedType(reference.builtin["Range"]);
    this.BUILTIN_CELL = new ReferencedType(reference.builtin["Cell"]);
    this.BUILTIN_ERROR = new ReferencedType(
      reference.modules["core::error"].types["Error"],
    );
    this.BUILTIN_ARRAYBUILDER = new ReferencedType(
      reference.builtin["ArrayBuilder"],
    );
  }

  public findLibraryTypeOrAlias(
    namePath: string[],
    typeArguments: RealizedType[],
  ): RealizedType | undefined {
    const aliased = this.referencedTypeAliases[namePath.join("::")];
    if (aliased) return this.resolveTypeRef(aliased);
    return this.findLibraryType(namePath, typeArguments);
  }

  public findLibraryType(
    namePath: string[],
    typeArguments: RealizedType[],
  ): RealizedType | undefined {
    const fullName = namePath.join("::");
    switch (fullName) {
      case "Option":
        if (typeArguments.length === 1) return new OptionType(typeArguments[0]);
        break;
      case "Result":
        if (typeArguments.length === 1 || typeArguments.length === 2)
          return new ResultType(typeArguments[0]);
        break;
    }

    return this.referencedTypes[fullName]?.fillGenericArguments(typeArguments);
  }

  public resolveTypeRef(
    typeRef: TypeRef,
    genericMap?: Record<string, RealizedType>,
  ): RealizedType | undefined {
    switch (typeRef.kind) {
      case "Builtin":
        return this.findLibraryType([typeRef.name], []);
      case "Generic":
        return genericMap?.[typeRef.name] ?? new GenericParameter(typeRef.name);
      case "Standard":
        return this.findLibraryType([typeRef.module, typeRef.name], []);
      case "Array":
        return new ArrayType(
          this.resolveTypeRef(typeRef.parameters[0]) ?? UNKNOWN_TYPE,
        );
      case "Option":
        return new OptionType(
          this.resolveTypeRef(typeRef.parameters[0]) ?? UNKNOWN_TYPE,
        );
      case "Result":
        return new ResultType(
          this.resolveTypeRef(typeRef.parameters[0]) ?? UNKNOWN_TYPE,
        );
      case "Tuple":
        return new TupleType(
          typeRef.parameters.map(
            (param) => this.resolveTypeRef(param) ?? UNKNOWN_TYPE,
          ),
        );
      case "Record":
        return new RecordType(
          typeRef.parameters.map((param, idx) => [
            { range: UNKNOWN_RANGE, value: typeRef.names[idx] },
            this.resolveTypeRef(param) ?? UNKNOWN_TYPE,
          ]),
        );
      case "Function":
        return new FunctionType(
          typeRef.isAsync,
          typeRef.parameters.map((param, idx) => [
            `param${idx}`,
            this.resolveTypeRef(param) ?? UNKNOWN_TYPE,
            false,
          ]),
          this.resolveTypeRef(typeRef.returnType) ?? UNKNOWN_TYPE,
        );
    }
  }
}

var typeResolverInstance: TypeResolver | undefined = undefined;

export function currentTypeResolver(): TypeResolver {
  if (typeResolverInstance === undefined)
    console.log("Init type resolver from default");
  typeResolverInstance ??= new TypeResolver(REFERENCE);
  return typeResolverInstance;
}

export function typeResolverInitialized(): boolean {
  return typeResolverInstance !== undefined;
}

export function initTypeResolver(reference: Reference) {
  console.log("Init type resolver from mod");
  typeResolverInstance = new TypeResolver(reference);
}
