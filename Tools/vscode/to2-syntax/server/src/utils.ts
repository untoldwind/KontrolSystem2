import { TextDocuments } from "vscode-languageserver";
import { DocumentUri, TextDocument } from "vscode-languageserver-textdocument";
import { URI } from "vscode-uri";

export function uriToPath(stringUri: DocumentUri): string | undefined {
  if (stringUri.startsWith("zipfile:")) {
    return stringUri;
  }
  const uri = URI.parse(stringUri);
  if (uri.scheme !== "file") {
    return undefined;
  }
  return normalizeFsPath(uri.fsPath);
}

export function pathToUri(
  filepath: string,
  documents: TextDocuments<TextDocument> | undefined,
): DocumentUri {
  if (filepath.startsWith("zipfile:")) {
    return filepath;
  }
  const fileUri = URI.file(filepath);
  const normalizedFilepath = normalizePath(fileUri.fsPath);
  const document = documents?.get(normalizedFilepath);
  return document ? document.uri : fileUri.toString();
}

/**
 * Normalizes the file system path.
 *
 * On systems other than Windows it should be an no-op.
 *
 * On Windows, an input path in a format like "C:/path/file.ts"
 * will be normalized to "c:/path/file.ts".
 */
export function normalizePath(filePath: string): string {
  const fsPath = URI.file(filePath).fsPath;
  return normalizeFsPath(fsPath);
}

const RE_PATHSEP_WINDOWS = /\\/g;

/**
 * Normalizes the path obtained through the "fsPath" property of the URI module.
 */
export function normalizeFsPath(fsPath: string): string {
  return fsPath.replace(RE_PATHSEP_WINDOWS, "/");
}
