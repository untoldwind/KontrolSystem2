import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { ModuleContext } from "./context";

export class UseDeclaration implements Node, ModuleItem {
  public readonly range: InputRange;

  constructor(
    public readonly useKeyword: WithPosition<"use">,
    public readonly names: WithPosition<string>[] | undefined,
    public readonly alias: WithPosition<string> | undefined,
    public readonly asKeyword: WithPosition<"as"> | undefined,
    public readonly fromKeyword: WithPosition<"from"> | undefined,
    public readonly moduleNamePath: WithPosition<string[]>,
    start: InputPosition,
    end: InputPosition
  ) {
    this.range = new InputRange(start, end);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T
  ): T {
    return combine(initialValue, this);
  }

  public validateModuleFirstPass(context: ModuleContext): ValidationError[] {
    const errors: ValidationError[] = [];

    const importedModule = context.findModule(this.moduleNamePath.value);
    if (!importedModule) {
      errors.push({
        status: "error",
        message: `Module not found: ${this.moduleNamePath.value.join("::")}`,
        range: this.moduleNamePath.range,
      });
    } else {
      if (this.alias) {
        if (context.moduleAliases.has(this.alias.value)) {
          errors.push({
            status: "error",
            message: `Duplicate module alias: ${this.alias.value}`,
            range: this.alias.range,
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
              range: name.range,
            });
          }
          if (importedConstant) {
            if (context.mappedConstants.has(name.value)) {
              errors.push({
                status: "error",
                message: `Duplicate constant ${name.value}`,
                range: this.range,
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
                range: this.range,
              });
            } else {
              context.mappedFunctions.set(name.value, importedFunction);
            }
          }
          if (importedType) {
            if (context.typeAliases.has(name.value)) {
              errors.push({
                status: "error",
                message: `Duplicate type alias ${name.value}`,
                range: this.range,
              });
            } else {
              context.typeAliases.set(
                name.value,
                importedType.realizedType(context)
              );
            }
          }
        }
      }
    }

    return errors;
  }

  public validateModuleSecondPass(context: ModuleContext): ValidationError[] {
    return [];
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(this.useKeyword.range.semanticToken("keyword"));
    if (this.names) {
      for (const name of this.names) {
        semanticTokens.push(name.range.semanticToken("variable", "definition"));
      }
    }
    if (this.asKeyword)
      semanticTokens.push(this.asKeyword.range.semanticToken("keyword"));
    if (this.fromKeyword)
      semanticTokens.push(this.fromKeyword.range.semanticToken("keyword"));
    semanticTokens.push(this.moduleNamePath.range.semanticToken("namespace"));
  }
}
