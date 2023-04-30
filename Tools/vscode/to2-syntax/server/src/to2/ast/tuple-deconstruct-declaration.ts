import { BlockItem, Expression, Node, ValidationError } from ".";
import {
  DeclarationParameterOrPlaceholder,
  isDeclarationParameter,
} from "./variable-declaration";
import { TO2Type } from "./to2-type";
import { InputPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class TupleDeconstructDeclaration implements Node, BlockItem {
  constructor(
    public readonly declarations: DeclarationParameterOrPlaceholder[],
    public readonly isConst: boolean,
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

    for (const declaration of this.declarations) {
      if (isDeclarationParameter(declaration)) {
        if (context.localVariables.has(declaration.target.value)) {
          errors.push({
            status: "error",
            message: `Duplicate variable ${declaration.target.value}`,
            start: declaration.target.start,
            end: declaration.target.end,
          });
        } else {
          context.localVariables.set(
            declaration.target.value,
            this.resultType(context)
          );
        }
      }
    }
    errors.push(...this.expression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const declaration of this.declarations) {
      if (isDeclarationParameter(declaration)) {
        semanticTokens.push({
          type: "variable",
          modifiers: ["definition"],
          start: declaration.target.start,
          length:
            declaration.target.end.offset - declaration.target.start.offset,
        });
      }
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
