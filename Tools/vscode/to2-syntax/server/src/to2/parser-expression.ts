import { Input, Parser, ParserResult } from "../parser";
import { alt } from "../parser/branch";
import { map, opt, recognizeAs } from "../parser/combinator";
import { spacing0, tag, whitespace0, whitespace1 } from "../parser/complete";
import {
  chain,
  delimited0,
  delimited1,
  delimitedM_N,
  delimitedUntil,
  fold0,
  many0,
} from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import { Expression } from "./ast";
import { ArrayCreate } from "./ast/array-create";
import { Binary } from "./ast/binary";
import { BinaryBool } from "./ast/binary-bool";
import { Block } from "./ast/block";
import { Break, Continue } from "./ast/break-continue";
import { Call } from "./ast/call";
import { FunctionParameter } from "./ast/function-declaration";
import { IfThen, IfThenElse } from "./ast/if-then";
import { IndexSpec } from "./ast/index-spec";
import { Lambda } from "./ast/lambda";
import { Operator } from "./ast/operator";
import { RangeCreate } from "./ast/range-create";
import { RecordCreate } from "./ast/record-create";
import { ReturnEmpty, ReturnValue } from "./ast/return";
import { TupleCreate } from "./ast/tuple-create";
import { TupleDeconstructAssign } from "./ast/tuple-deconstruct-assign";
import { TupleDeconstructDeclaration } from "./ast/tuple-deconstruct-declaration";
import { Unapply } from "./ast/unapply";
import { UnaryPrefix } from "./ast/unary-prefix";
import { VariableAssign } from "./ast/variable-assign";
import { VariableDeclaration } from "./ast/variable-declaration";
import { VariableGet } from "./ast/variable-get";
import { While } from "./ast/while";
import {
  commaDelimiter,
  constKeyword,
  declarationParameter,
  declarationParameterOrPlaceholder,
  eqDelimiter,
  identifier,
  identifierPath,
  letKeyword,
  lineComment,
  typeRef,
  typeSpec,
} from "./parser-common";
import {
  literalBool,
  literalFloat,
  literalInt,
  literalString,
} from "./parser-literals";
import {
  AssignSuffixOperation,
  FieldGetSuffix,
  IndexGetSuffix,
  MethodCallSuffix,
  OperatorSuffix,
  SuffixOperation,
} from "./suffix-operation";

const letOrConst = alt([
  recognizeAs(letKeyword, false),
  recognizeAs(constKeyword, true),
]);

const variableDeclaration = map(
  seq([
    letOrConst,
    alt([
      map(declarationParameter, (item) => ({ isVar: true, items: [item] })),
      map(
        between(
          terminated(tag("("), whitespace0),
          delimited1(
            declarationParameterOrPlaceholder,
            commaDelimiter,
            "<var declaration>"
          ),
          preceded(whitespace0, tag(")"))
        ),
        (items) => ({ isVar: false, items })
      ),
    ]),
    preceded(eqDelimiter, expression),
  ]),
  ([isConst, { isVar, items }, expression], start, end) =>
    isVar
      ? new VariableDeclaration(items[0], isConst, expression, start, end)
      : new TupleDeconstructDeclaration(items, isConst, expression, start, end)
);

const returnExpression = map(
  seq([tag("return"), opt(preceded(spacing0, expression))]),
  ([_, returnValue], start, end) =>
    returnValue
      ? new ReturnValue(returnValue, start, end)
      : new ReturnEmpty(start, end)
);

const whileExpression = map(
  seq([
    between(
      preceded(tag("while"), between(whitespace0, tag("("), whitespace0)),
      expression,
      preceded(whitespace0, tag(")"))
    ),
    preceded(whitespace0, expression),
  ]),
  ([condition, loopExpression], start, end) =>
    new While(condition, loopExpression, start, end)
);

const breakExpression = map(
  tag("break"),
  (_, start, end) => new Break(start, end)
);

