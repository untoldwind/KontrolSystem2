import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { ModuleContext } from "./context";

export class UseDeclaration implements Node, ModuleItem {
  constructor(
    public readonly useKeyword: WithPosition<"use">,
    public readonly names: WithPosition<string>[] | undefined,
    public readonly alias: WithPosition<string> | undefined,
    public readonly asKeyword: WithPosition<"as"> | undefined,
    public readonly fromKeyword: WithPosition<"from"> | undefined,
    public readonly moduleNamePath: WithPosition<string[]>,
    public readonly start: InputPosition,
    public readonly end: InputPosition
  ) {}

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateModule(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const importedModule = context.findModule(this.moduleNamePath.value);
    if (!importedModule) {
      errors.push({
        status: "error",
        message: `Module not found: ${this.moduleNamePath.value.join("::")}`,
        start: this.moduleNamePath.start,
        end: this.moduleNamePath.end,
      });
    } else {
      if (this.alias) {
        if (context.moduleAliases.has(this.alias.value)) {
          errors.push({
            status: "error",
            message: `Duplicate module alias: ${this.alias.value}`,
            start: this.alias.start,
            end: this.alias.end,
          });
        } else {
          context.moduleAliases.set(
            this.alias.value,
            this.moduleNamePath.value
          );
        }
      }
      if (this.names) {
        for (const name of this.names) {
          const importedConstant = importedModule.findConstant(name.value);
          const importedFunction = importedModule.findFunction(name.value);
          const importedType = importedModule.findType(name.value);

          if (!importedConstant && !importedFunction && !importedType) {
            errors.push({
              status: "error",
              message: `Module ${importedModule.name} does not have an exported member ${name.value}`,
              start: name.start,
              end: name.end,
            });
          }
          if (importedConstant) {
            if (context.mappedConstants.has(name.value)) {
              errors.push({
                status: "error",
                message: `Duplicate constant ${name.value}`,
                start: this.start,
                end: this.end,
              });
            } else {
              context.mappedConstants.set(name.value, importedConstant);
            }
          }
          if (importedFunction) {
            if (context.mappedFunctions.has(name.value)) {
              errors.push({
                status: "error",
                message: `Duplicate function name ${name.value}`,
                start: this.start,
                end: this.end,
              });
            } else {
              context.mappedFunctions.set(name.value, importedFunction);
            }
          }
        }
      }
    }

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push({
      type: "keyword",
      start: this.useKeyword.start,
      length: this.useKeyword.end.offset - this.useKeyword.start.offset,
    });
    if (this.names) {
      for (const name of this.names) {
        semanticTokens.push({
          type: "variable",
          modifiers: ["declaration"],
          start: name.start,
          length: name.end.offset - name.start.offset,
        });
      }
    }
    if (this.asKeyword)
      semanticTokens.push({
        type: "keyword",
        start: this.asKeyword.start,
        length: this.asKeyword.end.offset - this.asKeyword.start.offset,
      });
    if (this.fromKeyword)
      semanticTokens.push({
        type: "keyword",
        start: this.fromKeyword.start,
        length: this.fromKeyword.end.offset - this.fromKeyword.start.offset,
      });
    semanticTokens.push({
      type: "namespace",
      start: this.moduleNamePath.start,
      length: this.moduleNamePath.end.offset - this.moduleNamePath.start.offset,
    });
  }
}
