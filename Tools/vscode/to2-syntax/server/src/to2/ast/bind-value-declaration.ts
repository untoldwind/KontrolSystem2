import { InlayHint } from "vscode-languageserver";
import { BlockItem, Expression, Node, ValidationError } from ".";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BoundValueType } from "./bound-value-type";
import { BlockContext } from "./context";
import { TO2Type, UNKNOWN_TYPE } from "./to2-type";

export class BindValueDeclaration implements Node, BlockItem {
  public readonly range: InputRange;
  public inlayHints: InlayHint[] | undefined;
  public documentation?: WithPosition<string>[];

  constructor(
    private readonly bindKeyword: WithPosition<"bind">,
    private readonly name: WithPosition<string>,
    private readonly toKeyword: WithPosition<"to" | "<-">,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    this.range = new InputRange(start, end);
  }

  public resultType(context: BlockContext): TO2Type {
    return new BoundValueType(this.expression.resultType(context));
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    const errors: ValidationError[] = [];

    if (context.localVariables.has(this.name.value)) {
      errors.push({
        status: "error",
        message: `Duplicate variable ${this.name.value}`,
        range: this.name.range,
      });
    } else {
      const variableType = this.resultType(context).realizedType(
        context.module,
      );

      if (variableType !== UNKNOWN_TYPE) {
        this.inlayHints = [
          {
            position: this.name.range.end,
            label: `: ${variableType.localName}`,
            paddingLeft: true,
          },
        ];
      }

      context.localVariables.set(this.name.value, {
        definition: {
          moduleName: context.module.moduleName,
          range: this.name.range,
        },
        value: variableType,
      });
      this.documentation = [
        this.name.range.with(
          `Bind declaration \`${this.name.value} : ${variableType.name}\``,
        ),
      ];
    }
    errors.push(...this.expression.validateBlock(context, undefined));

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.bindKeyword.range.semanticToken("keyword"));
    semanticTokens.push(
      this.name.range.semanticToken("variable", "declaration"),
    );
    semanticTokens.push(this.toKeyword.range.semanticToken("keyword"));
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
