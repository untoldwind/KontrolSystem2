import "./matchers";
import { digits0, digits1, whitespace0 } from "../../src/parser/complete";
import { StringInput } from "./string-input";

describe("Complete parsers", () => {
  it("should parse whitespace", () => {
    expect(whitespace0(new StringInput(""))).toBeSuccess();
    expect(whitespace0(new StringInput(" \n\t\v"))).toBeSuccess();
    expect(whitespace0(new StringInput("   01234"))).toHaveRemaining("01234");
  });

  it("should parse digits", () => {
    expect(digits0(new StringInput(""))).toBeSuccess();
    expect(digits0(new StringInput("0123456789"))).toBeSuccess();
    expect(digits1(new StringInput("1"))).toBeSuccess();
    expect(digits1(new StringInput("0123456789"))).toBeSuccess();
    expect(digits0(new StringInput("0123456789abc"))).toHaveRemaining("abc");
    expect(digits1(new StringInput("0123456789abc"))).toHaveRemaining("abc");
    expect(digits1(new StringInput("abc"))).toBeFailure();
  });
});
