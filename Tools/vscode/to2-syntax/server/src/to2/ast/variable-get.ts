import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";

export class VariableGet extends Expression {
  constructor(
    public readonly namePath: string[],
    start: Position,
    end: Position
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}
