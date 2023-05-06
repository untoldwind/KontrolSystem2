import { TextDocument } from "vscode-languageserver-textdocument";
import { Registry } from "./to2/ast/registry";
import {
  CompletionItem,
  CompletionParams,
  Connection,
  Diagnostic,
  DiagnosticSeverity,
  DidChangeConfigurationParams,
  Hover,
  HoverParams,
  SemanticTokens,
  SemanticTokensParams,
  TextDocumentChangeEvent,
  TextDocuments,
} from "vscode-languageserver";
import { TextDocumentInput } from "./parser/text-document-input";
import { module } from "./to2/parser-module";
import { To2LspSettings, defaultSettings } from "./settings";
import { TO2ModuleNode } from "./to2/ast/to2-module";
import { SemanticToken, convertSemanticTokens } from "./syntax-token";
import { findNodesAt } from "./helper";

export class LspServer {
  private readonly registry = new Registry();
  private globalSettings: To2LspSettings = defaultSettings;
  private readonly documentSettings: Map<string, To2LspSettings> = new Map();
  private readonly documentModules: Map<string, TO2ModuleNode> = new Map();
  private readonly documents: TextDocuments<TextDocument>;

  constructor(
    private readonly connection: Connection,
    public hasConfigurationCapability: boolean
  ) {
    this.documents = new TextDocuments(TextDocument);
    this.documents.onDidOpen(this.onDidOpen.bind(this));
    this.documents.onDidClose(this.onDidClose.bind(this));
    this.documents.onDidChangeContent(this.onDidChange.bind(this));
    this.documents.listen(connection);
  }

  async validateTextDocument(textDocument: TextDocument): Promise<void> {
    const input = new TextDocumentInput(textDocument);
    const moduleResult = module("<test>")(input);

    const diagnostics: Diagnostic[] = [];

    if (moduleResult.value) {
      this.documentModules.set(textDocument.uri, moduleResult.value);
    } else {
      this.documentModules.delete(textDocument.uri);
    }

    if (!moduleResult.success) {
      const diagnostic: Diagnostic = {
        severity: DiagnosticSeverity.Error,
        range: {
          start: moduleResult.remaining.position,
          end: textDocument.positionAt(
            moduleResult.remaining.position.offset +
              moduleResult.remaining.available
          ),
        },
        message: moduleResult.expected,
        source: "parser",
      };
      diagnostics.push(diagnostic);
    }
    if (moduleResult.value) {
      // In this simple example we get the settings for every validate run.
      const settings = await this.getDocumentSettings(textDocument.uri);

      for (const validationError of moduleResult.value.validate(
        this.registry
      )) {
        if (diagnostics.length < settings.maxNumberOfProblems) {
          const diagnostic: Diagnostic = {
            severity:
              validationError.status === "error"
                ? DiagnosticSeverity.Error
                : DiagnosticSeverity.Warning,
            range: validationError.range,
            message: validationError.message,
            source: "parser",
          };
          diagnostics.push(diagnostic);
        }
      }
    }

    // Send the computed diagnostics to VSCode.
    this.connection.sendDiagnostics({ uri: textDocument.uri, diagnostics });
  }

  async getDocumentSettings(resource: string): Promise<To2LspSettings> {
    if (!this.hasConfigurationCapability) {
      return this.globalSettings;
    }
    let result = this.documentSettings.get(resource);
    if (!result) {
      result = await this.connection.workspace.getConfiguration({
        scopeUri: resource,
        section: "to2LspServer",
      });
      if (result) this.documentSettings.set(resource, result);
    }
    return result ?? this.globalSettings;
  }

  onDidChangeConfiguration(change: DidChangeConfigurationParams) {
    if (this.hasConfigurationCapability) {
      // Reset all cached document settings
      this.documentSettings.clear();
    } else {
      this.globalSettings = <To2LspSettings>(
        (change.settings.to2LspServer || defaultSettings)
      );
    }

    // Revalidate all open text documents
    this.documents
      .all()
      .forEach((document) => this.validateTextDocument(document));
  }

  onDidClose(event: TextDocumentChangeEvent<TextDocument>) {
    this.documentSettings.delete(event.document.uri);
    this.documentModules.delete(event.document.uri);
  }

  onDidOpen(event: TextDocumentChangeEvent<TextDocument>) {
    this.validateTextDocument(event.document);
  }

  onDidChange(event: TextDocumentChangeEvent<TextDocument>) {
    this.validateTextDocument(event.document);
  }

  onSemanticTokens(params: SemanticTokensParams): SemanticTokens {
    const module = this.documentModules.get(params.textDocument.uri);
    const token: SemanticToken[] = [];

    module?.collectSemanticTokens(token);

    return {
      data: convertSemanticTokens(token),
    };
  }

  onHover(params: HoverParams): Hover | undefined {
    const module = this.documentModules.get(params.textDocument.uri);

    if (!module) return undefined;

    const documentation = findNodesAt(module, params.position).flatMap((node) =>
      node.documentation
        ? node.documentation
            .filter((doc) => doc.range.contains(params.position))
            .map((doc) => doc.value)
        : []
    );

    if (documentation.length > 0)
      return {
        contents: {
          kind: "markdown",
          value: documentation.join("\n- - -\n"),
        },
      };

    return undefined;
  }

  onCompleteion(params: CompletionParams): CompletionItem[] {
    const module = this.documentModules.get(params.textDocument.uri);

    if (!module) return [];

    return findNodesAt(module, params.position).flatMap(
      (node) => node.completionsAt?.(params.position) ?? []
    );
  }

  onCompletionResolve(item: CompletionItem): CompletionItem {
    return item;
  }
}
