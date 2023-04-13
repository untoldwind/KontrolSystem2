import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";

export class LiteralBool implements Expression {
    public value: boolean;
    public start: Position;
    public end: Position;

    constructor(value: boolean, start: Position, end: Position) {
        this.value = value;
        this.start = start;
        this.end = end;
    }
}

export class LiteralInt implements Expression {
    public value: number;
    public start: Position;
    public end: Position;

    constructor(value: number, start: Position, end: Position) {
        this.value = value;
        this.start = start;
        this.end = end;
    }
}

export class LiteralFloat implements Expression {
    public value: number;
    public start: Position;
    public end: Position;

    constructor(value: number, start: Position, end: Position) {
        this.value = value;
        this.start = start;
        this.end = end;
    }
}

export class LiteralString implements Expression {
    public value: string;
    public start: Position;
    public end: Position;

    constructor(value: string, start: Position, end: Position) {
        this.value = value;
        this.start = start;
        this.end = end;
    }
}
