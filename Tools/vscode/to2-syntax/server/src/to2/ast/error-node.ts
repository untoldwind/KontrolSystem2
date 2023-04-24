import { BlockItem, ModuleItem, Node } from ".";
import { InputPosition } from "../../parser";
import { BUILTIN_UNIT, TO2Type } from "./to2-type";

export class ErrorNode implements Node, ModuleItem, BlockItem {
  public isError: boolean = true;
  public isComment: boolean = true;

  constructor(
    public readonly message: string,
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
}

export function isErrorNode(node: Node): node is ErrorNode {
  return node.isError === true;
}
