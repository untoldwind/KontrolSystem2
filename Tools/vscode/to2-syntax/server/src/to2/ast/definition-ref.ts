import { InputRange } from "../../parser";

export type DefinitionRef = { range: InputRange };

export type WithDefinitionRef<T> = { definition?: DefinitionRef; value: T };
