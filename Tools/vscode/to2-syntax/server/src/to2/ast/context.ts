import { FunctionType } from "./function-type";
import { Registry } from "./registry";
import { TO2Module } from "./to2-module";
import { RealizedType, TO2Type, findLibraryType } from "./to2-type";

export class ModuleContext {
  public readonly mappedConstants: Map<string, TO2Type> = new Map();
  public readonly mappedFunctions: Map<string, FunctionType> = new Map();
  public readonly moduleAliases: Map<string, string[]> = new Map();
  public readonly typeAliases: Map<string, RealizedType> = new Map();

  constructor(
    public readonly registry: Registry,
    public readonly structContext?: TO2Type
  ) {}

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
        return findLibraryType([...mappedModule, namePath[1]], typeArguments);
      }
    }
    return findLibraryType(namePath, typeArguments);
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
