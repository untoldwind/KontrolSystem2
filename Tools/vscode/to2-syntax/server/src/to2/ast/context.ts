import { FunctionType } from "./function-type";
import { TO2Type } from "./to2-type";

export class ModuleContext {
  public readonly mappedConstants: Map<string, TO2Type> = new Map();
  public readonly mappedFunctions: Map<string, FunctionType> = new Map();
}

export class BlockContext {
  public readonly localVariables: Map<string, TO2Type> = new Map();

  constructor(public readonly module: ModuleContext, private readonly parent: BlockContext | undefined = undefined) {}
}

export class FunctionContext extends BlockContext {
  constructor(module: ModuleContext) {
    super(module);
  }
}
