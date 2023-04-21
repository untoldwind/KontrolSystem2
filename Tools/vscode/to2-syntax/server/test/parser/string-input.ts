import { off } from "process";
import { Input, InputPosition } from "../../src/parser";

export class StringInput implements Input {
  public position: InputPosition;
  public available: number;

  constructor(
    private readonly source: string,
    public readonly offset: number = 0
  ) {
    this.source = source;
    this.offset = offset;
    this.position = new InputPosition(offset, 1, offset);
    this.available = source.length - offset;
  }

  take(count: number): string {
    if (count === 0) return "";

    return this.source.substring(this.offset, this.offset + count);
  }

  findNext(predicate: (charCode: number) => boolean): number {
    for (let p = this.offset; p < this.source.length; p++) {
      if (predicate(this.source.charCodeAt(p))) return p - this.offset;
    }
    return -1;
  }

  advance(count: number): Input {
    return new StringInput(this.source, this.offset + count);
  }
}
