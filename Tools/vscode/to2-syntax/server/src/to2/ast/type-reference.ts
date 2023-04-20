import { Position } from "vscode-languageserver-textdocument";
import { TO2Type } from "./to2-type";
import { Node } from ".";

export class LookupTypeReference implements Node, TO2Type {
  public name: string;
  public description: string;
  public localName: string;

  constructor(
    public readonly namePath: string[],
    public readonly typeArguments: TO2Type[],
    public readonly start: Position,
    public readonly end: Position
  ) {
    this.name = namePath.join("::");
    this.description = "";
    this.localName = namePath.join("::");
  }
}
