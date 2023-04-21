import { Position } from "vscode-languageserver-textdocument";

export class InputPosition implements Position {
  constructor(
    public readonly offset: number,
    public readonly line: number,
    public readonly character: number
  ) {}
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

  select<U>(
    next: (result: ParserSuccess<T>) => ParserResult<U>
  ): ParserResult<U> {
    return new ParserFailure<U>(this.remaining, this.expected, undefined);
  }
}

export type ParserResult<T> = ParserSuccess<T> | ParserFailure<T>;

export type Parser<T> = (input: Input) => ParserResult<T>;
