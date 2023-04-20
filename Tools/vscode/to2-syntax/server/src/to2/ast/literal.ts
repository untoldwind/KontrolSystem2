import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import {
  BUILTIN_BOOL,
  BUILTIN_FLOAT,
  BUILTIN_INT,
  BUILTIN_STRING,
  TO2Type,
} from "./to2-type";

export class LiteralBool extends Expression {
  constructor(public readonly value: boolean, start: Position, end: Position) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}

export class LiteralInt extends Expression {
  constructor(public readonly value: number, start: Position, end: Position) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_INT;
  }
}

export class LiteralFloat extends Expression {
  constructor(public readonly value: number, start: Position, end: Position) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_FLOAT;
  }
}

export class LiteralString extends Expression {
  constructor(public readonly value: string, start: Position, end: Position) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_STRING;
  }
}
