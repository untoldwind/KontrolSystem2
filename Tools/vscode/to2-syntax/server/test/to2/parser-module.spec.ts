import "../parser/matchers";
import { module } from "../../src/to2/parser-module";
import { StringInput } from "../parser/string-input";
import { ErrorNode, isErrorNode } from "../../src/to2/ast/error-node";

describe("TO2 parser module", () => {
  it("should parse a simple module file", () => {
    const source = `
        use { Vessel } from ksp::vessel
        use { CONSOLE } from ksp::console

        pub fn test_fun() -> Unit = {}
    `;
    const moduleParser = module("<test>");
    const moduleResult = moduleParser(new StringInput(source));

    expect(moduleResult).toBeSuccess();
    expect(moduleResult.value?.items).toHaveLength(3);
  });

  it("should parse module file with recoverable errors", () => {
    const source = `
        use { Vessel } from ksp::vessel
        use { CONSOLE } from ksp::console

        123thisshould be broken

        pub fn test_fun() -> Unit = {}
    `;
    const moduleParser = module("<test>");
    const moduleResult = moduleParser(new StringInput(source));

    expect(moduleResult).toBeSuccess();
    expect(moduleResult.value?.items).toHaveLength(4);

    const errors = moduleResult.value?.reduceNode((errors, node) => {
      if (isErrorNode(node)) errors.push(node);
      return errors;
    }, [] as ErrorNode[]);

    expect(errors).toHaveLength(1);
  });

  it("should parse module file with recoverable errors in function", () => {
    const source = `
        use { Vessel } from ksp::vessel
        use { CONSOLE } from ksp::console

        pub fn test_fun() -> Unit = {
          const a = 1

          *()123 thisshould be broken

          CONSOLE.print_line(">>" + a.to_string())
        }
    `;
    const moduleParser = module("<test>");
    const moduleResult = moduleParser(new StringInput(source));

    expect(moduleResult).toBeSuccess();
    expect(moduleResult.value?.items).toHaveLength(3);

    const errors = moduleResult.value?.reduceNode((errors, node) => {
      if (isErrorNode(node)) errors.push(node);
      return errors;
    }, [] as ErrorNode[]);

    expect(errors).toHaveLength(2);
  });
});
