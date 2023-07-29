import { BlockItem, ModuleItem, Node, ValidationError } from ".";
import { InputPosition, InputRange } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext, ModuleContext } from "./context";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class ErrorNode implements Node, ModuleItem, BlockItem {
  public readonly isError: boolean = true;
  public readonly isComment: boolean = true;
  public readonly range: InputRange;

  constructor(
    public readonly message: string,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_UNIT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return combine(initialValue, this);
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    return [
      {
        status: "error",
        message: this.message,
        range: this.range,
      },
    ];
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    return [
      {
        status: "error",
        message: this.message,
        range: this.range,
      },
    ];
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return [
      {
        status: "error",
        message: this.message,
        range: this.range,
      },
    ];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {}

  public setModuleName(moduleName: string) {}
}

export function isErrorNode(node: Node): node is ErrorNode {
  return node.isError === true;
}
