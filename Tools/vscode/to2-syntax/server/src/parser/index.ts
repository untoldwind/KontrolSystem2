import { Position, Range } from "vscode-languageserver-textdocument";
import {
  SemanticToken,
  SemanticTokenType,
  SemanticTokenModifier,
} from "../syntax-token";

export class InputPosition implements Position {
  constructor(
    public readonly offset: number,
    public readonly line: number,
    public readonly character: number
  ) {}
}

export class InputRange implements Range {
  constructor(
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  semanticToken(
    type: SemanticTokenType,
    ...modifiers: SemanticTokenModifier[]
  ): SemanticToken {
    return {
      type,
      modifiers,
      start: this.start,
      length: this.end.offset - this.start.offset,
    };
  }
}

export interface Input {
  position: InputPosition;
  available: number;
  take(count: number): string;
  findNext(predicate: (charCode: number) => boolean): number;
  advance(count: number): Input;
}

export class ParserSuccess<T> {
  success: true = true;

  constructor(public readonly remaining: Input, public readonly value: T) {
    this.remaining = remaining;
    this.value = value;
  }

  map<U>(mapper: (result: T) => U): ParserResult<U> {
    return new ParserSuccess<U>(this.remaining, mapper(this.value));
  }
}

export class ParserFailure<T> {
  success: false = false;

  constructor(
    public readonly remaining: Input,
    public readonly expected: string,
    public readonly value: T | undefined
  ) {
    this.expected = expected;
  }

  map<U>(mapper: (result: T) => U): ParserResult<U> {
    return new ParserFailure<U>(
      this.remaining,
      this.expected,
      this.value ? mapper(this.value) : undefined
    );
  }
}

export type ParserResult<T> = ParserSuccess<T> | ParserFailure<T>;

export type Parser<T> = (input: Input) => ParserResult<T>;

export type WithPosition<T> = {
  value: T;
  range: InputRange;
};
