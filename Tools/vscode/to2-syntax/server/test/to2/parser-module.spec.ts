import "../parser/matchers";
import { module } from "../../src/to2/parser-module";
import { StringInput } from "../parser/string-input";

describe("TO2 parser module", () => {
  it("should parse a simple module file", () => {
    const source = `
        use { Vessel } from ksp::vessel
        use { CONSOLE } from ksp::console
    `;
    const moduleParser = module("<test>");

    expect(moduleParser(new StringInput(source))).toBeSuccess();
  });
});
