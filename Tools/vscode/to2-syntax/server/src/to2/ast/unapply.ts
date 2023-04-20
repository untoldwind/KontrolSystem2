import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";

export class Unapply implements Expression {
  constructor(
    public readonly pattern: string,
    public readonly extractNames: string[],
    expression: Expression,
    public readonly start: Position,
    public readonly end: Position
  ) {}

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}
