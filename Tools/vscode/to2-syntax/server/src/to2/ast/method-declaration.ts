import { Expression, Node } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class MethodDeclaration implements Node {
  constructor(
    public readonly isAsync: boolean,
    public readonly name: string,
    public readonly description: string,
    public readonly declaredReturn: TO2Type,
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}
}
