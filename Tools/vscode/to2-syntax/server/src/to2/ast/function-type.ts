import { RealizedType, TO2Type } from "./to2-type";

export class FunctionType implements RealizedType {
  public name: string;
  public description: string;
  public localName: string;
  constructor(
    private readonly isAsync: boolean,
    private readonly parameterTypes: TO2Type[],
    private readonly returnType: TO2Type
  ) {
    this.name = `${isAsync ? "" : "sync "}fn(${parameterTypes.join(
      ", "
    )}) -> ${returnType}`;
    this.description = "";
    this.localName = this.name;
  }
}
