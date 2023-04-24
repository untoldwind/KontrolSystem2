import { getCategory } from "unicode-properties";

describe("Unicode categories", () => {
  test("space should be Zs", () => {
    expect(getCategory(" ".charCodeAt(0))).toBe("Zs");
  });
});
