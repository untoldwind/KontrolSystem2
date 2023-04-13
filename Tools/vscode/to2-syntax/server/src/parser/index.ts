import { Position } from "vscode-languageserver-textdocument"

export interface Input {
    offset: number
    position: Position
    available(): number
    take(count: number): string
    findNext(predicate: (charCode : number) => boolean): number
    advance(count: number): Input
}

export class ParserSuccess<T> {
    success: true = true;
    result: T;
    remaining: Input;

    constructor(remaining: Input, result: T) {
        this.remaining = remaining;
        this.result = result;
    }

    map<U>(mapper: (result: T) => U) : ParserResult<U> {
        return new ParserSuccess<U>(this.remaining, mapper(this.result));
    }

    select<U>(next: (result: ParserSuccess<T>) => ParserResult<U>) : ParserResult<U> {
        return next(this);
    }
}

export class ParserFailure<T> {
    success: false = false;
    expected: string;

    constructor(expected: string) {
        this.expected = expected;
    }

    map<U>(mapper: (result: T) => U) : ParserResult<U> {
        return new ParserFailure<U>(this.expected);
    }

    select<U>(next: (result: ParserSuccess<T>) => ParserResult<U>) : ParserResult<U> {
        return new ParserFailure<U>(this.expected);
    }
}


export type ParserResult<T> = ParserSuccess<T> | ParserFailure<T>;

export type Parser<T> = (input: Input) => ParserResult<T>;
