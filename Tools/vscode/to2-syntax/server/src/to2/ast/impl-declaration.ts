import { Node, ValidationError } from ".";
import { LineComment } from "./line-comment";
import { MethodDeclaration } from "./method-declaration";
import { InputPosition, WithPosition } from "../../parser";
import { ImplModuleContext, ModuleContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { RecordType, isRecordType } from "./record-type";

export class ImplDeclaration implements Node {
  constructor(
    private readonly implKeyword: WithPosition<"impl">,
    public readonly name: WithPosition<string>,
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

    const structType = this.findStructType(context);

    const implContext = new ImplModuleContext(
      context,
      structType ?? new RecordType([])
    );
    for (const method of this.methods) {
      errors.push(...method.validateModuleFirstPass(implContext));
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const structType = this.findStructType(context);
    if (!structType) {
      errors.push({
        status: "error",
        message: `Undefined struct ${this.name.value}`,
        start: this.name.start,
        end: this.name.end,
      });
    }

    const implContext = new ImplModuleContext(
      context,
      structType ?? new RecordType([])
    );
    for (const method of this.methods) {
      errors.push(...method.validateModuleSecondPass(implContext));
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "keyword",
      start: this.implKeyword.start,
      length: this.implKeyword.end.offset - this.implKeyword.start.offset,
    });
    semanticTokens.push({
      type: "struct",
      modifiers: ["declaration"],
      start: this.name.start,
      length: this.name.end.offset - this.name.start.offset,
    });

    for (const method of this.methods) {
      method.collectSemanticTokens(semanticTokens);
    }
  }

  private findStructType(context: ModuleContext): RecordType | undefined {
    const typeAlias = context.typeAliases
      .get(this.name.value)
      ?.realizedType(context);

    return typeAlias && isRecordType(typeAlias) ? typeAlias : undefined;
  }
}
