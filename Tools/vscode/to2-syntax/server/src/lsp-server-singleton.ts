import * as fs from "fs/promises";
import * as path from "path";
import {
  CompletionItem,
  CompletionParams,
  Connection,
  DefinitionParams,
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
  LocationLink,
  SemanticTokens,
  SemanticTokensParams,
  TextDocumentChangeEvent,
  TextDocumentSyncKind,
  TextDocuments,
} from "vscode-languageserver";
import { DocumentUri, TextDocument } from "vscode-languageserver-textdocument";
import { findNodesAt } from "./helper";
import { ParserResult } from "./parser";
import { TextDocumentInput } from "./parser/text-document-input";
import { To2LspSettings, defaultSettings } from "./settings";
import {
  SEMANTIC_TOKEN_MODIFIERS,
  SEMANTIC_TOKEN_TYPES,
  SemanticToken,
  convertSemanticTokens,
} from "./syntax-token";
import { Registry } from "./to2/ast/registry";
import { TO2ModuleNode, isTO2ModuleNode } from "./to2/ast/to2-module";
import { module } from "./to2/parser-module";
import { isDirectory, isFile, pathToUri, uriToPath } from "./utils";
import { initTypeResolver, typeResolverInitialized } from "./to2/ast/to2-type";
import { Reference } from "./reference";

