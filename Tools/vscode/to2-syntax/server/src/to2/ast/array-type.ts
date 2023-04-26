import { RealizedType, TO2Type } from "./to2-type";

export class ArrayType implements RealizedType {
  public readonly elementType: TO2Type;
  public name: string;
  public localName: string;
  public description: string;

  constructor(element: TO2Type, dimension: number = 1) {
    this.elementType = dimension > 1 ? new ArrayType(element, dimension - 1) : element;
    this.name = this.localName = `${this.elementType}[]`;
    this.description = "";
  }
}