const continueExpression = map(
  tag("continue"),
  (_, start, end) => new Continue(start, end)
);

const block = map(
  preceded(
    terminated(tag("{"), whitespace0),
    delimitedUntil(
      alt([
        expression,
        lineComment,
        variableDeclaration,
        returnExpression,
        whileExpression,
        breakExpression,
        continueExpression,
      ]),
      whitespace1,
      tag("}"),
      "<block item>"
    )
  ),
  (items, start, end) => new Block(items, start, end)
);

const callArguments = preceded(
  terminated(tag("("), whitespace0),
  delimitedUntil(
    expression,
    commaDelimiter,
    preceded(whitespace0, tag(")")),
    "<call argument>"
  )
);

const variableRefOrCall = map(
  seq([identifierPath, opt(preceded(spacing0, callArguments))]),
  ([fullname, args], start, end) =>
    args !== undefined
      ? new Call(fullname, args, start, end)
      : new VariableGet(fullname, start, end)
);

const tupleCreate = map(
  between(
    terminated(tag("("), whitespace0),
    delimitedM_N(2, undefined, expression, commaDelimiter, "<expression>"),
    preceded(whitespace0, tag(")"))
  ),
  (expressions, start, end) => new TupleCreate(expressions, start, end)
);

const recordCreate = map(
  seq([
    opt(
      between(
        terminated(tag("<"), whitespace0),
        typeRef,
        preceded(whitespace0, tag(">"))
      )
    ),
    between(
      terminated(tag("("), whitespace0),
      delimited1(
        seq([
          identifier,
          preceded(between(spacing0, tag(":"), spacing0), expression),
        ]),
        commaDelimiter,
        "<expression>"
      ),
      preceded(whitespace0, tag(")"))
    ),
  ]),
  ([resultType, items], start, end) =>
    new RecordCreate(resultType, items, start, end)
);

const arrayCreate = map(
  seq([
    opt(
      between(
        terminated(tag("<"), whitespace0),
        typeRef,
        preceded(whitespace0, tag(">"))
      )
    ),
    preceded(
      terminated(tag("["), whitespace0),
      delimitedUntil(
        expression,
        commaDelimiter,
        preceded(whitespace0, tag("]")),
        "<expression>"
      )
    ),
  ]),
  ([elementType, items], start, end) =>
    new ArrayCreate(elementType, items, start, end)
);

const lambdaParameter = map(
  seq([identifier, opt(typeSpec)]),
  ([name, type], start, end) =>
    new FunctionParameter(name, type, undefined, start, end)
);

const lambdaParameters = preceded(
  terminated(tag("("), whitespace0),
  delimitedUntil(
    lambdaParameter,
    commaDelimiter,
    preceded(whitespace0, tag(")")),
    "<lambda parameter>"
  )
);

const lambda = map(
  seq([
    preceded(terminated(tag("fn"), spacing0), lambdaParameters),
    preceded(between(whitespace0, tag("->"), whitespace0), expression),
  ]),
  ([parameters, expression], start, end) =>
    new Lambda(parameters, expression, start, end)
);

const bracketTerm = between(
  terminated(tag("("), whitespace0),
  expression,
  preceded(whitespace0, tag(")"))
);

const term = alt<Expression>([
  literalBool,
  literalFloat,
  literalInt,
  literalString,
  bracketTerm,
  block,
  arrayCreate,
  tupleCreate,
  recordCreate,
  variableRefOrCall,
  lambda,
]);

const indexSpec = map(expression, (expression) => new IndexSpec(expression));

const suffixOp = recognizeAs(tag("?"), Operator.Unwrap);

