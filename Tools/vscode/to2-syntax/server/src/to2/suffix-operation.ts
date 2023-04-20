import { Position } from "vscode-languageserver-textdocument";
import { Expression } from "./ast";
import { FieldGet } from "./ast/field-get";
import { IndexGet } from "./ast/index-get";
import { IndexSpec } from "./ast/index-spec";
import { MethodCall } from "./ast/method-call";
import { Operator } from "./ast/operator";
import { UnarySuffix } from "./ast/unary-suffix";

export interface SuffixOperation {
  getExpression(target: Expression, start: Position, end: Position): Expression;
}

export interface AssignSuffixOperation {}

export class IndexGetSuffix implements SuffixOperation, AssignSuffixOperation {
  constructor(public readonly indexSpec: IndexSpec) {}

  getExpression(
    target: Expression,
    start: Position,
    end: Position
  ): Expression {
    return new IndexGet(target, this.indexSpec, start, end);
  }
}

export class FieldGetSuffix implements SuffixOperation, AssignSuffixOperation {
  constructor(public readonly fieldName: string) {}

  getExpression(
    target: Expression,
    start: Position,
    end: Position
  ): Expression {
    return new FieldGet(target, this.fieldName, start, end);
  }
}

export class MethodCallSuffix implements SuffixOperation {
  constructor(
    public readonly methodName: string,
    public readonly args: Expression[]
  ) {}

  getExpression(
    target: Expression,
    start: Position,
    end: Position
  ): Expression {
    return new MethodCall(target, this.methodName, this.args, start, end);
  }
}

export class OperatorSuffix implements SuffixOperation {
  constructor(public readonly op: Operator) {}

  getExpression(
    target: Expression,
    start: Position,
    end: Position
  ): Expression {
    return new UnarySuffix(target, this.op, start, end);
  }
}
