import "../parser/matchers";
import {
  literalBool,
  literalFloat,
  literalInt,
  literalString,
} from "../../src/to2/parser-literals";
import { StringInput } from "../parser/string-input";

describe("TO2 parser literals", () => {
  it("should parse literal strings", () => {
    expect(literalString(new StringInput(""))).toBeFailure();
    expect(literalString(new StringInput(`""`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: "",
        }),
      }),
    );
    expect(literalString(new StringInput(`"abcdefgh01234"`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: "abcdefgh01234",
        }),
      }),
    );
    expect(literalString(new StringInput(`"abdc\\"edf\\ngh\\tzu"`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: 'abdc"edf\ngh\tzu',
        }),
      }),
    );
  });

  it("should parse literal int", () => {
    expect(literalInt(new StringInput(""))).toBeFailure();
    expect(literalInt(new StringInput(`123456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: 123456,
        }),
      }),
    );
    expect(literalInt(new StringInput(`-123_456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: -123456,
        }),
      }),
    );
    expect(literalInt(new StringInput(`0x12_34_56`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: 1193046,
        }),
      }),
    );
    expect(literalInt(new StringInput(`-0x123456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: -1193046,
        }),
      }),
    );
    expect(literalInt(new StringInput(`0o123456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: 42798,
        }),
      }),
    );
    expect(literalInt(new StringInput(`0b1010010011`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: 659,
        }),
      }),
    );
  });

  it("should parse literal float", () => {
    expect(literalFloat(new StringInput(""))).toBeFailure();
    expect(literalFloat(new StringInput(`123.456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: 123.456,
        }),
      }),
    );
    expect(literalFloat(new StringInput(`-123.456`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: -123.456,
        }),
      }),
    );
    expect(literalFloat(new StringInput(`1.23456E4`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: 12345.6,
        }),
      }),
    );
  });

  it("should parse literal bool", () => {
    expect(literalBool(new StringInput(""))).toBeFailure();
    expect(literalBool(new StringInput(`false`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: false,
        }),
      }),
    );
    expect(literalBool(new StringInput(`true`))).toEqual(
      expect.objectContaining({
        success: true,
        value: expect.objectContaining({
          value: true,
        }),
      }),
    );
  });
});
