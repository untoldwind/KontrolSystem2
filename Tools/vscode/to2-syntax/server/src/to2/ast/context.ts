import { FunctionType } from "./function-type";
import { RecordType } from "./record-type";
import { Registry } from "./registry";
import { TO2Module } from "./to2-module";
import { RealizedType, TO2Type, findLibraryTypeOrAlias } from "./to2-type";

export interface ModuleContext {
  mappedConstants: Map<string, TO2Type>;
  moduleAliases: Map<string, string[]>;
  mappedFunctions: Map<string, FunctionType>;
  typeAliases: Map<string, RealizedType>;

  findType(
    namePath: string[],
    typeArguments: RealizedType[]
  ): RealizedType | undefined;
  findConstant(namePath: string[]): TO2Type | undefined;
  findFunction(namePath: string[]): FunctionType | undefined;
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

  findFunction(namePath: string[]): FunctionType | undefined {
    if (namePath.length === 1 && this.mappedFunctions.has(namePath[0])) {
      return this.mappedFunctions.get(namePath[0]);
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

  findModule(namePath: string[]): TO2Module | undefined {
    return this.registry.findModule(namePath);
  }
}

export class ImplModuleContext implements ModuleContext {
  public readonly mappedConstants: Map<string, TO2Type> = new Map();
  public readonly mappedFunctions: Map<string, FunctionType> = new Map();
  public readonly moduleAliases: Map<string, string[]> = new Map();
  public readonly typeAliases: Map<string, RealizedType> = new Map();

  constructor(
    private readonly root: ModuleContext,
    public readonly structType: RecordType
  ) {
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

  findFunction(namePath: string[]): FunctionType | undefined {
    return this.root.findFunction(namePath);
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

  public findVariable(namePath: string[]): TO2Type | undefined {
    return (
      (namePath.length === 1
        ? this.localVariables.get(namePath[0])
        : undefined) ??
      this.parent?.findVariable(namePath) ??
      this.module.findConstant(namePath) ??
      this.module.findFunction(namePath)
    );
  }
}

export class FunctionContext extends BlockContext {
  constructor(module: ModuleContext, public readonly returnType: TO2Type) {
    super(module);
  }
}
