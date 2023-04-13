import { Position } from "vscode-languageserver-textdocument";

export interface Node {
    start: Position
    end: Position
}

export interface Expression extends Node {

}