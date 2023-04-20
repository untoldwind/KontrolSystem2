import { Position } from "vscode-languageserver-textdocument";
import { TO2Type } from "./to2-type";

export interface Node {
  start: Position;
  end: Position;
}

export interface BlockItem {
  resultType(): TO2Type;
}

export interface Expression extends Node, BlockItem {}
