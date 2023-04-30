import { Expression } from "./ast";
import { FieldGet } from "./ast/field-get";
import { IndexGet } from "./ast/index-get";
import { IndexSpec } from "./ast/index-spec";
import { MethodCall } from "./ast/method-call";
import { Operator } from "./ast/operator";
import { UnarySuffix } from "./ast/unary-suffix";
import { IndexAssign } from "./ast/index-assign";
import { FieldAssign } from "./ast/field-assign";
import { InputPosition, WithPosition } from "../parser";

export interface SuffixOperation {
  getExpression(
    target: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression;
}

export interface AssignSuffixOperation extends SuffixOperation {
  assignExpression(
    target: Expression,
    op: Operator,
    value: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression;
}

export class IndexGetSuffix implements SuffixOperation, AssignSuffixOperation {
  constructor(public readonly indexSpec: IndexSpec) {}
  getExpression(
    target: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression {
    return new IndexGet(target, this.indexSpec, start, end);
  }

  assignExpression(
    target: Expression,
    op: Operator,
    value: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression {
    return new IndexAssign(target, this.indexSpec, op, value, start, end);
  }
}

export class FieldGetSuffix implements SuffixOperation, AssignSuffixOperation {
  constructor(public readonly fieldName: WithPosition<string>) {}
  getExpression(
    target: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression {
    return new FieldGet(target, this.fieldName, start, end);
  }

  assignExpression(
    target: Expression,
    op: Operator,
    value: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression {
    return new FieldAssign(target, this.fieldName, op, value, start, end);
  }
}

export class MethodCallSuffix implements SuffixOperation {
  constructor(
    public readonly methodName: WithPosition<string>,
    public readonly args: Expression[]
  ) {}

  getExpression(
    target: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression {
    return new MethodCall(target, this.methodName, this.args, start, end);
  }
}

export class OperatorSuffix implements SuffixOperation {
  constructor(public readonly op: WithPosition<Operator>) {}

  getExpression(
    target: Expression,
    start: InputPosition,
    end: InputPosition
  ): Expression {
    return new UnarySuffix(target, this.op, start, end);
  }
}
