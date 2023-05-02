import { Node, ValidationError } from ".";
import { LineComment } from "./line-comment";
import { MethodDeclaration } from "./method-declaration";
import { InputPosition, WithPosition } from "../../parser";
import { ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";

export class ImplDeclaration implements Node {
  constructor(
    private readonly implKeyword: WithPosition<"impl">,
    public readonly name: string,
    public readonly methods: (LineComment | MethodDeclaration)[],
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return this.methods.reduce(
      (prev, method) => method.reduceNode(combine, prev),
      combine(initialValue, this)
    );
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    for (const method of this.methods) {
      errors.push(...method.validateModuleFirstPass(context));
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    for (const method of this.methods) {
      errors.push(...method.validateModuleSecondPass(context));
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    for (const method of this.methods) {
      method.collectSemanticTokens(semanticTokens);
    }
  }
}
