import { Expression, Node, ValidationError } from ".";
import { InputPosition, InputRange } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { BlockContext } from "./context";
import { BUILTIN_STRING, TO2Type } from "./to2-type";

interface StringInterpolationPartString {
  value: string;
  range: InputRange;
}

interface StringInterpolationPartExpression {
  expression: Expression;
  alignOrFormat?: string;
}

export type StringInterpolationPart =
  | StringInterpolationPartString
  | StringInterpolationPartExpression;

function isStringInterpolationPartString(
  part: StringInterpolationPart,
): part is StringInterpolationPartString {
  return (part as StringInterpolationPartString).value !== undefined;
}

function isStringInterpolationPartExpression(
  part: StringInterpolationPart,
): part is StringInterpolationPartExpression {
  return (part as StringInterpolationPartExpression).expression !== undefined;
}

export class StringInterpolation extends Expression {
  constructor(
    public readonly parts: StringInterpolationPart[],
    public readonly startRange: InputRange,
    public readonly endRange: InputRange,
  ) {
    super(startRange.start, endRange.end);
  }

  public resultType(): TO2Type {
    return BUILTIN_STRING;
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.parts.reduce(
      (prev, part) =>
        isStringInterpolationPartExpression(part)
          ? part.expression.reduceNode(combine, prev)
          : prev,
      initialValue,
    );
  }

  public validateBlock(context: BlockContext): ValidationError[] {
    return this.parts.flatMap((part) =>
      isStringInterpolationPartExpression(part)
        ? part.expression.validateBlock(context, BUILTIN_STRING)
        : [],
    );
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.startRange.semanticToken("string"));
    for (const part of this.parts) {
      isStringInterpolationPartExpression(part) &&
        part.expression.collectSemanticTokens(semanticTokens);
      isStringInterpolationPartString(part) &&
        semanticTokens.push(part.range.semanticToken("string"));
    }
    semanticTokens.push(this.endRange.semanticToken("string"));
  }
}
