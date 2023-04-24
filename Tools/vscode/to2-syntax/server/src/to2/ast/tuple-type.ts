import { RealizedType, TO2Type } from "./to2-type";

export class TupleType implements RealizedType {
  public name: string;
  public localName: string;
  public description: string;

  constructor(public readonly itemTypes: TO2Type[]) {
    this.name = this.localName = `(${itemTypes
      .map((item) => item.name)
      .join(", ")})`;
    this.description = "";
  }
}
