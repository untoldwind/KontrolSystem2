import {
  CompletionItem,
  CompletionParams,
  Connection,
  Diagnostic,
  DiagnosticSeverity,
  DidChangeConfigurationNotification,
  DidChangeConfigurationParams,
  Hover,
  HoverParams,
  InitializeParams,
  InitializeResult,
  InlayHint,
  InlayHintParams,
  SemanticTokens,
  SemanticTokensParams,
  TextDocumentChangeEvent,
  TextDocumentSyncKind,
  TextDocuments,
} from "vscode-languageserver";
import { TextDocument } from "vscode-languageserver-textdocument";
import { findNodesAt } from "./helper";
import { TextDocumentInput } from "./parser/text-document-input";
import { To2LspSettings, defaultSettings } from "./settings";
import {
  SEMANTIC_TOKEN_MODIFIERS,
  SEMANTIC_TOKEN_TYPES,
  SemanticToken,
  convertSemanticTokens,
} from "./syntax-token";
import { Registry } from "./to2/ast/registry";
import { TO2Module, TO2ModuleNode } from "./to2/ast/to2-module";
import { module } from "./to2/parser-module";
import { pathToUri, uriToPath } from "./utils";
import * as fs from "fs/promises";
import * as path from "path";
import { ParserResult } from "./parser";

export class LspServerSingleton {
  private readonly registry = new Registry();
  private globalSettings: To2LspSettings = defaultSettings;
  private hasWorkspaceFolderCapability = false;
  private hasDiagnosticRelatedInformationCapability = false;
  private hasConfigurationCapability = false;
  private hasWorkDonwCapability = false;
  private readonly documentSettings: Map<string, To2LspSettings> = new Map();
  private readonly documentModules: Map<string, TO2ModuleNode> = new Map();
  private readonly documents: TextDocuments<TextDocument>;
  private workspaceFolders: Set<string> = new Set();

  constructor(private readonly connection: Connection) {
    this.documents = new TextDocuments(TextDocument);
    this.documents.onDidOpen(this.onDidOpen.bind(this));
    this.documents.onDidClose(this.onDidClose.bind(this));
    this.documents.onDidChangeContent(this.onDidChange.bind(this));
    this.documents.listen(connection);

    connection.onInitialize(this.onInitialize.bind(this));
    connection.onInitialized(this.onInitialized.bind(this));

    connection.onDidChangeWatchedFiles((_change) => {
      // Monitored files have change in VSCode
      connection.console.log("We received an file change event");
    });

    connection.onCompletion(this.onCompleteion.bind(this));
    connection.onCompletionResolve(this.onCompletionResolve.bind(this));
    connection.onHover(this.onHover.bind(this));
    connection.languages.inlayHint.on(this.onInlayHint.bind(this));
    connection.languages.semanticTokens.on(this.onSemanticTokens.bind(this));
  }

