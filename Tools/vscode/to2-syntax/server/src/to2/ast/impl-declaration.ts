import { Node } from ".";
import { LineComment } from "./line-comment";
import { MethodDeclaration } from "./method-declaration";
import { InputPosition } from "../../parser";

export class ImplDeclaration implements Node {
  constructor(
    public readonly name: string,
    public readonly methods: (LineComment | MethodDeclaration)[],
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.methods.reduce(
      (prev, method) => method.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }
}
