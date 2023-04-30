import "../parser/matchers";
import { expression } from "../../src/to2/parser-expression";
import { StringInput } from "../parser/string-input";
import { Operator } from "../../src/to2/ast/operator";

describe("TO2 parser expression", () => {
  it("should parse additions", () => {
    expect(expression(new StringInput(`123 + 456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          op: expect.objectContaining({ value: "+" }),
          left: expect.objectContaining({
            value: 123,
          }),
          right: expect.objectContaining({
            value: 456,
          }),
        }),
      })
    );
    expect(expression(new StringInput(`123 - 456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          op: expect.objectContaining({ value: "-" }),
          left: expect.objectContaining({
            value: 123,
          }),
          right: expect.objectContaining({
            value: 456,
          }),
        }),
      })
    );
  });

  it("should parse operator precedence correctly", () => {
    expect(expression(new StringInput(`12 * 45 + 67 / 89`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          op: expect.objectContaining({ value: "+" }),
          left: expect.objectContaining({
            op: expect.objectContaining({ value: "*" }),
            left: expect.objectContaining({
              value: 12,
            }),
            right: expect.objectContaining({
              value: 45,
            }),
          }),
          right: expect.objectContaining({
            op: expect.objectContaining({ value: "/" }),
            left: expect.objectContaining({
              value: 67,
            }),
            right: expect.objectContaining({
              value: 89,
            }),
          }),
        }),
      })
    );

    expect(expression(new StringInput(`12 * (45 - 67) / 89`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          op: expect.objectContaining({ value: "/" }),
          left: expect.objectContaining({
            op: expect.objectContaining({ value: "*" }),
            left: expect.objectContaining({
              value: 12,
            }),
            right: expect.objectContaining({
              op: expect.objectContaining({ value: "-" }),
              left: expect.objectContaining({
                value: 45,
              }),
              right: expect.objectContaining({
                value: 67,
              }),
            }),
          }),
          right: expect.objectContaining({
            value: 89,
          }),
        }),
      })
    );
  });
});
