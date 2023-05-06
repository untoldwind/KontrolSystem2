import { FunctionType, isFunctionType } from "./function-type";
import { RecordType } from "./record-type";
import { Registry } from "./registry";
import { ResultType } from "./result-type";
import { TO2Module } from "./to2-module";
import {
  BUILTIN_ARRAYBUILDER,
  BUILTIN_CELL,
  BUILTIN_INT,
  RealizedType,
  TO2Type,
  UNKNOWN_TYPE,
  findLibraryTypeOrAlias,
} from "./to2-type";

export interface ModuleContext {
  mappedConstants: Map<string, TO2Type>;
  moduleAliases: Map<string, string[]>;
  mappedFunctions: Map<string, FunctionType>;
  typeAliases: Map<string, RealizedType>;
  registry: Registry;

  findType(
    namePath: string[],
    typeArguments: RealizedType[]
  ): RealizedType | undefined;
  findConstant(namePath: string[]): TO2Type | undefined;
  allConstants(): [string, TO2Type][];
  findFunction(
    namePath: string[],
    typeHint?: RealizedType
  ): FunctionType | undefined;
  allFunctions(): [string, FunctionType][];
  findModule(namePath: string[]): TO2Module | undefined;
}

export class RootModuleContext implements ModuleContext {
  public readonly mappedConstants: Map<string, TO2Type> = new Map();
  public readonly mappedFunctions: Map<string, FunctionType> = new Map();
  public readonly moduleAliases: Map<string, string[]> = new Map();
  public readonly typeAliases: Map<string, RealizedType> = new Map();

  constructor(public readonly registry: Registry) {}

  findType(
    namePath: string[],
    typeArguments: RealizedType[]
  ): RealizedType | undefined {
    if (namePath.length === 1 && this.typeAliases.has(namePath[0])) {
      return this.typeAliases.get(namePath[0]);
    }
    if (namePath.length === 2) {
      const mappedModule = this.moduleAliases.get(namePath[0]);
      if (mappedModule) {
        return findLibraryTypeOrAlias(
          [...mappedModule, namePath[1]],
          typeArguments
        );
      }
    }
    return findLibraryTypeOrAlias(namePath, typeArguments);
  }

  findConstant(namePath: string[]): TO2Type | undefined {
    if (namePath.length === 1 && this.mappedConstants.has(namePath[0])) {
      return this.mappedConstants.get(namePath[0]);
    }
    if (namePath.length === 2) {
      const mappedModule = this.moduleAliases.get(namePath[0]);
      if (mappedModule) {
        return this.findModule(mappedModule)?.findConstant(namePath[1]);
      }
    }
    return namePath.length > 2
      ? this.findModule(namePath.slice(0, namePath.length - 1))?.findConstant(
          namePath[namePath.length - 1]
        )
      : undefined;
  }

  allConstants(): [string, TO2Type][] {
    return [...this.mappedConstants.entries()];
  }

  findFunction(
    namePath: string[],
    typeHint?: RealizedType
  ): FunctionType | undefined {
    if (namePath.length === 1) {
      if (this.mappedFunctions.has(namePath[0])) {
        return this.mappedFunctions.get(namePath[0]);
      } else {
        const returnType =
          typeHint && isFunctionType(typeHint)
            ? typeHint.returnType
            : undefined;
        const args =
          typeHint && isFunctionType(typeHint)
            ? typeHint.parameterTypes.map((param) => param[1])
            : [];
        switch (namePath[0]) {
          case "Cell":
            if (args.length === 1) {
              return new FunctionType(
                false,
                [["value", args[0], false]],
                BUILTIN_CELL.fillGenerics([args[0].realizedType(this)])
              );
            }
            break;
          case "Ok":
            if (args.length === 1) {
              return new FunctionType(
                false,
                [["value", args[0], false]],
                new ResultType(args[0], UNKNOWN_TYPE)
              );
            }
            break;
          case "Err":
            if (args.length === 1) {
              return new FunctionType(
                false,
                [["value", args[0], false]],
                new ResultType(UNKNOWN_TYPE, args[0])
              );
            }
            break;
          case "ArrayBuilder":
            if (args.length === 1) {
              return new FunctionType(
                false,
                [["value", BUILTIN_INT, false]],
                BUILTIN_ARRAYBUILDER
              );
            }
            break;
        }
      }
    }
    if (namePath.length === 2) {
      const mappedModule = this.moduleAliases.get(namePath[0]);
      if (mappedModule) {
        return this.findModule(mappedModule)?.findFunction(namePath[1]);
      }
    }
    return namePath.length > 2
      ? this.findModule(namePath.slice(0, namePath.length - 1))?.findFunction(
          namePath[namePath.length - 1]
        )
      : undefined;
  }

  allFunctions(): [string, FunctionType][] {
    return [...this.mappedFunctions.entries()];
  }

  findModule(namePath: string[]): TO2Module | undefined {
    return this.registry.findModule(namePath);
  }
}

export class ImplModuleContext implements ModuleContext {
  public readonly mappedConstants: Map<string, TO2Type> = new Map();
  public readonly mappedFunctions: Map<string, FunctionType> = new Map();
  public readonly moduleAliases: Map<string, string[]> = new Map();
  public readonly typeAliases: Map<string, RealizedType> = new Map();
  public readonly registry: Registry;

  constructor(
    private readonly root: ModuleContext,
    public readonly structType: RecordType
  ) {
    this.registry = root.registry;
    this.mappedConstants = root.mappedConstants;
    this.mappedFunctions = root.mappedFunctions;
    this.moduleAliases = root.moduleAliases;
    this.typeAliases = root.typeAliases;
  }

  findType(
    namePath: string[],
    typeArguments: RealizedType[]
  ): RealizedType | undefined {
    return this.root.findType(namePath, typeArguments);
  }

  findConstant(namePath: string[]): TO2Type | undefined {
    return this.root.findConstant(namePath);
  }

  allConstants(): [string, TO2Type][] {
    return this.root.allConstants();
  }

  findFunction(
    namePath: string[],
    typeHint?: RealizedType
  ): FunctionType | undefined {
    return this.root.findFunction(namePath, typeHint);
  }

  allFunctions(): [string, FunctionType][] {
    return this.root.allFunctions();
  }

  findModule(namePath: string[]): TO2Module | undefined {
    return this.root.findModule(namePath);
  }
}

export function isImplModuleContext(
  context: ModuleContext
): context is ImplModuleContext {
  return (context as ImplModuleContext).structType !== undefined;
}

export class BlockContext {
  public readonly localVariables: Map<string, TO2Type> = new Map();

  constructor(
    public readonly module: ModuleContext,
    private readonly parent: BlockContext | undefined = undefined
  ) {}

  public findVariable(
    namePath: string[],
    typeHint?: RealizedType
  ): TO2Type | undefined {
    return (
      (namePath.length === 1
        ? this.localVariables.get(namePath[0])
        : undefined) ??
      this.parent?.findVariable(namePath, typeHint) ??
      this.module.findConstant(namePath) ??
      this.module.findFunction(namePath, typeHint)
    );
  }

  public allVariables(): [string, TO2Type][] {
    if (this.parent) {
      return [...this.parent.allVariables(), ...this.localVariables.entries()];
    }
    return [
      ...this.module.allConstants(),
      ...this.module.allFunctions(),
      ...this.localVariables.entries(),
    ];
  }
}

export class FunctionContext extends BlockContext {
  constructor(module: ModuleContext, public readonly returnType: TO2Type) {
    super(module);
  }
}
