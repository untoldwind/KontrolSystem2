import { Position } from "vscode-languageserver-textdocument";
import { Expression } from ".";
import {
  BUILTIN_BOOL,
  BUILTIN_FLOAT,
  BUILTIN_INT,
  BUILTIN_STRING,
  TO2Type,
} from "./to2-type";

export class LiteralBool implements Expression {
  constructor(
    public readonly value: boolean,
    public readonly start: Position,
    public readonly end: Position
  ) {}

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }
}

export class LiteralInt implements Expression {
  constructor(
    public readonly value: number,
    public readonly start: Position,
    public readonly end: Position
  ) {}

  resultType(): TO2Type {
    return BUILTIN_INT;
  }
}

export class LiteralFloat implements Expression {
  constructor(
    public readonly value: number,
    public readonly start: Position,
    public readonly end: Position
  ) {}

  resultType(): TO2Type {
    return BUILTIN_FLOAT;
  }
}

export class LiteralString implements Expression {
  constructor(
    public readonly value: string,
    public readonly start: Position,
    public readonly end: Position
  ) {}

  resultType(): TO2Type {
    return BUILTIN_STRING;
  }
}
