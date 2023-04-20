import { ModuleItem } from ".";

export class TO2Module {
  constructor(
    public readonly name: string,
    public readonly description: string,
    public readonly items: ModuleItem[]
  ) {}
}
