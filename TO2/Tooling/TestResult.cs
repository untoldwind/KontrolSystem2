using System;
using System.Collections.Generic;

namespace KontrolSystem.TO2.Tooling;

public enum TestResultState {
    Success,
    Failure,
    Error
}

public readonly struct TestResult {
    public readonly TestResultState state;
    public readonly string testName;
    public readonly int successfulAssertions;
    public readonly int stackCount;
    public readonly string? failure;
    public readonly Exception? exception;
    public readonly IEnumerable<string> messages;

    public TestResult(string testName, int successfulAssertions, int stackCount, IEnumerable<string> messages) {
        state = TestResultState.Success;
        this.testName = testName;
        this.successfulAssertions = successfulAssertions;
        this.stackCount = stackCount;
        failure = null;
        exception = null;
        this.messages = messages;
    }

    public TestResult(string testName, int successfulAssertions, int stackCount, string failure,
        IEnumerable<string> messages) {
        state = TestResultState.Failure;
        this.testName = testName;
        this.successfulAssertions = successfulAssertions;
        this.stackCount = stackCount;
        this.failure = failure;
        exception = null;
        this.messages = messages;
    }

    public TestResult(string testName, int successfulAssertions, int stackCount, Exception exceptions,
        IEnumerable<string> messages) {
        state = TestResultState.Error;
        this.testName = testName;
        this.successfulAssertions = successfulAssertions;
        this.stackCount = stackCount;
        failure = null;
        exception = exceptions;
        this.messages = messages;
    }
}
