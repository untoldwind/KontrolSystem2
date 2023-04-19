import "../parser/matchers";
import { expression } from "../../src/to2/parser-expression";
import { StringInput } from "../parser/string-input";
import { Operator } from "../../src/to2/ast/operator";

describe("TO2 parser expression", () => {
    it("should parse additions", () => {
        expect(expression(new StringInput(`123 + 456`))).toEqual(expect.objectContaining({
            success: true,
            result: expect.objectContaining({
                op: Operator.Add,
                left: expect.objectContaining({
                    value: 123,
                }),
                right: expect.objectContaining({
                    value: 456,
                }),
            }),
        }));
        expect(expression(new StringInput(`123 - 456`))).toEqual(expect.objectContaining({
            success: true,
            result: expect.objectContaining({
                op: Operator.Sub,
                left: expect.objectContaining({
                    value: 123,
                }),
                right: expect.objectContaining({
                    value: 456,
                }),
            }),
        }));
    });
});