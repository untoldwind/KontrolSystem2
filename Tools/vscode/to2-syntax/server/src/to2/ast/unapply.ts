import { Expression, Node, ValidationError } from ".";
import { TO2Type, UNKNOWN_TYPE, currentTypeResolver } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { isOptionType } from "./option-type";
import { InlayHint } from "vscode-languageserver";

export class Unapply extends Expression {
  public inlayHints: InlayHint[] | undefined;

  constructor(
    public readonly pattern: WithPosition<string>,
    public readonly extractNames: WithPosition<string>[],
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return currentTypeResolver().BUILTIN_BOOL;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const expressionType = this.expression
      .resultType(context)
      .realizedType(context.module);

    switch (this.pattern.value) {
      case "Some":
        if (this.extractNames.length !== 1) {
          errors.push({
            status: "error",
            message: "Some requires one argument",
            range: this.pattern.range,
          });
        } else if (isOptionType(expressionType)) {
          context.localVariables.set(this.extractNames[0].value, {
            definition: {
              moduleName: context.module.moduleName,
              range: this.pattern.range,
            },
            value: expressionType.elementType,
          });
          if (expressionType.elementType !== UNKNOWN_TYPE)
            this.inlayHints = [
              {
                position: this.extractNames[0].range.end,
                label: `: ${expressionType.elementType.localName}`,
                paddingLeft: true,
              },
            ];
        } else {
          errors.push({
            status: "error",
            message: "Expected option type",
            range: this.expression.range,
          });
        }
        break;
      default:
        errors.push({
          status: "error",
          message: `Undefined pattern ${this.pattern}`,
          range: this.pattern.range,
        });
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const extractName of this.extractNames) {
      semanticTokens.push(
        extractName.range.semanticToken("variable", "declaration"),
      );
    }
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
