import { BlockItem, ModuleItem, Node, ValidationError } from ".";
import { TO2Type, currentTypeResolver } from "./to2-type";
import { InputPosition, InputRange } from "../../parser";
import { BlockContext, ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class LineComment implements Node, BlockItem, ModuleItem {
  public isComment: boolean = true;
  public readonly range: InputRange;

  constructor(
    public readonly comment: string,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  resultType(): TO2Type {
    return currentTypeResolver().BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return combine(initialValue, this);
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    return [];
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    return [];
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.range.semanticToken("comment"));
  }

  public setModuleName(moduleName: string, context: ModuleContext) {}
}

export function isLineComment(type: Node): type is LineComment {
  return !!type.isComment;
}
