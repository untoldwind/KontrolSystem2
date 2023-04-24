import { Expression, Node } from ".";
import {
  BUILTIN_BOOL,
  BUILTIN_FLOAT,
  BUILTIN_INT,
  BUILTIN_STRING,
  TO2Type,
} from "./to2-type";
import { InputPosition } from "../../parser";

export class LiteralBool extends Expression {
  constructor(
    public readonly value: boolean,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_BOOL;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}

export class LiteralInt extends Expression {
  constructor(
    public readonly value: number,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_INT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}

export class LiteralFloat extends Expression {
  constructor(
    public readonly value: number,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_FLOAT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}

export class LiteralString extends Expression {
  constructor(
    public readonly value: string,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return BUILTIN_STRING;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }
}
