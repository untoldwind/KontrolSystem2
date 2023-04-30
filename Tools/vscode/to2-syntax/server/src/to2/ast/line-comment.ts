import { BlockItem, ModuleItem, Node, ValidationError } from ".";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext, ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class LineComment implements Node, BlockItem, ModuleItem {
  public isComment: boolean = true;

  constructor(
    public readonly comment: string,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateModule(context: ModuleContext): ValidationError[] {
    return [];
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "comment",
      start: this.start,
      length: this.end.offset - this.start.offset,
    });
  }
}
