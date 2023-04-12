export interface Input {
    current(): string
    available(): number
    take(count: number): string
    findNext(predicate: (ch : string) => boolean): number
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
    error: string;

    constructor(error: string) {
        this.error = error;
    }

    map<U>(mapper: (result: T) => U) : ParserResult<U> {
        return new ParserFailure<U>(this.error);
    }

    select<U>(next: (result: ParserSuccess<T>) => ParserResult<U>) : ParserResult<U> {
        return new ParserFailure<U>(this.error);
    }
}


export type ParserResult<T> = ParserSuccess<T> | ParserFailure<T>;

export type Parser<T> = (input: Input) => ParserResult<T>;
