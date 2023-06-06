import { REFERENCE } from "../../reference";
import { ReferencedModule, TO2Module, TO2ModuleNode } from "./to2-module";

export class Registry {
  public readonly modules: Map<string, TO2Module> = new Map();

  constructor() {
    for (const moduleReference of Object.values(REFERENCE.modules)) {
      const module = new ReferencedModule(moduleReference);
      this.modules.set(module.name, module);
    }
  }

  public findModule(namePath: string[]): TO2Module | undefined {
    return this.modules.get(namePath.join("::"));
  }

  public allModuleNames(): string[] {
    return [...this.modules.keys()];
  }
}
