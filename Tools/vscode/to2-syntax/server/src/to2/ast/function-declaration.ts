import { Position } from "vscode-languageserver-textdocument";
import { Expression, ModuleItem, Node } from ".";
import { TO2Type } from "./to2-type";

export enum FunctionModifier {
  Public,
  Private,
  Test,
}

export class FunctionParameter implements Node {
  constructor(
    public readonly name: string,
    public readonly type: TO2Type,
    public readonly defaultValue: Expression | undefined,
    public readonly start: Position,
    public readonly end: Position
  ) {}
}

export class FunctionDeclaration implements Node, ModuleItem {
  constructor(
    public readonly modifier: FunctionModifier,
    public readonly isAsync: boolean,
    public readonly name: string,
    public readonly description: string,
    public readonly parameters: FunctionParameter[],
    public readonly declaredReturn: TO2Type,
    public readonly expression: Expression,
    public readonly start: Position,
    public readonly end: Position
  ) {}
}
