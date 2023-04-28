import { FunctionType } from "./function-type";
import { Registry } from "./registry";
import { TO2Module } from "./to2-module";
import { RealizedType, TO2Type, findLibraryType } from "./to2-type";

export class ModuleContext {
  public readonly mappedConstants: Map<string, TO2Type> = new Map();
  public readonly mappedFunctions: Map<string, FunctionType> = new Map();

  constructor(public readonly registry: Registry) {}

  findType(
    namePath: string[],
    typeArguments: TO2Type[]
  ): RealizedType | undefined {
    return findLibraryType(namePath, typeArguments);
  }

  findConstant(name: string): TO2Type | undefined {
    return this.mappedConstants.get(name);
  }

  findFunction(name: string): FunctionType | undefined {
    return this.mappedFunctions.get(name);
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

  public findVariable(name: string): TO2Type | undefined {
    return (
      this.localVariables.get(name) ??
      this.parent?.findVariable(name) ??
      this.module.findConstant(name) ??
      this.module.findFunction(name)
    );
  }
}

export class FunctionContext extends BlockContext {
  constructor(module: ModuleContext) {
    super(module);
  }
}
