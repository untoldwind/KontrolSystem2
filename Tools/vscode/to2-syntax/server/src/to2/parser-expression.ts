import { alt } from "../parser/branch";
import { Expression } from "./ast";
import { literalBool, literalFloat, literalInt, literalString } from "./parser-literals";

export const term = alt<Expression>([
    literalBool,
    literalFloat,
    literalInt,
    literalString,
])