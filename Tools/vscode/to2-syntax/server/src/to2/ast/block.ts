import { BlockItem, Expression } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";

export class Block extends Expression {
  public isComment: boolean = false;

  constructor(
    public readonly items: BlockItem[],
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }
  resultType(): TO2Type {
    return (
      this.items
        .filter((item) => !item.isComment)
        .pop()
        ?.resultType() ?? BUILTIN_UNIT
    );
  }
}
