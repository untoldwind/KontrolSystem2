import { TextDocument } from "vscode-languageserver-textdocument";
import { Registry } from "./to2/ast/registry";
import {
  Connection,
  Diagnostic,
  DiagnosticSeverity,
  DidChangeConfigurationParams,
  SemanticTokens,
  SemanticTokensParams,
  TextDocumentChangeEvent,
  TextDocuments,
} from "vscode-languageserver";
import { TextDocumentInput } from "./parser/text-document-input";
import { module } from "./to2/parser-module";
import { To2LspSettings, defaultSettings } from "./settings";
import { TO2ModuleNode } from "./to2/ast/to2-module";

export class LspServer {
  private readonly registry = new Registry();
  private globalSettings: To2LspSettings = defaultSettings;
  private readonly documentSettings: Map<string, Thenable<To2LspSettings>> =
    new Map();
  private readonly documentModules: Map<string, TO2ModuleNode> = new Map();
  private readonly documents: TextDocuments<TextDocument>;

  constructor(
    private readonly connection: Connection,
    public hasConfigurationCapability: boolean
  ) {
    this.documents = new TextDocuments(TextDocument);
    this.documents.onDidClose(this.onDidClose.bind(this));
    this.documents.onDidChangeContent(this.onDidChange.bind(this));
    this.documents.listen(connection);
  }

  async validateTextDocument(textDocument: TextDocument): Promise<void> {
    // In this simple example we get the settings for every validate run.
    const settings = await this.getDocumentSettings(textDocument.uri);

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
      for (const validationError of moduleResult.value.validate(
        this.registry
      )) {
        if (diagnostics.length < settings.maxNumberOfProblems) {
          const diagnostic: Diagnostic = {
            severity: DiagnosticSeverity.Error,
            range: {
              start: validationError.start,
              end: validationError.end,
            },
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

  getDocumentSettings(resource: string): Thenable<To2LspSettings> {
    if (!this.hasConfigurationCapability) {
      return Promise.resolve(this.globalSettings);
    }
    let result = this.documentSettings.get(resource);
    if (!result) {
      result = this.connection.workspace.getConfiguration({
        scopeUri: resource,
        section: "to2LspServer",
      });
      this.documentSettings.set(resource, result);
    }
    return result;
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

  onDidChange(event: TextDocumentChangeEvent<TextDocument>) {
    this.connection.console.log(event.document.uri);
    this.validateTextDocument(event.document);
  }

  onSemanticTokens(params: SemanticTokensParams): SemanticTokens {
    return {
      data: [],
    };
  }
}