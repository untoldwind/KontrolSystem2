import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";

export class Unapply extends Expression {
  constructor(
    public readonly pattern: string,
    public readonly extractNames: string[],
    public readonly expression: Expression,
    start: Position,
    end: Position
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}
