import referenceJson from "../../src/reference/reference.json";
import { Reference } from "../../src/reference";

describe("Reference", () => {
  it("Should conform to the reference schema", (done) => {
    const result = Reference.safeParse(referenceJson);

    if (!result.success) {
      done(result.error);
    } else {
      done();
    }
  });
});
