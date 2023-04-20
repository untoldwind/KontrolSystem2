import { ParserResult } from "../../src/parser/index";

declare global {
  namespace jest {
    interface Matchers<R> {
      toBeSuccess(): CustomMatcherResult;
      toBeFailure(): CustomMatcherResult;
      toHaveRemaining(remaining: string): CustomMatcherResult;
    }
  }
}

expect.extend({
  toBeSuccess(received: ParserResult<any>): jest.CustomMatcherResult {
    const pass: boolean =
      received.success && received.remaining.available === 0;
    const message: () => string = () => {
      if (received.success) {
        const available = received.remaining.available;
        if (available > 0) {
          return `Remaining input: ${received.remaining.take(available)}`;
        }
        return "";
      } else {
        return `Not successful, expected: ${received.expected}`;
      }
    };

    return {
      message,
      pass,
    };
  },

  toBeFailure(received: ParserResult<any>): jest.CustomMatcherResult {
    const pass: boolean = !received.success;
    const message: () => string = () => (pass ? "" : `Successful: ${received}`);

    return {
      message,
      pass,
    };
  },

  toHaveRemaining(
    received: ParserResult<any>,
    remaining: string
  ): jest.CustomMatcherResult {
    const pass: boolean =
      received.success &&
      received.remaining.take(received.remaining.available) === remaining;
    const message: () => string = () => {
      if (received.success) {
        const actualRemaining = received.remaining.take(
          received.remaining.available
        );
        if (actualRemaining !== remaining) {
          return `Unexpected remaining input: "${actualRemaining}" != "${remaining}"`;
        }
        return "";
      } else {
        return `Not successful, expected: ${received.expected}`;
      }
    };

    return {
      message,
      pass,
    };
  },
});
