import "../parser/matchers";
import { identifier, identifierPath } from "../../src/to2/parser-common";
import { StringInput } from "../parser/string-input";

describe("TO2 parser commons", () => {
  it("should parse identifieres", () => {
    expect(identifier(new StringInput(""))).toBeFailure();
    expect(identifier(new StringInput("01234"))).toBeFailure();
    expect(identifier(new StringInput("_01234"))).toBeSuccess();
    expect(identifier(new StringInput("a01234"))).toBeSuccess();
    expect(identifier(new StringInput("abcABD"))).toBeSuccess();
  });

  it("should parser identifier paths", () => {
    expect(identifierPath(new StringInput(""))).toBeFailure();
    expect(identifierPath(new StringInput("0124"))).toBeFailure();
    expect(identifierPath(new StringInput("abc"))).toBeSuccess();
    expect(identifierPath(new StringInput("abc::edf"))).toBeSuccess();
    expect(identifierPath(new StringInput("abc::edf::_02345"))).toBeSuccess();
  });
});
