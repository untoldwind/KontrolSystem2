import { BlockItem, Expression, Node, ValidationError } from ".";
import {
  DeclarationParameterOrPlaceholder,
  isDeclarationParameter,
} from "./variable-declaration";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class TupleDeconstructDeclaration implements Node, BlockItem {
  constructor(
    private readonly constLetKeyword: WithPosition<"let" | "const">,
    public readonly declarations: DeclarationParameterOrPlaceholder[],
    public readonly expression: Expression,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public resultType(context: BlockContext): TO2Type {
    return this.expression.resultType(context);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const resultType = this.resultType(context).realizedType(context.module);
    for (let i = 0; i < this.declarations.length; i++) {
      const declaration = this.declarations[i];
      if (isDeclarationParameter(declaration)) {
        if (context.localVariables.has(declaration.target.value)) {
          errors.push({
            status: "error",
            message: `Duplicate variable ${declaration.target.value}`,
            start: declaration.target.start,
            end: declaration.target.end,
          });
        } else if (declaration.type) {
          context.localVariables.set(
            declaration.target.value,
            declaration.type
          );
        } else {
          context.localVariables.set(
            declaration.target.value,
            declaration.extractedType(resultType, i) ?? UNKNOWN_TYPE
          );
        }
      }
    }
    errors.push(...this.expression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "keyword",
      start: this.constLetKeyword.start,
      length:
        this.constLetKeyword.end.offset - this.constLetKeyword.start.offset,
    });
    for (const declaration of this.declarations) {
      if (isDeclarationParameter(declaration)) {
        semanticTokens.push({
          type: "variable",
          modifiers: ["declaration"],
          start: declaration.target.start,
          length:
            declaration.target.end.offset - declaration.target.start.offset,
        });
      }
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
