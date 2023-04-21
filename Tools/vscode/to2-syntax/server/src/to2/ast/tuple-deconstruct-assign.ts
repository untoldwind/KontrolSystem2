import { BlockItem, Expression, Node } from ".";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export interface TupleTarget {
  target: string;
  source: string;
}

export class TupleDeconstructAssign implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(
    public readonly targets: TupleTarget[],
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}
  resultType(): TO2Type {
    return this.expression.resultType();
  }
}
