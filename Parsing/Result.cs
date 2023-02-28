using System;
using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.Parsing {
    public interface IResult<out T> {
        T Value { get; }

        IInput Remaining { get; }

        bool WasSuccessful { get; }

        List<string> Expected { get; }

        Position Position { get; }

        IResult<U> Map<U>(Func<T, U> f);

        IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next);
    }

    public static class Result {
        public static IResult<T> Success<T>(IInput remaining, T value) => new SuccessResult<T>(remaining, value);

        public static IResult<T> Failure<T>(IInput input, string expected) =>
            new FailureResult<T>(input, new List<string> { expected });

        public static IResult<T> Failure<T>(IInput input, IEnumerable<string> expected) =>
            new FailureResult<T>(input, expected.ToList());

        private readonly struct SuccessResult<T> : IResult<T> {
            public T Value { get; }

            public IInput Remaining { get; }

            internal SuccessResult(IInput remaining, T value) {
                Remaining = remaining;
                Value = value;
            }

            public bool WasSuccessful => true;

            public List<string> Expected => new List<string>();

            public Position Position => Remaining.Position;

            public IResult<U> Map<U>(Func<T, U> f) => new SuccessResult<U>(Remaining, f(Value));

            public IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next) => next(this);
        }

        private readonly struct FailureResult<T> : IResult<T> {
            public IInput Remaining { get; }
            public List<string> Expected { get; }

            internal FailureResult(IInput input, List<string> expected) {
                Remaining = input;
                Expected = expected;
            }

            public T Value => throw new InvalidOperationException("Failure has no value");

            public bool WasSuccessful => false;

            public Position Position => Remaining.Position;

            public IResult<U> Map<U>(Func<T, U> f) => new FailureResult<U>(Remaining, Expected);

            public IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next) => new FailureResult<U>(Remaining, Expected);
        }
    }
}
