import { REFERENCE } from "../../reference";
import { ReferencedModule, TO2Module } from "./to2-module";

export class Registry {
  private readonly modules: Map<string, TO2Module> = new Map();

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
