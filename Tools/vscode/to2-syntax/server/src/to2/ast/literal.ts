import { BlockItem, Expression, Node, ValidationError } from ".";
import { TO2Type, currentTypeResolver } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class LiteralBool extends Expression {
  constructor(
    public readonly value: boolean,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return currentTypeResolver().BUILTIN_BOOL;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return combine(initialValue, this);
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.range.semanticToken("keyword"));
  }
}

export class LiteralInt extends Expression {
  constructor(
    public readonly value: number,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return currentTypeResolver().BUILTIN_INT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return combine(initialValue, this);
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.range.semanticToken("number"));
  }
}

export class LiteralFloat extends Expression {
  constructor(
    public readonly value: number,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return currentTypeResolver().BUILTIN_FLOAT;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return combine(initialValue, this);
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.range.semanticToken("number"));
  }
}

export class LiteralString extends Expression {
  constructor(
    public readonly value: string,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  resultType(): TO2Type {
    return currentTypeResolver().BUILTIN_STRING;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return combine(initialValue, this);
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.range.semanticToken("string"));
  }
}
