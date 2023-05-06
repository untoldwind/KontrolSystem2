import {
  CompletionItem,
  CompletionItemKind,
  TextEdit,
} from "vscode-languageserver";
import { Position } from "vscode-languageserver-textdocument";
import { ModuleItem, Node, ValidationError } from ".";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { SemanticToken } from "../../syntax-token";
import { ModuleContext } from "./context";
import { Registry } from "./registry";
import { TO2Module } from "./to2-module";

export class UseDeclaration implements Node, ModuleItem {
  public readonly range: InputRange;
  private importedModule?: TO2Module;
  private allModuleNames?: string[];

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

    this.importedModule = context.findModule(this.moduleNamePath.value);
    if (!this.importedModule) {
      errors.push({
        status: "error",
        message: `Module not found: ${this.moduleNamePath.value.join("::")}`,
        range: this.moduleNamePath.range,
      });
      this.allModuleNames = context.registry.allModuleNames();
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
          const importedConstant = this.importedModule.findConstant(name.value);
          const importedFunction = this.importedModule.findFunction(name.value);
          const importedType = this.importedModule.findType(name.value);

          if (!importedConstant && !importedFunction && !importedType) {
            errors.push({
              status: "error",
              message: `Module ${this.importedModule.name} does not have an exported member ${name.value}`,
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
      } else {
        for (const [
          name,
          importedConstant,
        ] of this.importedModule.allConstants()) {
          if (!context.mappedConstants.has(name)) {
            context.mappedConstants.set(name, importedConstant);
          }
        }
        for (const [
          name,
          importedFunction,
        ] of this.importedModule.allFunctions()) {
          if (!context.mappedFunctions.has(name)) {
            context.mappedFunctions.set(name, importedFunction);
          }
        }
        for (const [name, importedType] of this.importedModule.allTypes()) {
          if (!context.typeAliases.has(name)) {
            context.typeAliases.set(name, importedType.realizedType(context));
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

  public completionsAt(position: Position): CompletionItem[] {
    if (this.moduleNamePath.range.contains(position)) {
      return (
        this.allModuleNames?.map((name) => ({
          kind: CompletionItemKind.Module,
          label: name,
          textEdit: TextEdit.replace(this.moduleNamePath.range, name),
        })) ?? []
      );
    } else if (this.names && this.importedModule) {
      for (const name of this.names) {
        if (name.range.contains(position)) {
          return [
            ...this.importedModule.allConstants().map(([name]) => ({
              kind: CompletionItemKind.Constant,
              label: name,
              detail: this.importedModule?.name,
            })),
            ...this.importedModule.allTypes().map(([name]) => ({
              kind: CompletionItemKind.Struct,
              label: name,
              detail: this.importedModule?.name,
            })),
            ...this.importedModule.allFunctions().map(([name]) => ({
              kind: CompletionItemKind.Function,
              label: name,
              detail: this.importedModule?.name,
            })),
          ];
        }
      }
    }
    return [];
  }
}
