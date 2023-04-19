import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import { Operator } from "./operator";
import { BUILTIN_BOOL, TO2Type } from "./to2-type";

export class Binary implements Expression {
    constructor(public readonly left: Expression, public readonly op: Operator, public readonly right: Expression, public readonly start: Position, public readonly end: Position) {
    }

    resultType(): TO2Type {
        return BUILTIN_BOOL;
    }
}
