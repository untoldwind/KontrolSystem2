import { BlockItem, Expression, Node, ValidationError } from ".";
import {
  DeclarationParameterOrPlaceholder,
  isDeclarationParameter,
} from "./variable-declaration";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class TupleDeconstructDeclaration implements Node, BlockItem {
  public documentation?: WithPosition<string>[];
  public readonly range: InputRange;

  constructor(
    private readonly constLetKeyword: WithPosition<"let" | "const">,
    public readonly declarations: DeclarationParameterOrPlaceholder[],
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition
  ) {
    this.range = new InputRange(start, end);
  }

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
    this.documentation = [];
    for (let i = 0; i < this.declarations.length; i++) {
      const declaration = this.declarations[i];
      if (isDeclarationParameter(declaration)) {
        if (context.localVariables.has(declaration.target.value)) {
          errors.push({
            status: "error",
            message: `Duplicate variable ${declaration.target.value}`,
            range: declaration.target.range,
          });
        } else {
          const variableType =
            declaration.type?.value ??
            declaration.extractedType(resultType, i) ??
            UNKNOWN_TYPE;
          context.localVariables.set(declaration.target.value, variableType);
          this.documentation.push(
            declaration.target.range.with(
              `Variable declaration \`${declaration.target.value} : ${variableType.name}\``
            )
          );
        }
      }
    }
    errors.push(...this.expression.validateBlock(context));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.constLetKeyword.range.semanticToken("keyword"));
    for (const declaration of this.declarations) {
      if (isDeclarationParameter(declaration)) {
        semanticTokens.push(
          declaration.target.range.semanticToken("variable", "declaration")
        );
      }
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
