import { Position } from "vscode-languageserver-textdocument";

export interface Input {
  offset: number;
  position: Position;
  available: number;
  take(count: number): string;
  findNext(predicate: (charCode: number) => boolean): number;
  advance(count: number): Input;
}

export class ParserSuccess<T> {
  success: true = true;

  constructor(public readonly remaining: Input, public readonly result: T) {
    this.remaining = remaining;
    this.result = result;
  }

  map<U>(mapper: (result: T) => U): ParserResult<U> {
    return new ParserSuccess<U>(this.remaining, mapper(this.result));
  }

  select<U>(
    next: (result: ParserSuccess<T>) => ParserResult<U>
  ): ParserResult<U> {
    return next(this);
  }
}

export class ParserFailure<T> {
  success: false = false;

  constructor(
    public readonly remaining: Input,
    public readonly expected: string
  ) {
    this.expected = expected;
  }

  map<U>(mapper: (result: T) => U): ParserResult<U> {
    return new ParserFailure<U>(this.remaining, this.expected);
  }

  select<U>(
    next: (result: ParserSuccess<T>) => ParserResult<U>
  ): ParserResult<U> {
    return new ParserFailure<U>(this.remaining, this.expected);
  }
}

export type ParserResult<T> = ParserSuccess<T> | ParserFailure<T>;

export type Parser<T> = (input: Input) => ParserResult<T>;