const suffixOps = alt<SuffixOperation>([
  map(
    seq([
      preceded(between(whitespace0, tag("."), whitespace0), identifier),
      opt(callArguments),
    ]),
    ([name, args]) =>
      args ? new MethodCallSuffix(name, args) : new FieldGetSuffix(name)
  ),
  map(
    preceded(
      spacing0,
      between(
        terminated(tag("["), whitespace0),
        indexSpec,
        preceded(whitespace0, tag("]"))
      )
    ),
    (indexSpec) => new IndexGetSuffix(indexSpec)
  ),
  map(preceded(spacing0, suffixOp), (op) => new OperatorSuffix(op)),
]);

const termWithSuffixOps = fold0(
  term,
  suffixOps,
  (target, suffixOp, start, end) => suffixOp.getExpression(target, start, end)
);

const unaryPrefixOp = alt<Operator>([
  recognizeAs(tag("-"), Operator.Neg),
  recognizeAs(tag("!"), Operator.Not),
  recognizeAs(tag("~"), Operator.BitNot),
]);

const unaryPrefixExpr = alt([
  map(
    seq([unaryPrefixOp, preceded(whitespace0, termWithSuffixOps)]),
    ([op, right], start, end) => new UnaryPrefix(op, right, start, end)
  ),
  termWithSuffixOps,
]);

const mulDivBinaryOp = between(
  whitespace0,
  alt([
    recognizeAs(tag("*"), Operator.Mul),
    recognizeAs(tag("/"), Operator.Div),
    recognizeAs(tag("%"), Operator.Mod),
  ]),
  whitespace0
);

const mulDivBinaryExpr = chain(
  unaryPrefixExpr,
  mulDivBinaryOp,
  (left, op, right, start, end) => new Binary(left, op, right, start, end)
);

const addSubBinaryOp = between(
  whitespace0,
  alt([
    recognizeAs(tag("+"), Operator.Add),
    recognizeAs(tag("-"), Operator.Sub),
  ]),
  whitespace0
);

const addSubBinaryExpr = chain(
  mulDivBinaryExpr,
  addSubBinaryOp,
  (left, op, right, start, end) => new Binary(left, op, right, start, end)
);

const BITOp = between(
  whitespace0,
  alt([
    recognizeAs(tag("&"), Operator.BitAnd),
    recognizeAs(tag("|"), Operator.BitOr),
    recognizeAs(tag("^"), Operator.BitXor),
  ]),
  whitespace0
);

const BITBinaryExpr = chain(
  addSubBinaryExpr,
  BITOp,
  (left, op, right, start, end) => new Binary(left, op, right, start, end)
);

const rangeCreate = map(
  seq([
    BITBinaryExpr,
    opt(
      seq([
        preceded(spacing0, preceded(tag(".."), opt(tag(".")))),
        preceded(spacing0, BITBinaryExpr),
      ])
    ),
  ]),
  ([from, rest], start, end) => {
    if (rest !== undefined) {
      const [inclusive, to] = rest;

      return new RangeCreate(from, to, inclusive !== undefined, start, end);
    }
    return from;
  }
);

const unapplyExpr = map(
  seq([
    identifier,
    preceded(
      spacing0,
      between(
        terminated(tag("("), spacing0),
        delimited0(identifier, commaDelimiter, "identifier"),
        preceded(spacing0, tag(")"))
      )
    ),
    preceded(eqDelimiter, BITBinaryExpr),
  ]),
  ([pattern, extractNames, expression], start, end) =>
    new Unapply(pattern, extractNames, expression, start, end)
);

const compareOp = between(
  whitespace0,
  alt([
    recognizeAs(tag("=="), Operator.Eq),
    recognizeAs(tag("!="), Operator.Neg),
    recognizeAs(tag("<="), Operator.Le),
    recognizeAs(tag(">="), Operator.Ge),
    recognizeAs(tag("<"), Operator.Lt),
    recognizeAs(tag(">"), Operator.Gt),
  ]),
  whitespace0
);

const compareExpr = chain(
  alt([unapplyExpr, rangeCreate]),
  compareOp,
  (left, op, right, start, end) => new Binary(left, op, right, start, end)
);

