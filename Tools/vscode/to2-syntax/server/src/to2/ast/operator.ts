import { z } from "zod";

export const OPERATORS = [
  "=",
  "+",
  "+=",
  "-",
  "-=",
  "*",
  "*=",
  "/",
  "/=",
  "%",
  "%=",
  "|",
  "|=",
  "&",
  "&=",
  "^",
  "^=",
  "**",
  "**=",
  "==",
  "!=",
  "<",
  "<=",
  ">",
  ">=",
  "!",
  "&&",
  "||",
  "?",
  "~",
] as const;

export const Operator = z.enum(OPERATORS);

export type Operator = z.infer<typeof Operator>;
