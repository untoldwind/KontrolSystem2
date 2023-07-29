import { Position, TextDocument } from "vscode-languageserver-textdocument";
import { Input, InputPosition } from ".";

export class TextDocumentInput implements Input {
  private sourceText: string;
  public position: InputPosition;
  public available: number;

  constructor(
    private readonly source: TextDocument,
    public readonly offset: number = 0,
    sourceText: string | undefined = undefined,
  ) {
    this.sourceText = sourceText ?? source.getText();
    const textPosition = source.positionAt(offset);
    this.position = new InputPosition(
      offset,
      textPosition.line,
      textPosition.character,
    );
    this.available = this.sourceText.length - offset;
  }

  take(count: number): string {
    if (count === 0) return "";

    return this.sourceText.substring(this.offset, this.offset + count);
  }

  findNext(predicate: (charCode: number) => boolean): number {
    for (let p = this.offset; p < this.sourceText.length; p++) {
      if (predicate(this.sourceText.charCodeAt(p))) return p - this.offset;
    }
    return -1;
  }

  advance(count: number): Input {
    return new TextDocumentInput(
      this.source,
      this.offset + count,
      this.sourceText,
    );
  }
}
