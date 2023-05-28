import { ProposedFeatures, createConnection } from "vscode-languageserver/node";
import { LspServerSingleton } from "./lsp-server-singleton";

const connection = createConnection(ProposedFeatures.all);

new LspServerSingleton(connection);

// Listen on the connection
connection.listen();
