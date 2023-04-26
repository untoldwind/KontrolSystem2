import { RealizedType, TO2Type } from "./to2-type";

export class ResultType implements RealizedType {
  public name: string;
  public localName: string;
  public description: string;

  constructor(
    public readonly successType: TO2Type,
    public readonly errorType: TO2Type
  ) {
    this.name = this.localName = `Result<${successType}, ${errorType}>`;
    this.description = "";
  }
}