export class LspServerSingleton {
  private readonly registry = new Registry();
  private globalSettings: To2LspSettings = defaultSettings;
  private hasWorkspaceFolderCapability = false;
  private hasDiagnosticRelatedInformationCapability = false;
  private hasConfigurationCapability = false;
  private hasWorkDonwCapability = false;
  private readonly documentSettings: Map<string, To2LspSettings> = new Map();
  private readonly modulesByUri: Map<string, TO2ModuleNode> = new Map();
  private readonly documents: TextDocuments<TextDocument>;
  private workspaceFolders: Set<DocumentUri> = new Set();

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
    connection.onDefinition(this.onDefinition.bind(this));
    connection.languages.inlayHint.on(this.onInlayHint.bind(this));
    connection.languages.semanticTokens.on(this.onSemanticTokens.bind(this));
  }

  async validateTextDocument(textDocument: TextDocument): Promise<void> {
    const moduleResult = this.parseModule(textDocument, true);

    const diagnostics: Diagnostic[] = [];

    if (!moduleResult.success) {
      const diagnostic: Diagnostic = {
        severity: DiagnosticSeverity.Error,
        range: {
          start: moduleResult.remaining.position,
          end: textDocument.positionAt(
            moduleResult.remaining.position.offset +
              moduleResult.remaining.available,
          ),
        },
        message: moduleResult.expected,
        source: "parser",
      };
      diagnostics.push(diagnostic);
    }

    if (moduleResult.value) {
      for (const validationError of moduleResult.value.validate(
        this.registry,
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

  parseModule(
    textDocument: TextDocument,
    override: boolean,
  ): ParserResult<TO2ModuleNode> {
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
    const moduleResult = module(
      textDocument.uri,
      moduleName,
      this.registry,
    )(input);
    if (moduleResult.value) {
      if (override || !this.modulesByUri.has(textDocument.uri))
        this.modulesByUri.set(textDocument.uri, moduleResult.value);

      this.registry.modules.set(moduleResult.value.name, moduleResult.value);
    } else {
      const existing = this.modulesByUri.get(textDocument.uri);
      if (existing) this.registry.modules.delete(existing.name);
      this.modulesByUri.delete(textDocument.uri);
    }
    return moduleResult;
  }

  async indexWorkspace(workspaceUri: string) {
    const workspacePath = uriToPath(workspaceUri);
    if (!workspacePath || !(await isDirectory(workspacePath))) return;

    this.workspaceFolders.add(workspaceUri);

    let pathsToIndex = [workspacePath];
    if (path.basename(workspacePath) === "KontrolSystem2") {
      // Editor has been opened on plugin folder, try to find "to2" and "to2Local" subdirectories
      const files = await fs.readdir(workspacePath, { withFileTypes: true });
      const subDirs = files
        .filter(
          (f) => f.isDirectory() && (f.name === "to2" || f.name === "to2Local"),
        )
        .map((f) => path.join(workspacePath, f.name));

      if (subDirs.length > 0) {
        pathsToIndex = subDirs;
      }
    }

    const progress = this.hasWorkDonwCapability
      ? await this.connection.window.createWorkDoneProgress()
      : undefined;

    progress?.begin("Indexing", 0, pathsToIndex.join(", "), true);

    const stack: string[] = pathsToIndex;
    const indexedModules: TO2ModuleNode[] = [];

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
            content,
          );
          const parseResult = this.parseModule(textDocument, false);
          if (parseResult.success) {
            indexedModules.push(parseResult.value);
          }
        }
      }
    }

    for (const module of indexedModules) {
      module.validate(this.registry);
    }

    for (const document of this.documents.all()) {
      if (progress && progress.token.isCancellationRequested) break;
      this.validateTextDocument(document);
    }

    progress?.done();
  }

  async updateTypeResolver(workspacePath: string) {
    if (typeResolverInitialized()) return;

    const candidates = [
      path.resolve(workspacePath, "reference.json"),
      path.resolve(workspacePath, "..", "reference.json"),
    ];

    for (const candidate of candidates) {
      if (await isFile(candidate)) {
        try {
          const content = JSON.parse(await fs.readFile(candidate, "utf-8"));

          initTypeResolver(content as Reference);
        } catch (e) {
          // ignore
        }
      }
    }
  }

  async updateConfig(config: To2LspSettings) {
    this.globalSettings = config;

    for (const libraryPath of [config.libraryPath, config.localLibraryPath]) {
      if (libraryPath.length === 0 || !(await isDirectory(libraryPath)))
        continue;
      await this.updateTypeResolver(libraryPath);
      const libraryUri = pathToUri(libraryPath, this.documents);
      if (!this.workspaceFolders.has(libraryUri)) {
        await this.indexWorkspace(libraryUri);
      }
    }
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
        definitionProvider: true,
      },
    };
    if (this.hasWorkspaceFolderCapability) {
      result.capabilities.workspace = {
        workspaceFolders: {
          supported: true,
        },
      };
    }
    params.workspaceFolders?.forEach((workspaceFolder) =>
      this.workspaceFolders.add(workspaceFolder.uri),
    );

    return result;
  }

  async onInitialized() {
    for (const folder of this.workspaceFolders) {
      const folderPath = uriToPath(folder);
      folderPath && (await this.updateTypeResolver(folderPath));
    }
    if (this.hasConfigurationCapability) {
      // Register for all configuration changes.
      this.connection.client.register(
        DidChangeConfigurationNotification.type,
        undefined,
      );
      this.connection.workspace
        .getConfiguration("to2LspServer")
        .then(this.updateConfig.bind(this));
    }
    if (this.hasWorkspaceFolderCapability) {
      this.connection.workspace.onDidChangeWorkspaceFolders((event) => {
        event.added.forEach((workspaceFolder) =>
          this.indexWorkspace(workspaceFolder.uri),
        );
        event.removed.forEach((workspaceFolder) =>
          this.workspaceFolders.delete(workspaceFolder.uri),
        );
      });
      this.connection.workspace
        .getWorkspaceFolders()
        .then((workspaceFolders) => {
          workspaceFolders?.forEach((workspaceFolder) =>
            this.indexWorkspace(workspaceFolder.uri),
          );
        });
    }
  }

  onDidChangeConfiguration(change: DidChangeConfigurationParams) {
    if (this.hasConfigurationCapability) {
      // Reset all cached document settings
      this.documentSettings.clear();
    } else {
      this.updateConfig(change.settings.to2LspServer || defaultSettings);
    }

    // Revalidate all open text documents
    this.documents
      .all()
      .forEach((document) => this.validateTextDocument(document));
  }

  onDidClose(event: TextDocumentChangeEvent<TextDocument>) {
    this.documentSettings.delete(event.document.uri);
  }

  onDidOpen(event: TextDocumentChangeEvent<TextDocument>) {
    this.validateTextDocument(event.document);
  }

  onDidChange(event: TextDocumentChangeEvent<TextDocument>) {
    this.validateTextDocument(event.document);
  }

  async onSemanticTokens(
    params: SemanticTokensParams,
  ): Promise<SemanticTokens> {
    const module = this.modulesByUri.get(params.textDocument.uri);
    const token: SemanticToken[] = [];

    module?.collectSemanticTokens(token);

    return {
      data: convertSemanticTokens(token),
    };
  }

  onHover(params: HoverParams): Hover | undefined {
    const module = this.modulesByUri.get(params.textDocument.uri);

    if (!module) return undefined;

    const documentation = findNodesAt(module, params.position).flatMap(
      (node) =>
        node.documentation
          ? node.documentation
              .filter((doc) => doc.range.contains(params.position))
              .map((doc) => doc.value)
          : [],
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

  onDefinition(params: DefinitionParams): LocationLink[] {
    const module = this.modulesByUri.get(params.textDocument.uri);

    if (!module) return [];

    return findNodesAt(module, params.position)
      .filter(
        (node) =>
          node.reference !== undefined &&
          node.reference.sourceRange.contains(params.position),
      )
      .flatMap((node) => {
        const reference = node.reference;
        if (!reference) return [];
        const module = this.registry.modules.get(
          reference.definition.moduleName,
        );
        if (!module || !isTO2ModuleNode(module)) return [];
        return [
          LocationLink.create(
            module.documentUri,
            node.reference!!.definition.range,
            node.reference!!.definition.range,
            node.reference!!.sourceRange,
          ),
        ];
      });
  }

  onCompleteion(params: CompletionParams): CompletionItem[] {
    const module = this.modulesByUri.get(params.textDocument.uri);

    if (!module) return [];

    return findNodesAt(module, params.position).flatMap(
      (node) => node.completionsAt?.(params.position) ?? [],
    );
  }

  onCompletionResolve(item: CompletionItem): CompletionItem {
    return item;
  }

  onInlayHint(params: InlayHintParams): InlayHint[] {
    const module = this.modulesByUri.get(params.textDocument.uri);

    return (
      module?.reduceNode((inlayHints, node) => {
        node.inlayHints && inlayHints.push(...node.inlayHints);
        return inlayHints;
      }, [] as InlayHint[]) ?? []
    );
  }
}
