import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition } from "../../parser";
import { ModuleReference } from "../../reference";
import { ModuleContext } from "./context";
import { Registry } from "./registry";

export interface TO2Module {
  name: string
  description: string
}

export class TO2ModuleNode implements Node, TO2Module {
  constructor(
    public readonly name: string,
    public readonly description: string,
    public readonly items: ModuleItem[],
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.items.reduce(
      (prev, item) => item.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validate(registry: Registry): ValidationError[] {
    const context = new ModuleContext(registry);
    const errors: ValidationError[] = [];

    for (const item of this.items) {
      errors.push(...item.validateModule(context));
    }

    return errors;
  }
}

export class ReferencedModule  implements TO2Module {
  public readonly name: string;
  public readonly description: string

  constructor(private readonly moduleReference: ModuleReference) {
    this.name = moduleReference.name;
    this.description = moduleReference.description || "";
  }
}
