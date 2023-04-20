import { Position } from "vscode-languageserver-textdocument";
import { ModuleItem, Node } from ".";

export class UseDeclaration implements Node, ModuleItem {
  constructor(
    public readonly names: string[] | undefined,
    public readonly alias: string | undefined,
    public readonly moduleNamePath: string[],
    public readonly start: Position,
    public readonly end: Position
  ) {}
}
