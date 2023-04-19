import { Input, Parser, ParserResult } from "../parser";
import { alt } from "../parser/branch";
import { map, opt, recognizeAs } from "../parser/combinator";
import { spacing0, tag, whitespace0 } from "../parser/complete";
import { chain, delimited0, fold0 } from "../parser/multi";
import { between, preceded, seq, terminated } from "../parser/sequence";
import { Expression } from "./ast";
import { Binary } from "./ast/binary";
import { BinaryBool } from "./ast/binary-bool";
import { Operator } from "./ast/operator";
import { RangeCreate } from "./ast/range-create";
import { Unapply } from "./ast/unapply";
import { UnaryPrefix } from "./ast/unary-prefix";
import { UnarySuffix } from "./ast/unary-suffix";
import { commaDelimiter, identifier } from "./parser-common";
import { literalBool, literalFloat, literalInt, literalString } from "./parser-literals";
import { OperatorSuffix, SuffixOperation } from "./suffix-operation";

export const term = alt<Expression>([
    literalBool,
    literalFloat,
    literalInt,
    literalString,
]);

const suffixOp = recognizeAs(tag("?"), Operator.Unwrap);

const suffixOps = alt([
    map(preceded(spacing0, suffixOp), (op => new OperatorSuffix(op) as SuffixOperation))
]);

const termWithSuffixOps = fold0(term, suffixOps, (target, suffixOp, start, end) => {
    if (suffixOp instanceof OperatorSuffix) {
        return new UnarySuffix(target, suffixOp.op, start, end);
    }
    throw new Error(`<valid suffix> ${start}`);
});

const unaryPrefixOp = alt<Operator>([
    recognizeAs(tag("-"), Operator.Neg),
    recognizeAs(tag("!"), Operator.Not),
    recognizeAs(tag("~"), Operator.BitNot),
]);

const unaryPrefixExpr = alt([
    map(seq([unaryPrefixOp, preceded(whitespace0, termWithSuffixOps)]), ([op, right], start, end) => new UnaryPrefix(op, right, start, end)),
    termWithSuffixOps
]);

const mulDivBinaryOp = between(whitespace0, alt([
    recognizeAs(tag("*"), Operator.Mul),
    recognizeAs(tag("/"), Operator.Div),
    recognizeAs(tag("%"), Operator.Mod),
]), whitespace0);

const mulDivBinaryExpr = chain(unaryPrefixExpr, mulDivBinaryOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

const addSubBinaryOp = between(whitespace0, alt([
    recognizeAs(tag("+"), Operator.Add),
    recognizeAs(tag("-"), Operator.Sub),
]), whitespace0);

const addSubBinaryExpr = chain(mulDivBinaryExpr, addSubBinaryOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

const BITOp = between(whitespace0, alt([
    recognizeAs(tag("&"), Operator.BitAnd),
    recognizeAs(tag("|"), Operator.BitOr),
    recognizeAs(tag("^"), Operator.BitXor),
]), whitespace0);

const BITBinaryExpr = chain(addSubBinaryExpr, BITOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

const rangeCreate = map(seq([
    BITBinaryExpr, opt(seq([preceded(spacing0, preceded(tag(".."), opt(tag(".")))), preceded(spacing0, BITBinaryExpr)]))
]), ([from, rest], start, end) => {
    if(rest !== undefined) {
        const [inclusive, to] = rest;

        return new RangeCreate(from, to, inclusive !== undefined, start, end);
    }
    return from;
});

const unapplyExpr = map(seq([
    identifier, 
    preceded(spacing0, between(terminated(tag("("), spacing0), delimited0(identifier, commaDelimiter, "identifier"), preceded(spacing0, tag(")")))),
    preceded(between(spacing0, tag("="), spacing0), BITBinaryExpr)
]), ([pattern, extractNames, expression], start, end) => new Unapply(pattern, extractNames, expression, start, end));

const compareOp = between(whitespace0, alt([
    recognizeAs(tag("=="), Operator.Eq),
    recognizeAs(tag("!="), Operator.Neg),
    recognizeAs(tag("<="), Operator.Le),
    recognizeAs(tag(">="), Operator.Ge),
    recognizeAs(tag("<"), Operator.Lt),
    recognizeAs(tag(">"), Operator.Gt),    
]), whitespace0);

const compareExpr = chain(alt([unapplyExpr, rangeCreate]), compareOp, (left, op, right, start, end) => new Binary(left, op, right, start, end));

const booleanOp = between(whitespace0, alt([
    recognizeAs(tag("&&"), Operator.BoolAnd),
    recognizeAs(tag("||"), Operator.BoolOr),
]), whitespace0);

const booleanExpr = chain(compareExpr, booleanOp, (left, op, right, start, end) => new BinaryBool(left, op, right, start, end));

const topLevelExpression = alt([
    booleanExpr
]);

export function expression(input: Input): ParserResult<Expression> {
    return topLevelExpression(input);
}