const booleanOp = between(
  whitespace0,
  alt([
    recognizeAs(tag("&&"), Operator.BoolAnd),
    recognizeAs(tag("||"), Operator.BoolOr),
  ]),
  whitespace0
);

const booleanExpr = chain(
  compareExpr,
  booleanOp,
  (left, op, right, start, end) => new BinaryBool(left, op, right, start, end)
);

const ifBody = alt([
  expression,
  returnExpression,
  breakExpression,
  continueExpression,
]);

const ifExpr = map(
  seq([
    between(
      preceded(tag("if"), between(whitespace0, tag("("), whitespace0)),
      booleanExpr,
      preceded(whitespace0, tag(")"))
    ),
    preceded(whitespace0, ifBody),
    opt(preceded(between(whitespace1, tag("else"), whitespace1), ifBody)),
  ]),
  ([condition, thenExpression, elseExpression], start, end) =>
    elseExpression
      ? new IfThenElse(condition, thenExpression, elseExpression, start, end)
      : new IfThen(condition, thenExpression, start, end)
);

const assignOp = between(
  whitespace0,
  alt([
    recognizeAs(tag("="), Operator.Assign),
    recognizeAs(tag("+="), Operator.AddAssign),
    recognizeAs(tag("-="), Operator.SubAssign),
    recognizeAs(tag("*="), Operator.MulAssign),
    recognizeAs(tag("/="), Operator.DivAssign),
    recognizeAs(tag("%="), Operator.ModAssign),
    recognizeAs(tag("|="), Operator.BitOrAssign),
    recognizeAs(tag("&="), Operator.BitAndAssign),
    recognizeAs(tag("^="), Operator.BitXorAssign),
  ]),
  whitespace0
);

const assignSuffixOps = alt<AssignSuffixOperation>([
  map(
    preceded(between(whitespace0, tag("."), whitespace0), identifier),
    (name) => new FieldGetSuffix(name)
  ),
  map(
    preceded(
      spacing0,
      between(
        terminated(tag("["), whitespace0),
        indexSpec,
        preceded(whitespace0, tag("]"))
      )
    ),
    (indexSpec) => new IndexGetSuffix(indexSpec)
  ),
]);

const assignment = map(
  seq([
    identifier,
    many0(assignSuffixOps, "<suffix op>"),
    assignOp,
    alt([booleanExpr, ifExpr]),
  ]),
  ([variableName, suffixOps, assignOp, value], start, end) => {
    if (suffixOps.length === 0)
      return new VariableAssign(variableName, assignOp, value, start, end);
    const last = suffixOps[suffixOps.length - 1];
    const target = suffixOps
      .slice(0, suffixOps.length - 1)
      .reduce<Expression>(
        (target, op) => op.getExpression(target, start, end),
        new VariableGet([variableName], start, end)
      );
    return last.assignExpression(target, assignOp, value, start, end);
  }
);

const sourceTargetList = between(
  terminated(tag("("), whitespace0),
  delimited1(
    alt([
      map(
        seq([
          identifier,
          preceded(between(spacing0, tag("@"), spacing0), identifier),
        ]),
        ([source, target]) => ({ source, target })
      ),
      recognizeAs(tag("_"), { source: "", target: "" }),
      map(identifier, (name) => ({ source: name, target: name })),
    ]),
    commaDelimiter,
    "<tuple target>"
  ),
  preceded(whitespace0, tag(")"))
);

const tupleDeconstructAssignment = map(
  seq([sourceTargetList, preceded(eqDelimiter, alt([booleanExpr, ifExpr]))]),
  ([targets, expression], start, end) =>
    new TupleDeconstructAssign(targets, expression, start, end)
);

const topLevelExpression = alt([
  tupleDeconstructAssignment,
  assignment,
  ifExpr,
  booleanExpr,
]);

export function expression(input: Input): ParserResult<Expression> {
  return topLevelExpression(input);
}
