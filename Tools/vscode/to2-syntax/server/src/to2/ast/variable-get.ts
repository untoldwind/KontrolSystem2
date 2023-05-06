import { Expression, Node, ValidationError } from ".";
import { RealizedType, TO2Type, UNKNOWN_TYPE } from "./to2-type";
import { InputPosition, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { Position } from "vscode-languageserver-textdocument";
import { CompletionItem, CompletionItemKind } from "vscode-languageserver";
import { isFunctionType } from "./function-type";

export class VariableGet extends Expression {
  private allVariables?: [string, RealizedType][];

  constructor(
    public readonly namePath: WithPosition<string[]>,
    start: InputPosition,
    end: InputPosition
  ) {
    super(start, end);
  }

  public resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    return context.findVariable(this.namePath.value, typeHint) ?? UNKNOWN_TYPE;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateBlock(
    context: BlockContext,
    typeHint?: RealizedType
  ): ValidationError[] {
    const errors: ValidationError[] = [];

    const variableType = context
      .findVariable(this.namePath.value, typeHint)
      ?.realizedType(context.module);
    if (!variableType) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.namePath.value[0]}`,
        range: this.namePath.range,
      });
      this.allVariables = context
        .allVariables()
        .map(([name, type]) => [name, type.realizedType(context.module)]);
    } else {
      this.documentation = [
        this.namePath.range.with(
          `Variable \`${this.namePath.value.join("::")} : ${
            variableType.name
          }\``
        ),
      ];
      if (variableType.description)
        this.documentation.push(
          this.namePath.range.with(variableType.description)
        );
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.namePath.range.semanticToken("variable"));
  }

  public completionsAt(position: Position): CompletionItem[] {
    if (this.namePath.range.contains(position)) {
      return (
        this.allVariables?.map(([name, type]) => ({
          kind: isFunctionType(type)
            ? CompletionItemKind.Function
            : CompletionItemKind.Variable,
          label: name,
        })) ?? []
      );
    }
    return [];
  }
}
