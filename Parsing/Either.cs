using System;

namespace KontrolSystem.Parsing {
    public interface IEither<out L, out R> {
        bool IsLeft { get; }

        bool IsRight { get; }

        L Left { get; }

        R Right { get; }
    }

    public static class Either {
        public static IEither<L, R> Left<L, R>(L value) => new LeftEither<L, R>(value);

        public static IEither<L, R> Right<L, R>(R value) => new RightEither<L, R>(value);

        private struct LeftEither<L, R> : IEither<L, R> {
            private readonly L value;

            internal LeftEither(L value) => this.value = value;

            public bool IsLeft => true;

            public bool IsRight => false;

            public L Left => value;

            public R Right => throw new InvalidOperationException("Either has no right");
        }

        private struct RightEither<L, R> : IEither<L, R> {
            private readonly R value;

            internal RightEither(R value) => this.value = value;

            public bool IsLeft => false;

            public bool IsRight => true;

            public L Left => throw new InvalidOperationException("Either has no left");

            public R Right => value;

        }
    }
}
