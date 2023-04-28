import { TextDocument } from "vscode-languageserver-textdocument";
import { Registry } from "./to2/ast/registry";
import {
  Connection,
  Diagnostic,
  DiagnosticSeverity,
} from "vscode-languageserver";
import { getDocumentSettings } from "./server";
import { TextDocumentInput } from "./parser/text-document-input";
import { module } from "./to2/parser-module";

export class LspServer {
  private readonly registry = new Registry();

  constructor(private readonly connection: Connection) {}

  async validateTextDocument(textDocument: TextDocument): Promise<void> {
    // In this simple example we get the settings for every validate run.
    const settings = await getDocumentSettings(textDocument.uri);

    const input = new TextDocumentInput(textDocument);
    const moduleResult = module("<test>")(input);

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
}
