import { InputRange } from "../../parser";

export type DefinitionRef = { moduleName: string; range: InputRange };

export type WithDefinitionRef<T> = { definition?: DefinitionRef; value: T };