  async validateTextDocument(textDocument: TextDocument): Promise<void> {
    const moduleResult = this.parseModule(textDocument);

    const diagnostics: Diagnostic[] = [];

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
        // In this simple example we get the settings for every validate run.
        const settings = await this.getDocumentSettings(textDocument.uri);

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

  parseModule(textDocument: TextDocument): ParserResult<TO2ModuleNode> {
    const input = new TextDocumentInput(textDocument);
    let moduleName = "<unknown>";
    for (const workspaceUri of this.workspaceFolders) {
      if (
        textDocument.uri.startsWith(workspaceUri) &&
        textDocument.uri.endsWith(".to2")
      ) {
        moduleName = textDocument.uri
          .substring(workspaceUri.length + 1, textDocument.uri.length - 4)
          .replaceAll("/", "::");
        break;
      }
    }
    const moduleResult = module(moduleName)(input);
    if (moduleResult.value) {
      this.documentModules.set(textDocument.uri, moduleResult.value);
    } else {
      this.documentModules.delete(textDocument.uri);
    }
    return moduleResult;
  }

  async indexWorkspace(workspaceUri: string) {
    const workspacePath = uriToPath(workspaceUri);
    if (!workspacePath) return;

    this.workspaceFolders.add(workspaceUri);
    const progress = this.hasWorkDonwCapability
      ? await this.connection.window.createWorkDoneProgress()
      : undefined;

    progress?.begin("Indexing", 0, workspacePath, true);

    const stack: string[] = [workspacePath];

    while (progress === undefined || !progress.token.isCancellationRequested) {
      const dir = stack.shift();
      if (!dir) break;
      const files = await fs.readdir(dir, { withFileTypes: true });

      for (const file of files) {
        if (file.name === "." || file.name === "..") continue;
        if (file.isDirectory()) {
          stack.push(path.join(dir, file.name));
        } else if (file.isFile() && file.name.endsWith(".to2")) {
          const filePath = path.join(dir, file.name);
          const content = await fs.readFile(filePath, "utf-8");
          const textDocument = TextDocument.create(
            pathToUri(filePath, this.documents),
            "to2",
            0,
            content
          );
          this.parseModule(textDocument);
        }
      }
    }

    progress?.done();
  }

  onInitialize(params: InitializeParams): InitializeResult {
    const capabilities = params.capabilities;

    // Does the client support the `workspace/configuration` request?
    // If not, we fall back using global settings.
    this.hasConfigurationCapability = !!(
      capabilities.workspace && !!capabilities.workspace.configuration
    );
    this.hasWorkspaceFolderCapability = !!(
      capabilities.workspace && !!capabilities.workspace.workspaceFolders
    );
    this.hasDiagnosticRelatedInformationCapability = !!(
      capabilities.textDocument &&
      capabilities.textDocument.publishDiagnostics &&
      capabilities.textDocument.publishDiagnostics.relatedInformation
    );
    this.hasWorkDonwCapability = !!(
      capabilities.window && capabilities.window.workDoneProgress
    );

    const result: InitializeResult = {
      capabilities: {
        textDocumentSync: TextDocumentSyncKind.Incremental,
        // Tell the client that this server supports code completion.
        completionProvider: {
          resolveProvider: true,
        },
        semanticTokensProvider: {
          legend: {
            tokenTypes: [...SEMANTIC_TOKEN_TYPES],
            tokenModifiers: [...SEMANTIC_TOKEN_MODIFIERS],
          },
          full: true,
        },
        hoverProvider: true,
        inlayHintProvider: true,
      },
    };
    if (this.hasWorkspaceFolderCapability) {
      result.capabilities.workspace = {
        workspaceFolders: {
          supported: true,
        },
      };
    }

    return result;
  }

  onInitialized() {
    if (this.hasConfigurationCapability) {
      // Register for all configuration changes.
      this.connection.client.register(
        DidChangeConfigurationNotification.type,
        undefined
      );
    }
    if (this.hasWorkspaceFolderCapability) {
      this.connection.workspace.onDidChangeWorkspaceFolders((event) => {
        event.added.forEach((workspaceFolder) =>
          this.indexWorkspace(workspaceFolder.uri)
        );
        event.removed.forEach((workspaceFolder) =>
          this.workspaceFolders.delete(workspaceFolder.uri)
        );
      });
      this.connection.workspace
        .getWorkspaceFolders()
        .then((workspaceFolders) => {
          workspaceFolders?.forEach((workspaceFolder) =>
            this.indexWorkspace(workspaceFolder.uri)
          );
        });
    }
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

  async onSemanticTokens(
    params: SemanticTokensParams
  ): Promise<SemanticTokens> {
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

  onInlayHint(params: InlayHintParams): InlayHint[] {
    const module = this.documentModules.get(params.textDocument.uri);

    return (
      module?.reduceNode((inlayHints, node) => {
        node.inlayHints && inlayHints.push(...node.inlayHints);
        return inlayHints;
      }, [] as InlayHint[]) ?? []
    );
  }
}
