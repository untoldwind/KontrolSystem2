import { Position } from "vscode-languageserver-textdocument";
import { Node } from "./to2/ast";

export function findNodesAt(root: Node, position: Position): Node[] {
  return root.reduceNode((result, node) => {
    if (node.range.contains(position)) result.push(node);
    return result;
  }, [] as Node[]);
}
