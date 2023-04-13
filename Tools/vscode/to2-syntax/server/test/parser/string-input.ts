import { Position } from "vscode-languageserver-textdocument";
import { Input } from "../../src/parser";

export class StringInput implements Input {
    private source: string;
    public offset: number;
    public position: Position;

    constructor(source: string, offset: number = 0) {
        this.source = source;
        this.offset = offset;
        this.position = {
            line: 1,
            character: offset,
        };
    }

    available(): number {
        return this.source.length - this.offset;
    }
    
    take(count: number): string {
        if (count === 0) return "";

        return this.source.substring(this.offset, this.offset + count);
    }

    findNext(predicate: (charCode: number) => boolean): number {
        for(let p = this.offset; p < this.source.length; p++) {
            if(predicate(this.source.charCodeAt(p))) return p - this.offset;
        }
        return -1;
    }
    
    advance(count: number): Input {
        return new StringInput(this.source, this.offset + count);
    }
}