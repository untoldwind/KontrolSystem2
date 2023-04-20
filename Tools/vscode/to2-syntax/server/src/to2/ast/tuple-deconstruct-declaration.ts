import { Position } from "vscode-languageserver-textdocument";
import { BlockItem, Expression, Node } from ".";
import { DeclarationParameter } from "./variable-declaration";
import { TO2Type } from "./to2-type";

export class TupleDeconstructDeclaration implements Node, BlockItem {
  public isComment: boolean = false;

  constructor(
    public readonly declarations: DeclarationParameter[],
    public readonly isConst: boolean,
    public readonly expression: Expression,
    public readonly start: Position,
    public readonly end: Position
  ) {}
  resultType(): TO2Type {
    return this.expression.resultType();
  }
}
