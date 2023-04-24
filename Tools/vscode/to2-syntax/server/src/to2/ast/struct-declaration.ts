import { Position } from "vscode-languageserver-textdocument";
import { Expression, ModuleItem, Node } from ".";
import { TO2Type } from "./to2-type";
import { FunctionParameter } from "./function-declaration";
import { LineComment } from "./line-comment";
import { InputPosition } from "../../parser";

export class StructField implements Node {
  constructor(
    public readonly name: string,
    public readonly type: TO2Type,
    public readonly description: string,
    public readonly initializer: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.initializer.reduceNode(combine, combine(initialValue, this));
  }
}

export class StructDeclaration implements Node, ModuleItem {
  constructor(
    public readonly exported: boolean,
    public readonly name: string,
    public readonly description: string,
    public readonly constructorParameters: FunctionParameter[],
    public readonly fields: (LineComment | StructField)[],
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.fields.reduce(
      (prev, field) => field.reduceNode(combine, prev),
      this.constructorParameters.reduce(
        (prev, param) => param.reduceNode(combine, prev),
        combine(initialValue, this)
      )
    );
  }
}
