import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { IndexSpec } from "./index-spec";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class IndexGet extends Expression {
  constructor(
    public readonly target: Expression,
    public readonly indexSpec: IndexSpec,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }

  public resultType(): TO2Type {
    return BUILTIN_UNIT;
  }
}
