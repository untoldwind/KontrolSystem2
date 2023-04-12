import { Input } from ".";

export class StringInput implements Input {
    private source: string;
    private position: number;

    constructor(source: string, position: number = 0) {
        this.source = source;
        this.position = position;
    }

    current(): string {
        return this.source[this.position];
    }

    available(): number {
        return this.source.length - this.position;
    }
    
    take(count: number): string {
        if (count === 0) return "";

        return this.source.substring(this.position, this.position + count);
    }

    findNext(predicate: (ch: string) => boolean): number {
        for(let p = this.position; p < this.source.length; p++) {
            if(predicate(this.source[p])) return p - this.position;
        }
        return -1;
    }
    
    advance(count: number): Input {
        return new StringInput(this.source, this.position + count);
    }
}